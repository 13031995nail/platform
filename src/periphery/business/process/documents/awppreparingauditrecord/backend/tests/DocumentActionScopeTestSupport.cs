namespace Tmobis.Business.Process.Documents.Tests;

using ICSSoft.STORMNET;
using ICSSoft.STORMNET.FunctionalLanguage;
using Tmobis.Business.Common.Address;
using Tmobis.Business.Common.Contacts;
using Tmobis.Business.Common.Resources;
using Tmobis.Business.Core.Providers.Roles;
using Tmobis.Business.Process.Documents.Base;
using Tmobis.Business.Process.Documents.Flexberry;
using Tmobis.Business.Process.Documents.Providers;
using Tmobis.Core.Models;

internal sealed class DocumentScopeFixture
{
    private readonly List<IEntity> entities = [];
    private readonly string role;
    private readonly TestWorkObjectAccessProvider access = new();

    public List<Type> Reads { get; } = [];
    public Organization Organization { get; } = new();
    public Awp Awp { get; } = new();
    public WorkObject Object { get; }
    public RedinessActAccess ActAccess { get; }
    public AwpReadinessPassportAccess PassportAccess { get; }
    public AwpAssessingProgramAccess ProgramAccess { get; }
    public WorkObjectDocumentsProvider Documents { get; }

    public DocumentScopeFixture(string role)
    {
        this.role = role;
        Object = AddObject(Organization);
        var repository = new InMemoryDocumentRepository(entities, onRead: type => Reads.Add(type));
        var signAccess = new ActSignAccessValidator(repository);
        ActAccess = new(access, signAccess, repository);
        PassportAccess = new(access, signAccess, repository);
        ProgramAccess = new(access, signAccess, repository);
        Roles roles = new(role);
        Documents = new(repository, access, ActAccess, PassportAccess, roles);
    }

    public T Add<T>(T entity) where T : class, IEntity
    {
        entities.Add(entity);
        return entity;
    }

    public WorkObject AddObject(Organization organization)
        => Add(new WorkObject { Organization = organization, Actuality = true });

    public RedinessAct AddAct(WorkObject workObject)
    {
        var act = Add(new RedinessAct { Organization = workObject.Organization, Awp = Awp });
        Add(new RedinessActObject { Act = act, WorkObject = workObject });
        return act;
    }

    public void Allow(params WorkObject[] objects)
    {
        var ids = objects.Select(item => item.Id).ToHashSet();
        access.RoleIds[role] = ids;
        ((HashSet<Guid>)access.GenericIds).UnionWith(ids);
    }

    private sealed class Roles(string role) : ICurrentRoleProvider
    {
        public ValueTask<List<string>> GetCurrentRoleNames() => ValueTask.FromResult<List<string>>([role]);
    }
}

internal static class DocumentActionScopeTestSupport
{
    public static AwpReadinessPassport CreatePassportPatch(
        DocumentScopeFixture fixture,
        RedinessAct readinessAct,
        Awp? awp = null,
        Organization? organization = null,
        RedinessAct? storedReadinessAct = null)
    {
        fixture.Allow(fixture.Object);
        fixture.Add(new RedinessActObject { Act = readinessAct, WorkObject = fixture.Object });
        var storedPassport = fixture.Add(new AwpReadinessPassport
        {
            Organization = fixture.Organization,
            Awp = fixture.Awp,
            ReadinessAct = storedReadinessAct ?? readinessAct,
        });
        storedPassport.SetStatus(ObjectStatus.UnAltered);

        var patch = new AwpReadinessPassport
        {
            Id = storedPassport.Id,
            Organization = organization ?? fixture.Organization,
            Awp = awp ?? fixture.Awp,
            ReadinessAct = readinessAct,
        };
        patch.SetStatus(ObjectStatus.Altered);
        return patch;
    }

    public static void MarkAltered(DataObject entity)
    {
        var properties = entity.GetType().GetProperties()
            .Where(property => property.Name is "Act" or "Program" or "WorkObject")
            .ToDictionary(property => property, property => property.GetValue(entity));
        entity.SetExistObjectPrimaryKey(entity.__PrimaryKey);
        foreach (var (property, value) in properties)
        {
            property.SetValue(entity, value);
        }

        entity.AddLoadedProperties(properties.Keys.Select(property => property.Name).ToArray());
        entity.InitDataCopy();
        foreach (var (property, value) in properties)
        {
            property.SetValue(entity.GetDataCopy(), value);
        }

        if (entity is ActSign sign)
        {
            sign.Fio = "updated metadata";
        }
        else if (entity is AwpAssessingProgramEvaluationSchedule schedule)
        {
            schedule.StartDate = DateTime.UtcNow;
        }
        else
        {
            var parent = properties.Keys.First(property => property.Name is "Act" or "Program");
            parent.SetValue(entity.GetDataCopy(), null);
        }

        entity.SetStatus(ObjectStatus.Altered);
        Xunit.Assert.Equal(ObjectStatus.Altered, entity.GetStatus());
    }
}

internal sealed class DocumentScheduleReadLimits : IWorkObjectReadLimitProvider
{
    public string? MasterProperty { get; private set; }
    public string? OwnerProperty { get; private set; }

    public Task<Function?> GetReadLimit(string workObjectPropertyPath)
        => throw new NotSupportedException();

    public Task<Function?> GetReadLimit(View detailView, string connectMasterProperty,
        string ownerConnectProperty)
    {
        MasterProperty = connectMasterProperty;
        OwnerProperty = ownerConnectProperty;
        return Task.FromResult<Function?>(null);
    }
}
