using System.Diagnostics;
using Moq;
using TIMS_X.BLL.Validation;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X_Tests.Repositories;

public class HistoryTests : TestObjectBase<History>
{
    [Theory]
    [InlineData(OperationType.Add)]
    [InlineData(OperationType.Update)]
    public override async Task TestAddAndUpdate(OperationType operationType)
    {
        await base.TestAddAndUpdate(operationType);
    }

    [Theory]
    [InlineData(OperationType.AddNew)]
    [InlineData(OperationType.Update)]
    public async void ValidatePatientExists_ReturnsError(OperationType operationType)
    {
        // Arrange
        var historyTypeUoW = new Mock<IHistoryTypeUnitOfWork>();
        var patientsUoW = new Mock<IPatientsUnitOfWork>();
        var appointmentsUoW = new Mock<IAppointmentsUnitOfWork>();
        var providersUoW = new Mock<IProvidersUnitOfWork>();
        var siteUoW = new Mock<ISiteUnitOfWork>();
        var marketingReferenceUoW = new Mock<IMarketingReferenceUnitOfWork>();
        historyTypeUoW
            .Setup(x => x.GetHistoryType(It.IsAny<int>())).Returns(Task.FromResult(new HistoryType()));
        appointmentsUoW.Setup(a => a.GetAppointment(It.IsAny<int>(), null)).Returns(Task.FromResult(new Appointment()));
        providersUoW
            .Setup(x => x.GetProvider(It.IsAny<int>(), null)).Returns(Task.FromResult(new Provider()));
        siteUoW
            .Setup(x => x.GetSite(It.IsAny<int>())).Returns(Task.FromResult(new Site()));
        marketingReferenceUoW
            .Setup(x => x.GetMarketingReference(It.IsAny<int>())).Returns(Task.FromResult(new MarketingReference()));
        var validator = new HistoryValidator(patientsUoW.Object, providersUoW.Object, historyTypeUoW.Object,
            siteUoW.Object, appointmentsUoW.Object, marketingReferenceUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new History()),
            OperationType.Update => await validator.Update(new History()),
            _ => null
        };

        // Assert
        Debug.Assert(validationResults != null, nameof(validationResults) + " != null");
        var validationResult = Assert.Single(validationResults);
        Assert.Equal("Patient must exist", validationResult.Message);
        Assert.Equal(Severity.Error, validationResult.Severity);
    }

    [Theory]
    [InlineData(OperationType.AddNew, 5)]
    [InlineData(OperationType.Update, 5)]
    public async void ValidateProviderExists_ReturnsError(OperationType operationType, int providerId)
    {
        // Arrange
        var historyTypeUoW = new Mock<IHistoryTypeUnitOfWork>();
        var patientsUoW = new Mock<IPatientsUnitOfWork>();
        var appointmentsUoW = new Mock<IAppointmentsUnitOfWork>();
        var providersUoW = new Mock<IProvidersUnitOfWork>();
        var siteUoW = new Mock<ISiteUnitOfWork>();
        var marketingReferenceUoW = new Mock<IMarketingReferenceUnitOfWork>();
        historyTypeUoW
            .Setup(x => x.GetHistoryType(It.IsAny<int>())).Returns(Task.FromResult(new HistoryType()));
        appointmentsUoW.Setup(a => a.GetAppointment(It.IsAny<int>(), null)).Returns(Task.FromResult(new Appointment()));
        patientsUoW
            .Setup(x => x.GetPatient(It.IsAny<long>(), null)).Returns(Task.FromResult(new Patient()));
        siteUoW
            .Setup(x => x.GetSite(It.IsAny<int>())).Returns(Task.FromResult(new Site()));
        marketingReferenceUoW
            .Setup(x => x.GetMarketingReference(It.IsAny<int>())).Returns(Task.FromResult(new MarketingReference()));
        var validator = new HistoryValidator(patientsUoW.Object, providersUoW.Object, historyTypeUoW.Object,
            siteUoW.Object, appointmentsUoW.Object, marketingReferenceUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new History { ProviderId = providerId }),
            OperationType.Update => await validator.Update(new History { ProviderId = providerId }),
            _ => null
        };

        // Assert
        Debug.Assert(validationResults != null, nameof(validationResults) + " != null");
        var validationResult = Assert.Single(validationResults);
        Assert.Equal("Provider must exist", validationResult.Message);
        Assert.Equal(Severity.Error, validationResult.Severity);
    }

    [Theory]
    [InlineData(OperationType.AddNew)]
    [InlineData(OperationType.Update)]
    public async void ValidateHistoryTypeExists_ReturnsError(OperationType operationType)
    {
        // Arrange
        var historyTypeUoW = new Mock<IHistoryTypeUnitOfWork>();
        var patientsUoW = new Mock<IPatientsUnitOfWork>();
        var appointmentsUoW = new Mock<IAppointmentsUnitOfWork>();
        var providersUoW = new Mock<IProvidersUnitOfWork>();
        var siteUoW = new Mock<ISiteUnitOfWork>();
        var marketingReferenceUoW = new Mock<IMarketingReferenceUnitOfWork>();
        providersUoW
            .Setup(x => x.GetProvider(It.IsAny<int>(), null)).Returns(Task.FromResult(new Provider()));
        appointmentsUoW.Setup(a => a.GetAppointment(It.IsAny<int>(), null)).Returns(Task.FromResult(new Appointment()));
        patientsUoW
            .Setup(x => x.GetPatient(It.IsAny<long>(), null)).Returns(Task.FromResult(new Patient()));
        siteUoW
            .Setup(x => x.GetSite(It.IsAny<int>())).Returns(Task.FromResult(new Site()));
        marketingReferenceUoW
            .Setup(x => x.GetMarketingReference(It.IsAny<int>())).Returns(Task.FromResult(new MarketingReference()));
        var validator = new HistoryValidator(patientsUoW.Object, providersUoW.Object, historyTypeUoW.Object,
            siteUoW.Object, appointmentsUoW.Object, marketingReferenceUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new History()),
            OperationType.Update => await validator.Update(new History()),
            _ => null
        };

        // Assert
        Debug.Assert(validationResults != null, nameof(validationResults) + " != null");
        var validationResult = Assert.Single(validationResults);
        Assert.Equal("HistoryType must exist", validationResult.Message);
        Assert.Equal(Severity.Error, validationResult.Severity);
    }

    [Theory]
    [InlineData(OperationType.AddNew, 5)]
    [InlineData(OperationType.Update, 5)]
    public async void ValidateAppointmentExists_ReturnsError(OperationType operationType, int appointmentId)
    {
        // Arrange
        var historyTypeUoW = new Mock<IHistoryTypeUnitOfWork>();
        var patientsUoW = new Mock<IPatientsUnitOfWork>();
        var appointmentsUoW = new Mock<IAppointmentsUnitOfWork>();
        var providersUoW = new Mock<IProvidersUnitOfWork>();
        var siteUoW = new Mock<ISiteUnitOfWork>();
        var marketingReferenceUoW = new Mock<IMarketingReferenceUnitOfWork>();
        providersUoW
            .Setup(x => x.GetProvider(It.IsAny<int>(), null)).Returns(Task.FromResult(new Provider()));
        historyTypeUoW.Setup(a => a.GetHistoryType(It.IsAny<int>())).Returns(Task.FromResult(new HistoryType()));
        patientsUoW
            .Setup(x => x.GetPatient(It.IsAny<long>(), null)).Returns(Task.FromResult(new Patient()));
        siteUoW
            .Setup(x => x.GetSite(It.IsAny<int>())).Returns(Task.FromResult(new Site()));
        marketingReferenceUoW
            .Setup(x => x.GetMarketingReference(It.IsAny<int>())).Returns(Task.FromResult(new MarketingReference()));
        var validator = new HistoryValidator(patientsUoW.Object, providersUoW.Object, historyTypeUoW.Object,
            siteUoW.Object, appointmentsUoW.Object, marketingReferenceUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new History { AppointmentId = appointmentId }),
            OperationType.Update => await validator.Update(new History { AppointmentId = appointmentId }),
            _ => null
        };

        // Assert
        Debug.Assert(validationResults != null, nameof(validationResults) + " != null");
        var validationResult = Assert.Single(validationResults);
        Assert.Equal("Appointment must exist", validationResult.Message);
        Assert.Equal(Severity.Error, validationResult.Severity);
    }

    [Theory]
    [InlineData(OperationType.AddNew, 5)]
    [InlineData(OperationType.Update, 5)]
    public async void ValidateReferralSourceExists_ReturnsError(OperationType operationType, int referralSourceId)
    {
        // Arrange
        var historyTypeUoW = new Mock<IHistoryTypeUnitOfWork>();
        var patientsUoW = new Mock<IPatientsUnitOfWork>();
        var appointmentsUoW = new Mock<IAppointmentsUnitOfWork>();
        var providersUoW = new Mock<IProvidersUnitOfWork>();
        var siteUoW = new Mock<ISiteUnitOfWork>();
        var marketingReferenceUoW = new Mock<IMarketingReferenceUnitOfWork>();
        providersUoW
            .Setup(x => x.GetProvider(It.IsAny<int>(), null)).Returns(Task.FromResult(new Provider()));
        historyTypeUoW.Setup(a => a.GetHistoryType(It.IsAny<int>())).Returns(Task.FromResult(new HistoryType()));
        patientsUoW
            .Setup(x => x.GetPatient(It.IsAny<long>(), null)).Returns(Task.FromResult(new Patient()));
        siteUoW
            .Setup(x => x.GetSite(It.IsAny<int>())).Returns(Task.FromResult(new Site()));
        appointmentsUoW
            .Setup(x => x.GetAppointment(It.IsAny<int>(), null)).Returns(Task.FromResult(new Appointment()));
        var validator = new HistoryValidator(patientsUoW.Object, providersUoW.Object, historyTypeUoW.Object,
            siteUoW.Object, appointmentsUoW.Object, marketingReferenceUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new History { ReferralSourceId = referralSourceId }),
            OperationType.Update => await validator.Update(new History { ReferralSourceId = referralSourceId }),
            _ => null
        };

        // Assert
        Debug.Assert(validationResults != null, nameof(validationResults) + " != null");
        var validationResult = Assert.Single(validationResults);
        Assert.Equal("ReferralSource must be valid", validationResult.Message);
        Assert.Equal(Severity.Error, validationResult.Severity);
    }

    [Theory]
    [InlineData(OperationType.AddNew, 5)]
    [InlineData(OperationType.Update, 5)]
    public async void ValidateSiteExists_ReturnsError(OperationType operationType, int siteId)
    {
        // Arrange
        var historyTypeUoW = new Mock<IHistoryTypeUnitOfWork>();
        var patientsUoW = new Mock<IPatientsUnitOfWork>();
        var appointmentsUoW = new Mock<IAppointmentsUnitOfWork>();
        var providersUoW = new Mock<IProvidersUnitOfWork>();
        var siteUoW = new Mock<ISiteUnitOfWork>();
        var marketingReferenceUoW = new Mock<IMarketingReferenceUnitOfWork>();
        providersUoW
            .Setup(x => x.GetProvider(It.IsAny<int>(), null)).Returns(Task.FromResult(new Provider()));
        historyTypeUoW.Setup(a => a.GetHistoryType(It.IsAny<int>())).Returns(Task.FromResult(new HistoryType()));
        patientsUoW
            .Setup(x => x.GetPatient(It.IsAny<long>(), null)).Returns(Task.FromResult(new Patient()));
        marketingReferenceUoW
            .Setup(x => x.GetMarketingReference(It.IsAny<int>())).Returns(Task.FromResult(new MarketingReference()));
        appointmentsUoW
            .Setup(x => x.GetAppointment(It.IsAny<int>(), null)).Returns(Task.FromResult(new Appointment()));
        var validator = new HistoryValidator(patientsUoW.Object, providersUoW.Object, historyTypeUoW.Object,
            siteUoW.Object, appointmentsUoW.Object, marketingReferenceUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new History { SyncSiteId = siteId }),
            OperationType.Update => await validator.Update(new History { SyncSiteId = siteId }),
            _ => null
        };

        // Assert
        Debug.Assert(validationResults != null, nameof(validationResults) + " != null");
        var validationResult = Assert.Single(validationResults);
        Assert.Equal("Site must exist", validationResult.Message);
        Assert.Equal(Severity.Error, validationResult.Severity);
    }

    [Theory]
    [InlineData(OperationType.AddNew)]
    [InlineData(OperationType.Update)]
    public async void ValidateEverythingExists_ReturnsNoError(OperationType operationType)
    {
        // Arrange
        var historyTypeUoW = new Mock<IHistoryTypeUnitOfWork>();
        var patientsUoW = new Mock<IPatientsUnitOfWork>();
        var appointmentsUoW = new Mock<IAppointmentsUnitOfWork>();
        var providersUoW = new Mock<IProvidersUnitOfWork>();
        var siteUoW = new Mock<ISiteUnitOfWork>();
        var marketingReferenceUoW = new Mock<IMarketingReferenceUnitOfWork>();
        providersUoW
            .Setup(x => x.GetProvider(It.IsAny<int>(), null)).Returns(Task.FromResult(new Provider()));
        historyTypeUoW.Setup(a => a.GetHistoryType(It.IsAny<int>())).Returns(Task.FromResult(new HistoryType()));
        patientsUoW
            .Setup(x => x.GetPatient(It.IsAny<long>(), null)).Returns(Task.FromResult(new Patient()));
        marketingReferenceUoW
            .Setup(x => x.GetMarketingReference(It.IsAny<int>())).Returns(Task.FromResult(new MarketingReference()));
        appointmentsUoW
            .Setup(x => x.GetAppointment(It.IsAny<int>(), null)).Returns(Task.FromResult(new Appointment()));
        siteUoW.Setup(s => s.GetSite(It.IsAny<int>())).Returns(Task.FromResult(new Site()));
        var validator = new HistoryValidator(patientsUoW.Object, providersUoW.Object, historyTypeUoW.Object,
            siteUoW.Object, appointmentsUoW.Object, marketingReferenceUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new History()),
            OperationType.Update => await validator.Update(new History()),
            _ => null
        };

        // Assert
        if (validationResults != null) Assert.Empty(validationResults);
    }
}