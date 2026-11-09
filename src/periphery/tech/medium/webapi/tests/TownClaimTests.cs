namespace Tmobis.Medium.WebApi.Tests;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Tmobis.Auth;
using Tmobis.Auth.JwtAuth.Options;
using Tmobis.Auth.JwtAuth.Providers;
using Tmobis.Business.Core.Models.Users;
using Tmobis.Business.Core.TownLimit;
using Tmobis.Frameworks.Web.Backend.Users;
using Xunit;

public sealed class TownClaimTests
{
    [Fact]
    public async Task JwtIncludesSignedTownClaim()
    {
        var townId = Guid.NewGuid();
        var user = new User { Login = "test", TownId = townId };
        var builder = new JwtTokenBuilder(
            Options.Create(new JwtAuthOptions()), null!);

        var response = JObject.Parse(await builder.GetToken(
            new AuthTokenData(user, [], [], [])));

        var token = new JwtSecurityTokenHandler().ReadJwtToken(
            response.Value<string>("accessToken")!);

        Assert.Equal(
            townId.ToString("D"),
            token.Claims.Single(c => c.Type == TownClaimTypes.TownId).Value);
    }

    [Fact]
    public async Task WebTownProviderUsesAuthenticatedClaimWithoutDatabaseRead()
    {
        var townId = Guid.NewGuid();
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    [new Claim(TownClaimTypes.TownId, townId.ToString("D"))],
                    "Bearer"))
        };
        // null разрешён здесь намеренно: чтение провайдера из БД
        // при наличии claim не должно происходить.
        var provider = new WebCurrentTownProvider(
            new HttpContextAccessor { HttpContext = context }, null!);

        Assert.True(provider.TryGetCachedTownId(out var cached));
        Assert.Equal(townId, cached);
        Assert.Equal(townId, await provider.GetCurrentTownId());
    }

    [Fact]
    public void InvalidAuthenticatedTownClaimIsRejected()
    {
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    [new Claim(TownClaimTypes.TownId, "not-a-guid")],
                    "Bearer"))
        };
        var provider = new WebCurrentTownProvider(
            new HttpContextAccessor { HttpContext = context }, null!);

        Assert.Throws<InvalidOperationException>(
            () => provider.TryGetCachedTownId(out _));
    }

    [Fact]
    public async Task OldJwtGetsTownClaimBeforeFirstSynchronousQuery()
    {
        var townId = Guid.NewGuid();
        var principal = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim(ClaimTypes.Name, "legacy-user")], "Bearer"));

        var resolvedLogin = "";
        var resolved = await LegacyTownClaimInitializer.EnsureTownClaimAsync(
            principal,
            login =>
            {
                resolvedLogin = login;
                return Task.FromResult<Guid?>(townId);
            });

        Assert.True(resolved);
        Assert.Equal("legacy-user", resolvedLogin);

        var context = new DefaultHttpContext { User = principal };
        var provider = new WebCurrentTownProvider(
            new HttpContextAccessor { HttpContext = context }, null!);
        Assert.True(provider.TryGetCachedTownId(out var cached));
        Assert.Equal(townId, cached);
    }

    [Fact]
    public async Task NewJwtDoesNotQueryUserToResolveTown()
    {
        var townId = Guid.NewGuid();
        var principal = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim(TownClaimTypes.TownId, townId.ToString("D"))],
            "Bearer"));

        var calls = 0;
        Assert.True(await LegacyTownClaimInitializer.EnsureTownClaimAsync(
            principal, _ =>
            {
                calls++;
                return Task.FromResult<Guid?>(null);
            }));
        Assert.Equal(0, calls);
    }

    [Fact]
    public async Task MissingOrInvalidTownIsRejected()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim(ClaimTypes.Name, "legacy-user")], "Bearer"));

        Assert.False(await LegacyTownClaimInitializer.EnsureTownClaimAsync(
            principal, _ => Task.FromResult<Guid?>(Guid.Empty)));
        Assert.Null(principal.FindFirst(TownClaimTypes.TownId));

        var invalid = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim(TownClaimTypes.TownId, "invalid")], "Bearer"));
        Assert.False(await LegacyTownClaimInitializer.EnsureTownClaimAsync(
            invalid, _ => throw new InvalidOperationException(
                "Lookup must not run for an invalid claim")));
    }

    [Fact]
    public async Task OldJwtTownCacheAvoidsRepeatedLookup()
    {
        using var memory = new MemoryCache(new MemoryCacheOptions());
        var cache = new LegacyTownIdCache(memory, Options.Create(new JwtAuthOptions()));
        var townId = Guid.NewGuid();
        var calls = 0;
        async Task<Guid?> Resolve(string _) { calls++; return await Task.FromResult<Guid?>(townId); }

        Assert.Equal(townId, await cache.ResolveAsync("old-user", Resolve));
        Assert.Equal(townId, await cache.ResolveAsync("old-user", Resolve));
        Assert.Equal(1, calls);
        Assert.Equal(townId, await cache.ResolveAsync("another-user", Resolve));
        Assert.Equal(2, calls);
    }

    [Fact]
    public async Task OldJwtTownCacheDoesNotCacheMissingUsers()
    {
        using var memory = new MemoryCache(new MemoryCacheOptions());
        var cache = new LegacyTownIdCache(memory, Options.Create(new JwtAuthOptions()));
        var calls = 0;
        Task<Guid?> Resolve(string _) { calls++; return Task.FromResult<Guid?>(null); }

        Assert.Null(await cache.ResolveAsync("missing-user", Resolve));
        Assert.Null(await cache.ResolveAsync("missing-user", Resolve));
        Assert.Equal(2, calls);
    }

    [Fact]
    public void LegacyTokenWithoutTownClaimKeepsAsyncFallback()
    {
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity([], "Bearer"))
        };
        var provider = new WebCurrentTownProvider(
            new HttpContextAccessor { HttpContext = context }, null!);

        Assert.False(provider.TryGetCachedTownId(out _));
    }
}
