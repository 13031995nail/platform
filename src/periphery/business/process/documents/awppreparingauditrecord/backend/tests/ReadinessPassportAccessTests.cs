namespace Tmobis.Business.Process.Documents.Tests;

using ICSSoft.STORMNET;
using ICSSoft.STORMNET.FunctionalLanguage;
using Tmobis.Business.Common.Address;
using Tmobis.Business.Common.Contacts;
using Tmobis.Business.Common.Resources;
using Tmobis.Business.Core.Providers.Roles;
using Tmobis.Business.Process.Documents.Flexberry;
using Tmobis.Business.Process.Documents.Providers;
using Tmobis.Business.Process.Documents.Base;
using Tmobis.Core.Models;
using Xunit;
using UnauthorizedAccessException = System.UnauthorizedAccessException;

public sealed class ReadinessPassportAccessTests
{
    [Fact]
    public async Task GeneratedReadinessPassportDetailsAndSignersDoNotRequireStoredRows()
    {
        var fixture = new DocumentScopeFixture(WorkObjectRoleNames.Administration);
        fixture.Allow(fixture.Object);
        var act = new RedinessAct { Organization = fixture.Organization };
        var passport = new AwpReadinessPassport { Organization = fixture.Organization, ReadinessAct = act };

        await fixture.PassportAccess.EnsureCanChange(new AwpReadinessPassportObject
        {
            Act = passport, WorkObject = fixture.Object,
        });
        await fixture.PassportAccess.EnsureCanChange(new AwpReadinessPassportSign { Act = passport });
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReadinessPassportReassignmentChecksBothParentOrganizationScopes(bool oldParentForeign)
    {
        var fixture = new DocumentScopeFixture(WorkObjectRoleNames.Administration);
        fixture.Allow(fixture.Object);
        var foreign = fixture.AddObject(new Organization());
        var oldOrganization = oldParentForeign ? foreign.Organization : fixture.Organization;
        var newOrganization = oldParentForeign ? fixture.Organization : foreign.Organization;
        var detail = new AwpReadinessPassportObject
        {
            Act = new AwpReadinessPassport
            {
                Organization = newOrganization, ReadinessAct = new RedinessAct(),
            },
            WorkObject = fixture.Object,
        };
        fixture.Add(new AwpReadinessPassportObject
        {
            Id = detail.Id,
            Act = new AwpReadinessPassport
            {
                Organization = oldOrganization, ReadinessAct = new RedinessAct(),
            },
            WorkObject = fixture.Object,
        });
        DocumentActionScopeTestSupport.MarkAltered(detail);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => fixture.PassportAccess.EnsureCanChange(detail));
    }

    [Fact]
    public async Task PassportPatchCannotUseUnsignedReadinessAct()
    {
        var fixture = new DocumentScopeFixture(WorkObjectRoleNames.Administration);
        var signedAct = fixture.Add(new RedinessAct
        {
            Awp = fixture.Awp,
            Organization = fixture.Organization,
            ActStatus = new ActStatus { Id = IActStatus.Keys.Signed },
        });
        var unsignedAct = fixture.Add(new RedinessAct
        {
            Awp = fixture.Awp,
            Organization = fixture.Organization,
            ActStatus = new ActStatus { Id = IActStatus.Keys.Editing },
        });
        var passport = DocumentActionScopeTestSupport.CreatePassportPatch(fixture, unsignedAct, storedReadinessAct: signedAct);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            fixture.PassportAccess.EnsureCanChange(passport));
    }

    [Fact]
    public async Task PassportPatchCannotUseReadinessActFromAnotherAwp()
    {
        var fixture = new DocumentScopeFixture(WorkObjectRoleNames.Administration);
        var signedAct = fixture.Add(new RedinessAct
        {
            Awp = fixture.Awp,
            Organization = fixture.Organization,
            ActStatus = new ActStatus { Id = IActStatus.Keys.Signed },
        });
        var passport = DocumentActionScopeTestSupport.CreatePassportPatch(fixture, signedAct, awp: new Awp());

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            fixture.PassportAccess.EnsureCanChange(passport));
    }

    [Fact]
    public async Task PassportPatchCannotUseReadinessActFromAnotherOrganization()
    {
        var fixture = new DocumentScopeFixture(WorkObjectRoleNames.Administration);
        var signedAct = fixture.Add(new RedinessAct
        {
            Awp = fixture.Awp,
            Organization = fixture.Organization,
            ActStatus = new ActStatus { Id = IActStatus.Keys.Signed },
        });
        var passport = DocumentActionScopeTestSupport.CreatePassportPatch(fixture, signedAct, organization: new Organization());

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            fixture.PassportAccess.EnsureCanChange(passport));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task PassportActionRequiresSignedReadinessAct(bool signed)
    {
        var fixture = new DocumentScopeFixture(WorkObjectRoleNames.Administration);
        fixture.Allow(fixture.Object);
        var act = fixture.AddAct(fixture.Object);
        act.ActStatus = new ActStatus { Id = signed ? IActStatus.Keys.Signed : IActStatus.Keys.Editing };

        var context = await fixture.Documents.GetContext(
            fixture.Object.Id, fixture.Organization.Id, fixture.Awp.Id, true);

        Assert.Equal(signed, context.Actions.CanCreateReadinessPassport);
    }

    [Fact]
    public async Task CannotCreatePassportFromUnsignedAct()
    {
        var fixture = new DocumentScopeFixture(WorkObjectRoleNames.Administration);
        fixture.Allow(fixture.Object);
        var act = fixture.AddAct(fixture.Object);
        act.ActStatus = new ActStatus { Id = IActStatus.Keys.Editing };

        var passport = new AwpReadinessPassport
        {
            Organization = fixture.Organization,
            Awp = fixture.Awp,
            ReadinessAct = act,
        };
        passport.SetStatus(ObjectStatus.Created);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            fixture.PassportAccess.EnsureCanChange(passport));
    }

    [Fact]
    public async Task CannotCreatePassportFromActOfAnotherAwp()
    {
        var fixture = new DocumentScopeFixture(WorkObjectRoleNames.Administration);
        fixture.Allow(fixture.Object);
        var act = fixture.AddAct(fixture.Object);
        act.ActStatus = new ActStatus { Id = IActStatus.Keys.Signed };

        var passport = new AwpReadinessPassport
        {
            Organization = fixture.Organization,
            Awp = new Awp { Id = Guid.NewGuid() },
            ReadinessAct = act,
        };
        passport.SetStatus(ObjectStatus.Created);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            fixture.PassportAccess.EnsureCanChange(passport));
    }

    [Fact]
    public async Task CannotCreatePassportFromActOfAnotherOrganization()
    {
        var fixture = new DocumentScopeFixture(WorkObjectRoleNames.Administration);
        fixture.Allow(fixture.Object);
        var act = fixture.AddAct(fixture.Object);
        act.ActStatus = new ActStatus { Id = IActStatus.Keys.Signed };

        var passport = new AwpReadinessPassport
        {
            Organization = new Organization { Id = Guid.NewGuid() },
            Awp = fixture.Awp,
            ReadinessAct = act,
        };
        passport.SetStatus(ObjectStatus.Created);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            fixture.PassportAccess.EnsureCanChange(passport));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task PassportActionAndSaveRequireAllOrganizationObjects(bool allowAll)
    {
        var fixture = new DocumentScopeFixture(WorkObjectRoleNames.Administration);
        var second = fixture.AddObject(fixture.Organization);
        fixture.Allow(fixture.Object, allowAll ? second : fixture.Object);
        var act = fixture.AddAct(fixture.Object);

        act.ActStatus = new ActStatus { Id = IActStatus.Keys.Signed };

        var context = await fixture.Documents.GetContext(
            fixture.Object.Id, fixture.Organization.Id, fixture.Awp.Id, true);

        Assert.Equal(allowAll, context.Actions.CanCreateReadinessPassport);
        var passport = new AwpReadinessPassport
        {
            Organization = fixture.Organization,
            Awp = fixture.Awp,
            ReadinessAct = act,
        };
        passport.SetStatus(ObjectStatus.Created);
        if (allowAll)
        {
            await fixture.PassportAccess.EnsureCanChange(passport);
        }
        else
        {
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                fixture.PassportAccess.EnsureCanChange(passport));
        }
    }

    [Fact]
    public async Task PassportCreationAlsoChecksObjectsLinkedToReadinessAct()
    {
        var fixture = new DocumentScopeFixture(WorkObjectRoleNames.Administration);
        fixture.Allow(fixture.Object);
        var act = fixture.AddAct(fixture.Object);
        act.ActStatus = new ActStatus { Id = IActStatus.Keys.Signed };
        fixture.Add(new RedinessActObject
        {
            Act = act,
            WorkObject = fixture.AddObject(new Organization()),
        });

        Assert.False(await fixture.PassportAccess.CanCreate(fixture.Organization.Id, act.Id));
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            fixture.PassportAccess.EnsureCanChange(new AwpReadinessPassport
            {
                Organization = fixture.Organization,
                Awp = fixture.Awp,
                ReadinessAct = act,
            }));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task PassportSignChecksOldAndNewDocumentsWithOneDetailRead(bool allowAll)
    {
        var fixture = new DocumentScopeFixture(WorkObjectRoleNames.Administration);
        var oldObject = fixture.AddObject(new Organization());
        fixture.Allow(fixture.Object, allowAll ? oldObject : fixture.Object);
        var oldPassport = new AwpReadinessPassport();
        var newPassport = new AwpReadinessPassport();
        fixture.Add(new AwpReadinessPassportObject { Act = oldPassport, WorkObject = oldObject });
        fixture.Add(new AwpReadinessPassportObject { Act = newPassport, WorkObject = fixture.Object });
        var sign = new AwpReadinessPassportSign { Act = newPassport };
        DocumentActionScopeTestSupport.MarkAltered(sign);
        fixture.Add(new AwpReadinessPassportSign { Id = sign.Id, Act = oldPassport });

        if (allowAll)
        {
            await fixture.PassportAccess.EnsureCanChange(sign);
        }
        else
        {
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                fixture.PassportAccess.EnsureCanChange(sign));
        }

        Assert.Equal(1, fixture.Reads.Count(type => type == typeof(AwpReadinessPassportObject)));
    }
}
