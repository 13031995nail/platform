namespace Tmobis.Integrations.Messengers.Max.Consumers;

using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tmobis.Core.Medium.Consumers;
using Tmobis.Core.Medium.Producers;
using Tmobis.Integrations.Messengers.Messages;
using Tmobis.Integrations.Messengers.Max.Messages;

public partial class MaxMessageCreatedConsumer(
    IMessageDispatcher messageDispatcher,
    IMaxAdapter maxAdapter,
    IOptions<MaxOptions> options,
    ILogger<MaxMessageCreatedConsumer> logger)
    : IMessageConsumer<MaxMessageCreated>
{
    private readonly IMessageDispatcher messageDispatcher = messageDispatcher;
    private readonly IMaxAdapter maxAdapter = maxAdapter;
    private readonly MaxOptions options = options.Value;
    private readonly ILogger<MaxMessageCreatedConsumer> logger = logger;

    public async Task Consume(MaxMessageCreated message)
    {
        var chatId = message.Message?.Recipient?.ChatId;
        logger.LogInformation(
            "Запрос мессенджера {Channel}: {Direction}, операция {Operation}, идентификатор {MessengerId}, событие {EventType}",
            "MAX",
            "Incoming",
            "UpdateReceived",
            chatId,
            message.UpdateType);

        if (!string.Equals(
            message.UpdateType,
            "message_created",
            StringComparison.Ordinal))
        {
            return;
        }

        if (chatId is null or 0)
        {
            logger.LogWarning(
                "Запрос мессенджера {Channel}: {Direction}, событие {EventType} не содержит идентификатор",
                "MAX",
                "Incoming",
                message.UpdateType);
            return;
        }

        var attachments = message.Message?.Body?.Attachments;
        var contact = attachments?.FirstOrDefault(attachment =>
                string.Equals(
                    attachment.Type,
                    "contact",
                    StringComparison.OrdinalIgnoreCase))
            ?.Payload;
        var result = await messageDispatcher.SendWithResult<
            MessengerMessageReceived,
            MessengerMessageResult>(new MessengerMessageReceived
            {
                MessengerCode = MaxOptions.MessengerCode,
                MessengerName = MaxOptions.MessengerName,
                ExternalId = chatId.Value.ToString(
                    CultureInfo.InvariantCulture),
                HasContact = contact != null,
                ContactIsTrusted = contact != null
                    && IsTrustedContact(
                        contact.VcfInfo,
                        contact.Hash,
                        options.AccessToken),
                Phone = contact == null
                    ? null
                    : ExtractPhone(contact.VcfInfo)
            });

        switch (result.Action)
        {
            case MessengerResponseAction.RequestContact:
                await maxAdapter.RequestContact(chatId.Value);
                break;
            case MessengerResponseAction.SendMessage
                when !string.IsNullOrWhiteSpace(result.Text):
                await maxAdapter.SendMessage(chatId.Value, result.Text);
                break;
        }
    }

    internal static string? ExtractPhone(string vcfInfo)
    {
        var match = PhoneRegex().Match(UnescapeVcfInfo(vcfInfo));
        return match.Success
            ? match.Groups["phone"].Value
            : null;
    }

    internal static bool IsTrustedContact(
        string vcfInfo,
        string hash,
        string accessToken)
    {
        if (string.IsNullOrWhiteSpace(vcfInfo)
            || string.IsNullOrWhiteSpace(hash)
            || string.IsNullOrWhiteSpace(accessToken))
        {
            return false;
        }

        var actualHash = DecodeHash(hash);
        if (actualHash is not { Length: 32 })
        {
            return false;
        }

        using var hmac = new HMACSHA256(
            Encoding.UTF8.GetBytes(accessToken.Trim()));
        foreach (var vcfCandidate in GetVcfCandidates(vcfInfo))
        {
            var expectedHash = hmac.ComputeHash(
                Encoding.UTF8.GetBytes(vcfCandidate));
            if (CryptographicOperations.FixedTimeEquals(
                expectedHash,
                actualHash))
            {
                return true;
            }
        }

        return false;
    }

    private static IEnumerable<string> GetVcfCandidates(string vcfInfo)
    {
        yield return vcfInfo;

        var unescaped = UnescapeVcfInfo(vcfInfo);
        if (!string.Equals(unescaped, vcfInfo, StringComparison.Ordinal))
        {
            yield return unescaped;
        }

        var crlf = unescaped
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace("\r", "\n", StringComparison.Ordinal)
            .Replace("\n", "\r\n", StringComparison.Ordinal);
        if (!string.Equals(crlf, unescaped, StringComparison.Ordinal)
            && !string.Equals(crlf, vcfInfo, StringComparison.Ordinal))
        {
            yield return crlf;
        }
    }

    private static string UnescapeVcfInfo(string vcfInfo)
        => vcfInfo
            .Replace("\\r\\n", "\r\n", StringComparison.Ordinal)
            .Replace("\\n", "\n", StringComparison.Ordinal)
            .Replace("\\r", "\r", StringComparison.Ordinal);

    private static byte[]? DecodeHash(string hash)
    {
        try
        {
            var normalizedHash = hash.Trim();
            if (normalizedHash.StartsWith(
                "sha256=",
                StringComparison.OrdinalIgnoreCase))
            {
                normalizedHash = normalizedHash["sha256=".Length..];
            }

            if (normalizedHash.StartsWith(
                "0x",
                StringComparison.OrdinalIgnoreCase))
            {
                normalizedHash = normalizedHash[2..];
            }

            if (normalizedHash.Length == 64
                && normalizedHash.All(Uri.IsHexDigit))
            {
                return Convert.FromHexString(normalizedHash);
            }

            var base64 = normalizedHash
                .Replace('-', '+')
                .Replace('_', '/');
            base64 = base64.PadRight(
                base64.Length + ((4 - (base64.Length % 4)) % 4),
                '=');
            return Convert.FromBase64String(base64);
        }
        catch (FormatException)
        {
            return null;
        }
    }

    [GeneratedRegex(
        @"(?im)^TEL(?:;[^:]*)?:(?<phone>[^\r\n]+)",
        RegexOptions.CultureInvariant)]
    private static partial Regex PhoneRegex();
}
