namespace Tmobis.Business.Process.Documents.Tests;

using System.Linq.Expressions;
using System.Reflection;
using Tmobis.Business.Common.Address;
using Tmobis.Business.Common.Contacts;
using Tmobis.Business.Common.Personnel;
using Tmobis.Business.Common.Resources;
using Tmobis.Business.Core.Flexberry.Models;
using Tmobis.Business.Process.Documents.Flexberry;
using Tmobis.Core.Repositories;
using Xunit;

public sealed class CommitteeSignerSelectionTests
{
    [Fact]
    public async Task AssessingProgramUsesItsLinkedCommittee()
    {
        var awp = new Awp();
        var organization = new Organization();
        var employee = new Employee { Fio = "Председатель связанной комиссии" };
        var committee = CreateCommittee(awp, organization, employee);
        var program = new AwpAssessingProgram
        {
            Awp = awp,
            Organization = organization,
            Committee = committee,
        };
        var repository = CreateRepository(committee);

        var signs = await new TestAssessingProgramCreated(repository)
            .CreateSigns(program);

        var sign = Assert.Single(signs);
        Assert.Equal(employee.Id, sign.Employee.Id);
    }

    [Fact]
    public async Task AssessingProgramDoesNotUseCommitteeFromAnotherHeatingPeriod()
    {
        var awp = new Awp();
        var anotherAwp = new Awp();
        var organization = new Organization();
        var employee = new Employee { Fio = "Председатель другого периода" };
        var committee = CreateCommittee(anotherAwp, organization, employee);
        var program = new AwpAssessingProgram
        {
            Awp = awp,
            Organization = organization,
            Committee = committee,
        };
        var repository = CreateRepository(committee);

        var signs = await new TestAssessingProgramCreated(repository)
            .CreateSigns(program);

        Assert.Empty(signs);
    }

    [Fact]
    public async Task ReadinessActUsesCommitteeLinkedThroughAssessingProgram()
    {
        var awp = new Awp();
        var organization = new Organization();
        var employee = new Employee { Fio = "Председатель комиссии периода" };
        var committee = CreateCommittee(awp, organization, employee);
        var program = new AwpAssessingProgram
        {
            Awp = awp,
            Organization = organization,
            Committee = committee,
        };
        var act = new RedinessAct
        {
            Awp = awp,
            Organization = organization,
            AwpAssessingProgram = program,
        };
        var repository = CreateRepository(committee, program);

        var signs = await new TestReadinessActCreated(repository)
            .CreateSigns(act);

        var sign = Assert.Single(signs);
        Assert.Equal(employee.Id, sign.Employee.Id);
    }

    [Fact]
    public async Task ReadinessActFallbackSearchIsScopedToItsHeatingPeriod()
    {
        var awp = new Awp();
        var anotherAwp = new Awp();
        var organization = new Organization();
        var wrongEmployee = new Employee { Fio = "Председатель другого периода" };
        var expectedEmployee = new Employee { Fio = "Председатель нужного периода" };
        var committees = new[]
        {
            CreateCommittee(anotherAwp, organization, wrongEmployee),
            CreateCommittee(awp, organization, expectedEmployee),
        };
        var act = new RedinessAct
        {
            Awp = awp,
            Organization = organization,
            AwpAssessingProgram = null!,
        };
        var repository = CreateRepository(committees: committees);

        var signs = await new TestReadinessActCreated(repository)
            .CreateSigns(act);

        var sign = Assert.Single(signs);
        Assert.Equal(expectedEmployee.Id, sign.Employee.Id);
    }

    [Fact]
    public async Task EvaluationSheetSearchSelectsCommitteeFromItsHeatingPeriod()
    {
        var awp = new Awp();
        var anotherAwp = new Awp();
        var organization = new Organization();
        var expectedEmployee = new Employee { Fio = "Член комиссии нужного периода" };
        var wrongCommittee = CreateCommittee(anotherAwp, organization, new Employee());
        var expectedCommittee = CreateCommittee(awp, organization, new Employee());
        AddSigner(expectedCommittee, expectedEmployee);
        var sheet = new EvaluationSheet
        {
            Awp = awp,
            Organization = organization,
        };
        var repository = CreateRepository(committees: [wrongCommittee, expectedCommittee]);

        var signs = await new TestEvaluationSheetCreated(repository).CreateSigns(sheet);

        var sign = Assert.Single(signs);
        Assert.Equal(expectedEmployee.Id, sign.Employee.Id);
    }

    [Fact]
    public async Task EvaluationSheetHierarchyFallbackSelectsCommitteeFromItsHeatingPeriod()
    {
        var awp = new Awp();
        var anotherAwp = new Awp();
        var hierarchy = new Organization();
        var organization = new Organization { Hierarchy = hierarchy };
        var expectedEmployee = new Employee { Fio = "Член комиссии нужного периода" };
        var wrongCommittee = CreateCommittee(anotherAwp, hierarchy, new Employee());
        var expectedCommittee = CreateCommittee(awp, hierarchy, new Employee());
        AddSigner(expectedCommittee, expectedEmployee);
        var sheet = new EvaluationSheet
        {
            Awp = awp,
            Organization = organization,
        };
        var repository = CreateRepository(committees: [wrongCommittee, expectedCommittee]);

        var signs = await new TestEvaluationSheetCreated(repository).CreateSigns(sheet);

        var sign = Assert.Single(signs);
        Assert.Equal(expectedEmployee.Id, sign.Employee.Id);
    }

    [Fact]
    public async Task LinkedCommitteeFromAnotherHeatingPeriodDoesNotProvideSigners()
    {
        var awp = new Awp();
        var anotherAwp = new Awp();
        var organization = new Organization();
        var employee = new Employee { Fio = "Председатель другого периода" };
        var committee = CreateCommittee(anotherAwp, organization, employee);
        var fallbackEmployee = new Employee { Fio = "Председатель периода" };
        var fallbackCommittee = CreateCommittee(awp, organization, fallbackEmployee);
        var program = new AwpAssessingProgram
        {
            Awp = awp,
            Organization = organization,
            Committee = committee,
        };
        var act = new RedinessAct
        {
            Awp = awp,
            Organization = organization,
            AwpAssessingProgram = program,
        };
        var repository = CreateRepository(
            committee,
            program,
            [committee, fallbackCommittee]);

        var signs = await new TestReadinessActCreated(repository)
            .CreateSigns(act);

        Assert.Empty(signs);
    }

    private static AdmissionCommittee CreateCommittee(
        Awp awp,
        Organization organization,
        Employee leader)
        => new()
        {
            Awp = awp,
            Organization = organization,
            Status = AdmissionCommitteeStatus.Approved,
            Leader = new AdmissionCommitteeEmployee { Employee = leader },
        };

    private static void AddSigner(AdmissionCommittee committee, Employee employee)
    {
        var leader = committee.Leader!;
        leader.Id = Guid.NewGuid();
        leader.AdmissionCommittee = committee;
        committee.AdmissionCommitteeEmployee.Add(leader);
        committee.AdmissionCommitteeEmployee.Add(new AdmissionCommitteeEmployee
        {
            Id = Guid.NewGuid(),
            AdmissionCommittee = committee,
            Employee = employee,
        });
    }

    private static IRepository CreateRepository(
        AdmissionCommittee? committee = null,
        AwpAssessingProgram? program = null,
        IReadOnlyCollection<AdmissionCommittee>? committees = null)
        => TestRepository.Create((method, args) => HandleRepositoryCall(
            method,
            args,
            committee,
            program,
            committees ?? []));

    private static object? HandleRepositoryCall(
        MethodInfo method,
        object?[]? args,
        AdmissionCommittee? committee,
        AwpAssessingProgram? program,
        IReadOnlyCollection<AdmissionCommittee> committees)
    {
        var entityType = method.IsGenericMethod
            ? method.GetGenericArguments()[0]
            : null;

        return method.Name switch
        {
            "GetById" when entityType == typeof(AdmissionCommittee)
                => Task.FromResult(committee),
            "GetById" when entityType == typeof(AwpAssessingProgram)
                => Task.FromResult(program),
            "GetById" when entityType == typeof(SignRole)
                => Task.FromResult<SignRole?>(new SignRole()),
            "MakeEntity" when entityType == typeof(AwpAssessingProgramSign)
                => new AwpAssessingProgramSign(),
            "MakeEntity" when entityType == typeof(ReadinessActSign)
                => new ReadinessActSign(),
            "MakeEntity" when entityType == typeof(EvaluationSheetSign)
                => new EvaluationSheetSign(),
            "FirstOrDefault" when entityType == typeof(AdmissionCommittee)
                => FindCommittee(args, committees),
            "FirstOrDefault" when entityType == typeof(PlaceOfEmployment)
                => Task.FromResult<PlaceOfEmployment?>(null),
            "Create" => null,
            _ => throw new NotSupportedException(method.Name),
        };
    }

    private static Task<AdmissionCommittee?> FindCommittee(
        object?[]? args,
        IReadOnlyCollection<AdmissionCommittee> committees)
    {
        var predicate = (Expression<Func<AdmissionCommittee, bool>>)args![0]!;
        var matches = committees.Where(predicate.Compile()).ToList();
        Assert.True(matches.Count <= 1, "The commission query must uniquely identify a committee.");
        return Task.FromResult(matches.FirstOrDefault());
    }

    private sealed class TestAssessingProgramCreated(IRepository repository)
        : OnAwpAssessingProgramCreated(repository)
    {
        public Task<List<AwpAssessingProgramSign>> CreateSigns(AwpAssessingProgram program)
            => base.CreateSigns(program);
    }

    private sealed class TestReadinessActCreated(IRepository repository)
        : OnRedinessActCreated(repository, null!)
    {
        public Task<List<ReadinessActSign>> CreateSigns(RedinessAct act)
            => base.CreateSigns(act);
    }

    private sealed class TestEvaluationSheetCreated(IRepository repository)
        : OnEvaluationSheetCreated(repository, null!)
    {
        public Task<List<EvaluationSheetSign>> CreateSigns(EvaluationSheet sheet)
            => base.CreateSigns(sheet);
    }
}
