namespace Tmobis.Audit.Json;

using Business.Core.Providers.Users;
using Core.Repositories;
using Newtonsoft.Json;
using System.Reflection;
using Microsoft.Extensions.Options;

/// <summary>
/// Аудит операций над сущностями в виде JSON слепков 
/// </summary>
public class JsonAuditWriter(
    IRepository repository,
    ICurrentUserProvider currentUserProvider,
    IOptions<AuditOptions> auditOptions)
: IAuditWriter
{
    private static readonly string CurrentService = Assembly.GetEntryAssembly()?.GetName().Name ?? "Unknown";
    
    private readonly AuditOptions auditOptions = auditOptions.Value;

    private static readonly JsonSerializerSettings JsonSerializerOptions = new()
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        PreserveReferencesHandling = PreserveReferencesHandling.None,
    };

    /// <inheritdoc/>
    public async Task AddEvent(AuditRequest request)
    {
        if (!auditOptions.IsEnabled
            || (request.EventType == AuditEventType.Read
                && !auditOptions.IsEnabledRead)
            || request.Entity.GetType()
                .IsAssignableTo(typeof(IAuditEvent))) return;
        
        var currentUser = await currentUserProvider.GetCurrentUser();
        var userConnection = currentUserProvider.GetUserConnectionInfo();
        
        var auditEvent = repository.MakeEntity<IAuditEvent>();
        auditEvent.EntityId = request.Entity.Id;
        auditEvent.UserId = currentUser?.Id;
        auditEvent.EntityType = request.Entity.GetType().Name;
        auditEvent.CreateTime = DateTime.UtcNow;
        auditEvent.Creator = currentUser?.Login;
        auditEvent.EventType = request.EventType;
        auditEvent.Source = $"{userConnection}; Service: {CurrentService}";

        if (auditEvent.EventType != AuditEventType.Read)
        {
            var entityJson = JsonConvert.SerializeObject(
                request.Entity, JsonSerializerOptions);
            auditEvent.EntitySnapshot = entityJson;
        }

        repository.Create(auditEvent);
    }
}
