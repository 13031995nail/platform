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

public sealed class ReadinessActAccessTests
{
    [Fact]
    public async Task GeneratedReadinessActDetailsAndSignersDoNotRequireStoredRows()
    {
        var fixture = new DocumentScopeFixture(WorkObjectRoleNames.Tso);
        fixture.Allow(fixture.Object);
        var act = new RedinessAct { Organization = fixture.Organization };
        var detail = new RedinessActObject { Act = act, WorkObject = fixture.Object };
        Assert.NotEqual(Guid.Empty, detail.Id);

        await fixture.ActAccess.EnsureCanChange(detail);
        await fixture.ActAccess.EnsureCanChange(new ReadinessActSign { Act = act });
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ReadinessActReassignmentChecksBothParentOrganizationScopes(bool oldParentForeign)
    {
        var fixture = new DocumentScopeFixture(WorkObjectRoleNames.Tso);
        fixture.Allow(fixture.Object);
        var foreign = fixture.AddObject(new Organization());
        var oldOrganization = oldParentForeign ? foreign.Organization : fixture.Organization;
        var newOrganization = oldParentForeign ? fixture.Organization : foreign.Organization;
        var detail = new RedinessActObject
        {
            Act = new RedinessAct { Organization = newOrganization }, WorkObject = fixture.Object,
        };
        fixture.Add(new RedinessActObject
        {
            Id = detail.Id, Act = new RedinessAct { Organization = oldOrganization }, WorkObject = fixture.Object,
        });
        DocumentActionScopeTestSupport.MarkAltered(detail);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => fixture.ActAccess.EnsureCanChange(detail));
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public async Task RepeatActActionPreservesSignedNotReadyCycle(bool signed, bool notReady)
    {
        var fixture = new DocumentScopeFixture(WorkObjectRoleNames.Tso);
        fixture.Allow(fixture.Object);
        var act = fixture.AddAct(fixture.Object);
        act.ActStatus = new ActStatus { Id = signed ? IActStatus.Keys.Signed : IActStatus.Keys.Editing };
        act.PartnerReadiness = notReady ? PartnerReadinessLevel.NotReady : PartnerReadinessLevel.Ready;

        var context = await fixture.Documents.GetContext(
            fixture.Object.Id, fixture.Organization.Id, fixture.Awp.Id, true);

        Assert.Equal(signed && notReady, context.Actions.CanCreateReadinessAct);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ActActionAndSaveRequireAllOrganizationObjects(bool allowAll)
    {
        var fixture = new DocumentScopeFixture(WorkObjectRoleNames.Tso);
        var second = fixture.AddObject(fixture.Organization);
        fixture.Allow(fixture.Object, allowAll ? second : fixture.Object);

        var context = await fixture.Documents.GetContext(
            fixture.Object.Id, fixture.Organization.Id, fixture.Awp.Id, true);

        Assert.Equal(allowAll, context.Actions.CanCreateReadinessAct);
        var act = new RedinessAct { Organization = fixture.Organization, Awp = fixture.Awp };
        act.SetStatus(ObjectStatus.Created);
        if (allowAll)
        {
            await fixture.ActAccess.EnsureCanChange(act);
        }
        else
        {
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                fixture.ActAccess.EnsureCanChange(act));
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ActSignChecksOldAndNewDocumentsWithOneDetailRead(bool allowAll)
    {
        var fixture = new DocumentScopeFixture(WorkObjectRoleNames.Tso);
        var oldObject = fixture.AddObject(new Organization());
        fixture.Allow(fixture.Object, allowAll ? oldObject : fixture.Object);
        var oldAct = fixture.AddAct(oldObject);
        var newAct = fixture.AddAct(fixture.Object);
        var sign = new ReadinessActSign { Act = newAct };
        DocumentActionScopeTestSupport.MarkAltered(sign);
        fixture.Add(new ReadinessActSign { Id = sign.Id, Act = oldAct });

        if (allowAll)
        {
            await fixture.ActAccess.EnsureCanChange(sign);
        }
        else
        {
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                fixture.ActAccess.EnsureCanChange(sign));
        }

        Assert.Equal(1, fixture.Reads.Count(type => type == typeof(RedinessActObject)));
    }
}
