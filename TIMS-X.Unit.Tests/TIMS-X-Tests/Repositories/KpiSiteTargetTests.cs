using System.Diagnostics;
using Moq;
using TIMS_X.BLL.Validation;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X_Tests.Repositories;

public class KpiSiteTargetTests : TestObjectBase<KpiSiteTarget>
{
    [Theory]
    [InlineData(OperationType.Add)]
    [InlineData(OperationType.Update)]
    public override async Task TestAddAndUpdate(OperationType operationType)
    {
        await base.TestAddAndUpdate(operationType);
    }

    [Theory]
    [InlineData(OperationType.AddNew, 1)]
    [InlineData(OperationType.Update, 1)]
    public async void ValidateSiteDoesNotExist_ReturnsError(OperationType operationType, int siteId)
    {
        // Arrange
        var kpiSiteTargets = new List<KpiSiteTarget>
        {
            new() { SiteId = 1, StartDate = DateTime.Today }
        };
        var kpiSiteTargetUoW = new Mock<IKpiSiteTargetUnitOfWork>();
        kpiSiteTargetUoW.Setup(x => x.GetKpiSiteTarget(It.IsAny<int>()))
            .Returns(Task.FromResult(kpiSiteTargets[0]));
        var siteUoW = new Mock<ISiteUnitOfWork>();
        var validator = new KpiSiteTargetValidator(siteUoW.Object, kpiSiteTargetUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new KpiSiteTarget
                { SiteId = siteId, StartDate = DateTime.Today }),
            OperationType.Update => await validator.Update(new KpiSiteTarget
                { SiteId = siteId, StartDate = DateTime.Today }),
            _ => null
        };

        // Assert
        Debug.Assert(validationResults != null, nameof(validationResults) + " != null");
        var validationResult = Assert.Single(validationResults);
        Assert.Equal("Site must exist", validationResult.Message);
        Assert.Equal(Severity.Error, validationResult.Severity);
    }

    [Theory]
    [InlineData(OperationType.AddNew, 1)]
    [InlineData(OperationType.Update, 1)]
    public async void ValidateSiteExists_ReturnsNoError(OperationType operationType, int siteId)
    {
        // Arrange
        var kpiSiteTargets = new List<KpiSiteTarget>
        {
            new() { SiteId = 1, StartDate = DateTime.Today }
        };
        var kpiSiteTargetUoW = new Mock<IKpiSiteTargetUnitOfWork>();
        var siteUoW = new Mock<ISiteUnitOfWork>();
        kpiSiteTargetUoW
            .Setup(x => x.GetKpiSiteTarget(It.IsAny<int>())).Returns(Task.FromResult(kpiSiteTargets[0]));
        siteUoW
            .Setup(x => x.GetSite(It.IsAny<int>())).Returns(Task.FromResult(new Site()));
        var validator = new KpiSiteTargetValidator(siteUoW.Object, kpiSiteTargetUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new KpiSiteTarget
                { SiteId = siteId, StartDate = DateTime.Today }),
            OperationType.Update => await validator.Update(new KpiSiteTarget
                { SiteId = siteId, StartDate = DateTime.Today }),
            _ => null
        };

        // Assert
        if (validationResults != null) Assert.Empty(validationResults);
    }
}