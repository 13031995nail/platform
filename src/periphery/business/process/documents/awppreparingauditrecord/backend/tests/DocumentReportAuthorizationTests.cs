namespace Tmobis.Business.Process.Documents.Tests;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Tmobis.Business.Common.Address;
using Tmobis.Business.Common.Personnel;
using Tmobis.Business.Process.Documents.Providers;
using Tmobis.Core.Models;
using Tmobis.Core.Medium.Producers;
using Tmobis.Reports.Adapter;
using Tmobis.Reports.Adapter.SecondaryAdapters;
using Tmobis.Reports.Adapter.UseCases;
using Tmobis.Reports.Controller;
using Tmobis.Reports.Models;
using Xunit;

public sealed class DocumentReportAuthorizationTests
{
    [Fact]
    public async Task RegisteredModulesResolveAuthorizationBeforeReportAdapter()
    {
        var fixture = new DocumentBoundaryFixture();
        fixture.Roles.Names = [WorkObjectRoleNames.Administration];
        var document = AddDocument(fixture, "RedinessAct", fixture.Object);
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();
        new BaseReportAdapterModule().Register(services, configuration, null!);
        new AwpPreparingAuditRecordModule().Register(services, configuration, null!);
        services.AddSingleton(fixture.Repository);
        services.AddSingleton<IWorkObjectAccessProvider>(fixture.Access);
        services.AddSingleton<Business.Core.Providers.Roles.ICurrentRoleProvider>(fixture.Roles);
        services.AddSingleton<Business.Core.Providers.Users.ICurrentUserProvider>(fixture.Users);
        services.AddScoped<AdmissionCommitteeMembershipProvider>();
        services.AddScoped<EvaluationSheetPermissionProvider>();
        services.AddSingleton(DispatchProxy.Create<IMessageDispatcher, UnusedDispatcher>());
        var adapter = new Adapter();
        services.AddSingleton<IReportServiceAdapter>(adapter);
        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        Assert.IsType<WorkObjectDocumentReportAuthorization>(
            Assert.Single(scope.ServiceProvider.GetServices<IReportAuthorizationProvider>()));
        var useCase = scope.ServiceProvider.GetRequiredService<IReportUseCase>();

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => useCase.GetFile(Run("RedinessAct", document.Id)));
        Assert.Null(adapter.Request);
        fixture.AllowForRole(WorkObjectRoleNames.Administration, fixture.Object);
        Assert.NotEmpty(await useCase.GetFile(Run("RedinessAct", document.Id)));
    }

    [Fact]
    public async Task AllowedHtmlUsesSameAuthorizationAndGenerationPath()
    {
        var fixture = new DocumentBoundaryFixture();
        fixture.Roles.Names = [WorkObjectRoleNames.Administration];
        fixture.AllowForRole(WorkObjectRoleNames.Administration, fixture.Object);
        var document = AddDocument(fixture, "RedinessAct", fixture.Object);
        var adapter = new Adapter();
        Assert.IsType<ContentResult>(await Controller(fixture, adapter).GetHtml(Run("RedinessAct", document.Id), null));
        Assert.Equal(ReportFormat.Html, adapter.Request!.Format);
    }

    [Theory]
    [InlineData("RedinessAct", false)]
    [InlineData("RedinessAct", true)]
    [InlineData("AwpReadinessPassport", false)]
    [InlineData("AwpReadinessPassport", true)]
    [InlineData("EvaluationSheet", false)]
    [InlineData("EvaluationSheet", true)]
    [InlineData("AwpAssessingProgram", false)]
    [InlineData("AwpAssessingProgram", true)]
    public async Task FileAndHtmlDenyForeignDocumentBeforeCallingAdapter(string template, bool html)
    {
        var fixture = new DocumentBoundaryFixture();
        var document = AddDocument(fixture, template, fixture.Object);
        var adapter = new Adapter();
        var controller = Controller(fixture, adapter);
        var parameters = Run(template, document.Id);

        var result = html ? await controller.GetHtml(parameters, null) : await controller.GetFile(parameters, null);

        Assert.IsType<ForbidResult>(result);
        Assert.Null(adapter.Request);
    }

    [Theory]
    [InlineData("RedinessAct")]
    [InlineData("AwpReadinessPassport")]
    [InlineData("EvaluationSheet")]
    [InlineData("AwpAssessingProgram")]
    public async Task ConsumerCannotPrintOfficialDocumentEvenWithReadAccess(string template)
    {
        var fixture = new DocumentBoundaryFixture();
        fixture.Roles.Names = [WorkObjectRoleNames.Consumer];
        fixture.AllowForRole(WorkObjectRoleNames.Consumer, fixture.Object);
        var document = AddDocument(fixture, template, fixture.Object);
        var adapter = new Adapter();
        var parameters = new ReportRunParams
        {
            TemplateName = template, Format = "pdf", ParamsList = [new("PK", document.Id.ToString("B"))],
        };

        Assert.IsType<ForbidResult>(await Controller(fixture, adapter).GetFile(parameters, null));
        Assert.Null(adapter.Request);
    }

    [Theory]
    [InlineData("RedinessAct")]
    [InlineData("AwpReadinessPassport")]
    [InlineData("EvaluationSheet")]
    [InlineData("AwpAssessingProgram")]
    public async Task TsoCanPrintReadableDocumentUsingCanonicalParamsListId(string template)
    {
        var fixture = new DocumentBoundaryFixture();
        fixture.Roles.Names = [WorkObjectRoleNames.Tso];
        fixture.AllowForRole(WorkObjectRoleNames.Tso, fixture.Object);
        var document = AddDocument(fixture, template, fixture.Object);
        var adapter = new Adapter();
        var parameters = new ReportRunParams
        {
            TemplateName = template, Format = "pdf", ParamsList = [new("PK", document.Id.ToString("B"))],
        };

        Assert.IsType<FileContentResult>(await Controller(fixture, adapter).GetFile(parameters, null));
        Assert.Equal(document.Id.ToString("D"), adapter.Request!.Params!["pk"]);
    }

    [Theory]
    [InlineData("RedinessAct")]
    [InlineData("AwpReadinessPassport")]
    [InlineData("AwpAssessingProgram")]
    public async Task EveryLinkedObjectMustBeReadable(string template)
    {
        var fixture = new DocumentBoundaryFixture();
        fixture.Roles.Names = [WorkObjectRoleNames.Administration];
        fixture.AllowForRole(WorkObjectRoleNames.Administration, fixture.Object);
        var foreign = fixture.Add(new WorkObject { Organization = new Common.Contacts.Organization() });
        var document = AddDocument(fixture, template, fixture.Object, foreign);
        var adapter = new Adapter();
        var controller = Controller(fixture, adapter);

        Assert.IsType<ForbidResult>(await controller.GetFile(Run(template, document.Id), null));
        Assert.Null(adapter.Request);
        fixture.AllowForRole(WorkObjectRoleNames.Administration, fixture.Object, foreign);
        Assert.IsType<FileContentResult>(await controller.GetFile(Run(template, document.Id), null));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("null")]
    [InlineData("not-a-guid")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public async Task ProtectedTemplateRejectsMissingOrInvalidId(string? pk)
    {
        var fixture = new DocumentBoundaryFixture();
        var adapter = new Adapter();
        var run = new ReportRunParams { TemplateName = "RedinessAct", Format = "pdf" };
        run.Params = pk == null ? null : new() { ["pk"] = pk };
        Assert.IsType<ForbidResult>(await Controller(fixture, adapter).GetFile(run, null));
        Assert.Null(adapter.Request);
    }

    [Theory]
    [InlineData("folder/RedinessAct.mrt", null)]
    [InlineData("folder\\RedinessAct.dll", null)]
    [InlineData("", "/RedinessAct.mrt")]
    [InlineData("RedinessAct", "_variant")]
    [InlineData("rediness-act", null)]
    public async Task TemplateAndSubtypeAliasesCannotSkipAuthorization(string template, string? subtype)
    {
        var adapter = new Adapter();
        Assert.IsType<ForbidResult>(await Controller(new(), adapter).GetFile(Run(template, Guid.NewGuid()), subtype));
        Assert.Null(adapter.Request);
    }

    [Fact]
    public async Task UnregisteredTemplateWithDocumentIdReachesAdapter()
    {
        const string template = "SomeContract.mrt";
        var adapter = new Adapter();
        var documentId = Guid.NewGuid();

        var result = await Controller(new(), adapter)
            .GetFile(Run(template, documentId), null);

        Assert.IsType<FileContentResult>(result);
        Assert.Equal(template, adapter.Request!.TemplateName);
        Assert.Equal(documentId.ToString(), adapter.Request.Params!["pk"]);
    }

    [Fact]
    public async Task ExplicitlyRegisteredAliasUsesItsDocumentPolicy()
    {
        var fixture = new DocumentBoundaryFixture();
        fixture.Roles.Names = [WorkObjectRoleNames.Administration];
        fixture.AllowForRole(WorkObjectRoleNames.Administration, fixture.Object);
        var document = AddDocument(fixture, "RedinessAct", fixture.Object);
        var registration = new Tmobis.Business.Process.Documents.Providers.WorkObjectDocumentReportRegistration(
            "CommissionFinalForm",
            Tmobis.Business.Process.Documents.Providers.WorkObjectDocumentReportType.ReadinessAct,
            Tmobis.Business.Process.Documents.Providers.WorkObjectDocumentReportPolicy.WorkObjectScope);
        var registrations = new List<Tmobis.Business.Process.Documents.Providers.WorkObjectDocumentReportRegistration>(
            Tmobis.Business.Process.Documents.Providers.WorkObjectDocumentReportRegistrations.BuiltIn)
        {
            registration,
        };
        var adapter = new Adapter();

        var result = await Controller(fixture, adapter, registrations)
            .GetFile(Run("CommissionFinalForm", document.Id), null);

        Assert.IsType<FileContentResult>(result);
        Assert.NotNull(adapter.Request);
    }

    [Fact]
    public async Task ExplicitlyRegisteredAliasCannotSkipItsDocumentPolicy()
    {
        var fixture = new DocumentBoundaryFixture();
        var document = AddDocument(fixture, "RedinessAct", fixture.Object);
        var registration = new Tmobis.Business.Process.Documents.Providers.WorkObjectDocumentReportRegistration(
            "CommissionFinalForm",
            Tmobis.Business.Process.Documents.Providers.WorkObjectDocumentReportType.ReadinessAct,
            Tmobis.Business.Process.Documents.Providers.WorkObjectDocumentReportPolicy.WorkObjectScope);
        var registrations = new List<Tmobis.Business.Process.Documents.Providers.WorkObjectDocumentReportRegistration>(
            Tmobis.Business.Process.Documents.Providers.WorkObjectDocumentReportRegistrations.BuiltIn)
        {
            registration,
        };
        var adapter = new Adapter();

        var result = await Controller(fixture, adapter, registrations)
            .GetFile(Run("CommissionFinalForm", document.Id), null);

        Assert.IsType<ForbidResult>(result);
        Assert.Null(adapter.Request);
    }

    [Fact]
    public async Task UnregisteredTemplateWithParamsListDocumentIdReachesAdapter()
    {
        var adapter = new Adapter();
        var parameters = new ReportRunParams
        {
            TemplateName = "GenericReportWithPk",
            Format = "pdf",
            ParamsList = [new("PK", Guid.NewGuid().ToString())],
        };

        var result = await Controller(new(), adapter).GetFile(parameters, null);

        Assert.IsType<FileContentResult>(result);
        Assert.Equal(parameters.TemplateName, adapter.Request!.TemplateName);
    }

    [Fact]
    public async Task ConflictingIdsAreRejectedAndUnrelatedGenericReportIsPreserved()
    {
        var fixture = new DocumentBoundaryFixture();
        fixture.Allow(fixture.Object);
        var document = AddDocument(fixture, "RedinessAct", fixture.Object);
        var run = Run("RedinessAct", document.Id);
        run.ParamsList = [new("pk", Guid.NewGuid().ToString())];
        var adapter = new Adapter();
        Assert.IsType<ForbidResult>(await Controller(fixture, adapter).GetFile(run, null));
        Assert.Null(adapter.Request);
        Assert.IsType<FileContentResult>(await Controller(fixture, adapter).GetFile(
            new ReportRunParams { TemplateName = "UnrelatedReport", Format = "pdf" }, null));
    }

    private static ReportController Controller(
        DocumentBoundaryFixture fixture,
        Adapter adapter,
        IEnumerable<Tmobis.Business.Process.Documents.Providers.WorkObjectDocumentReportRegistration>? registrations = null)
    {
        var memberships = new AdmissionCommitteeMembershipProvider(
            fixture.Repository,
            fixture.Roles,
            fixture.Users);
        var permissions = new EvaluationSheetPermissionProvider(
            fixture.Repository,
            fixture.Access,
            fixture.Roles,
            memberships);
        var authorization = new WorkObjectDocumentReportAuthorization(
            fixture.Repository,
            fixture.Access,
            permissions,
            registrations ?? Tmobis.Business.Process.Documents.Providers.WorkObjectDocumentReportRegistrations.BuiltIn);
        return new(new ReportUseCase(adapter, null!, [authorization]), NullLogger<ReportController>.Instance);
    }

    private static ReportRunParams Run(string template, Guid id)
        => new() { TemplateName = template, Format = "pdf", Params = new() { ["pk"] = id.ToString() } };

    private static IEntity AddDocument(DocumentBoundaryFixture fixture, string template, params WorkObject[] objects)
    {
        switch (template)
        {
            case "RedinessAct":
                var act = fixture.Add(new RedinessAct { Awp = fixture.Awp });
                foreach (var item in objects) fixture.Add(new RedinessActObject { Act = act, WorkObject = item });
                return act;
            case "AwpReadinessPassport":
                var passport = fixture.Add(new AwpReadinessPassport { Awp = fixture.Awp });
                foreach (var item in objects) fixture.Add(new AwpReadinessPassportObject { Act = passport, WorkObject = item });
                return passport;
            case "AwpAssessingProgram":
                var program = fixture.Add(new AwpAssessingProgram { Awp = fixture.Awp });
                foreach (var item in objects) fixture.Add(new AwpAssessingProgramObject { Program = program, WorkObject = item });
                return program;
            default:
                return fixture.Add(new EvaluationSheet { Awp = fixture.Awp, WorkObject = objects[0] });
        }
    }

    private sealed class Adapter : IReportServiceAdapter
    {
        public GetReportRequest? Request { get; private set; }
        public Task<byte[]> BuildReport(GetReportRequest request, bool throwOnError = false)
        {
            Request = request;
            return Task.FromResult<byte[]>([1]);
        }
    }

    public class UnusedDispatcher : DispatchProxy
    {
        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
            => throw new NotSupportedException();
    }
}
