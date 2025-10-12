using System.Diagnostics;
using Moq;
using TIMS_X.BLL.Validation;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X_Tests.Repositories;

public class HaHaHistoryTests : TestObjectBase<HaHistory>
{
    [Theory]
    [InlineData(OperationType.Add)]
    [InlineData(OperationType.Update)]
    public override async Task TestAddAndUpdate(OperationType operationType)
    {
        await base.TestAddAndUpdate(operationType);
    }

    [Theory]
    [InlineData(OperationType.AddNew, 5)]
    [InlineData(OperationType.Update, 5)]
    public async void ValidateBatterySizeExists_ReturnsError(OperationType operationType, int batterySizeId)
    {
        // Arrange
        var batterySizeUoW = new Mock<IBatterySizeUnitOfWork>();
        var appointmentsUoW = new Mock<IAppointmentsUnitOfWork>();
        var haModelUoW = new Mock<IHaModelUnitOfWork>();
        var haStatusUoW = new Mock<IHaStatusUnitOfWork>();
        var haStyleUoW = new Mock<IHaStyleUnitOfWork>();
        var haWarrantyType = new Mock<IHaWarrantyTypeUnitOfWork>();
        var patientsUoW = new Mock<IPatientsUnitOfWork>();
        var providersUoW = new Mock<IProvidersUnitOfWork>();
        var siteUoW = new Mock<ISiteUnitOfWork>();
        appointmentsUoW.Setup(a => a.GetAppointment(It.IsAny<int>(), null)).Returns(Task.FromResult(new Appointment()));
        haModelUoW.Setup(h => h.GetHaModel(It.IsAny<int>(), null)).Returns(Task.FromResult(new HaModel()));
        haStatusUoW.Setup(h => h.GetHaStatus(It.IsAny<int>())).Returns(Task.FromResult(new HaStatus()));
        haStyleUoW.Setup(h => h.GetHaStyle(It.IsAny<int>())).Returns(Task.FromResult(new HaStyle()));
        haWarrantyType.Setup(h => h.GetHaWarrantyType(It.IsAny<int>())).Returns(Task.FromResult(new HaWarrantyType()));
        patientsUoW.Setup(p => p.GetPatient(It.IsAny<long>(), null)).Returns(Task.FromResult(new Patient()));
        providersUoW
            .Setup(x => x.GetProvider(It.IsAny<int>(), null)).Returns(Task.FromResult(new Provider()));
        siteUoW
            .Setup(x => x.GetSite(It.IsAny<int>())).Returns(Task.FromResult(new Site()));
        var validator = new HaHistoryValidator(batterySizeUoW.Object, patientsUoW.Object, providersUoW.Object,
            siteUoW.Object, haModelUoW.Object, haWarrantyType.Object, appointmentsUoW.Object, haStyleUoW.Object,
            haStatusUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new HaHistory { BatterySizeId = batterySizeId }),
            OperationType.Update => await validator.Update(new HaHistory { BatterySizeId = batterySizeId }),
            _ => null
        };

        // Assert
        Debug.Assert(validationResults != null, nameof(validationResults) + " != null");
        var validationResult = Assert.Single(validationResults);
        Assert.Equal("BatterySize must exist", validationResult.Message);
        Assert.Equal(Severity.Error, validationResult.Severity);
    }

    [Theory]
    [InlineData(OperationType.AddNew, 5)]
    [InlineData(OperationType.Update, 5)]
    public async void ValidateAppointmentExists_ReturnsError(OperationType operationType, int appointmentId)
    {
        // Arrange
        var batterySizeUoW = new Mock<IBatterySizeUnitOfWork>();
        var appointmentsUoW = new Mock<IAppointmentsUnitOfWork>();
        var haModelUoW = new Mock<IHaModelUnitOfWork>();
        var haStatusUoW = new Mock<IHaStatusUnitOfWork>();
        var haStyleUoW = new Mock<IHaStyleUnitOfWork>();
        var haWarrantyType = new Mock<IHaWarrantyTypeUnitOfWork>();
        var patientsUoW = new Mock<IPatientsUnitOfWork>();
        var providersUoW = new Mock<IProvidersUnitOfWork>();
        var siteUoW = new Mock<ISiteUnitOfWork>();
        batterySizeUoW.Setup(a => a.GetBatterySize(It.IsAny<int>())).Returns(Task.FromResult(new BatterySize()));
        haModelUoW.Setup(h => h.GetHaModel(It.IsAny<int>(), null)).Returns(Task.FromResult(new HaModel()));
        haStatusUoW.Setup(h => h.GetHaStatus(It.IsAny<int>())).Returns(Task.FromResult(new HaStatus()));
        haStyleUoW.Setup(h => h.GetHaStyle(It.IsAny<int>())).Returns(Task.FromResult(new HaStyle()));
        haWarrantyType.Setup(h => h.GetHaWarrantyType(It.IsAny<int>())).Returns(Task.FromResult(new HaWarrantyType()));
        patientsUoW.Setup(p => p.GetPatient(It.IsAny<long>(), null)).Returns(Task.FromResult(new Patient()));
        providersUoW
            .Setup(x => x.GetProvider(It.IsAny<int>(), null)).Returns(Task.FromResult(new Provider()));
        siteUoW
            .Setup(x => x.GetSite(It.IsAny<int>())).Returns(Task.FromResult(new Site()));
        var validator = new HaHistoryValidator(batterySizeUoW.Object, patientsUoW.Object, providersUoW.Object,
            siteUoW.Object, haModelUoW.Object, haWarrantyType.Object, appointmentsUoW.Object, haStyleUoW.Object,
            haStatusUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new HaHistory { AppointmentId = appointmentId }),
            OperationType.Update => await validator.Update(new HaHistory { AppointmentId = appointmentId }),
            _ => null
        };

        // Assert
        Debug.Assert(validationResults != null, nameof(validationResults) + " != null");
        var validationResult = Assert.Single(validationResults);
        Assert.Equal("Appointment must exist", validationResult.Message);
        Assert.Equal(Severity.Error, validationResult.Severity);
    }

    [Theory]
    [InlineData(OperationType.AddNew)]
    [InlineData(OperationType.Update)]
    public async void ValidatePatientExists_ReturnsError(OperationType operationType)
    {
        // Arrange
        var batterySizeUoW = new Mock<IBatterySizeUnitOfWork>();
        var appointmentsUoW = new Mock<IAppointmentsUnitOfWork>();
        var haModelUoW = new Mock<IHaModelUnitOfWork>();
        var haStatusUoW = new Mock<IHaStatusUnitOfWork>();
        var haStyleUoW = new Mock<IHaStyleUnitOfWork>();
        var haWarrantyType = new Mock<IHaWarrantyTypeUnitOfWork>();
        var patientsUoW = new Mock<IPatientsUnitOfWork>();
        var providersUoW = new Mock<IProvidersUnitOfWork>();
        var siteUoW = new Mock<ISiteUnitOfWork>();
        batterySizeUoW.Setup(a => a.GetBatterySize(It.IsAny<int>())).Returns(Task.FromResult(new BatterySize()));
        haModelUoW.Setup(h => h.GetHaModel(It.IsAny<int>(), null)).Returns(Task.FromResult(new HaModel()));
        haStatusUoW.Setup(h => h.GetHaStatus(It.IsAny<int>())).Returns(Task.FromResult(new HaStatus()));
        haStyleUoW.Setup(h => h.GetHaStyle(It.IsAny<int>())).Returns(Task.FromResult(new HaStyle()));
        haWarrantyType.Setup(h => h.GetHaWarrantyType(It.IsAny<int>())).Returns(Task.FromResult(new HaWarrantyType()));
        appointmentsUoW.Setup(p => p.GetAppointment(It.IsAny<int>(), null)).Returns(Task.FromResult(new Appointment()));
        providersUoW
            .Setup(x => x.GetProvider(It.IsAny<int>(), null)).Returns(Task.FromResult(new Provider()));
        siteUoW
            .Setup(x => x.GetSite(It.IsAny<int>())).Returns(Task.FromResult(new Site()));
        var validator = new HaHistoryValidator(batterySizeUoW.Object, patientsUoW.Object, providersUoW.Object,
            siteUoW.Object, haModelUoW.Object, haWarrantyType.Object, appointmentsUoW.Object, haStyleUoW.Object,
            haStatusUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new HaHistory()),
            OperationType.Update => await validator.Update(new HaHistory()),
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
        var batterySizeUoW = new Mock<IBatterySizeUnitOfWork>();
        var appointmentsUoW = new Mock<IAppointmentsUnitOfWork>();
        var haModelUoW = new Mock<IHaModelUnitOfWork>();
        var haStatusUoW = new Mock<IHaStatusUnitOfWork>();
        var haStyleUoW = new Mock<IHaStyleUnitOfWork>();
        var haWarrantyType = new Mock<IHaWarrantyTypeUnitOfWork>();
        var patientsUoW = new Mock<IPatientsUnitOfWork>();
        var providersUoW = new Mock<IProvidersUnitOfWork>();
        var siteUoW = new Mock<ISiteUnitOfWork>();
        batterySizeUoW.Setup(a => a.GetBatterySize(It.IsAny<int>())).Returns(Task.FromResult(new BatterySize()));
        haModelUoW.Setup(h => h.GetHaModel(It.IsAny<int>(), null)).Returns(Task.FromResult(new HaModel()));
        haStatusUoW.Setup(h => h.GetHaStatus(It.IsAny<int>())).Returns(Task.FromResult(new HaStatus()));
        haStyleUoW.Setup(h => h.GetHaStyle(It.IsAny<int>())).Returns(Task.FromResult(new HaStyle()));
        haWarrantyType.Setup(h => h.GetHaWarrantyType(It.IsAny<int>())).Returns(Task.FromResult(new HaWarrantyType()));
        appointmentsUoW.Setup(p => p.GetAppointment(It.IsAny<int>(), null)).Returns(Task.FromResult(new Appointment()));
        patientsUoW
            .Setup(x => x.GetPatient(It.IsAny<long>(), null)).Returns(Task.FromResult(new Patient()));
        siteUoW
            .Setup(x => x.GetSite(It.IsAny<int>())).Returns(Task.FromResult(new Site()));
        var validator = new HaHistoryValidator(batterySizeUoW.Object, patientsUoW.Object, providersUoW.Object,
            siteUoW.Object, haModelUoW.Object, haWarrantyType.Object, appointmentsUoW.Object, haStyleUoW.Object,
            haStatusUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new HaHistory { ProviderId = providerId }),
            OperationType.Update => await validator.Update(new HaHistory { ProviderId = providerId }),
            _ => null
        };

        // Assert
        Debug.Assert(validationResults != null, nameof(validationResults) + " != null");
        var validationResult = Assert.Single(validationResults);
        Assert.Equal("Provider must exist", validationResult.Message);
        Assert.Equal(Severity.Error, validationResult.Severity);
    }

    [Theory]
    [InlineData(OperationType.AddNew, 5)]
    [InlineData(OperationType.Update, 5)]
    public async void ValidateSiteExists_ReturnsError(OperationType operationType, int siteId)
    {
        // Arrange
        var batterySizeUoW = new Mock<IBatterySizeUnitOfWork>();
        var appointmentsUoW = new Mock<IAppointmentsUnitOfWork>();
        var haModelUoW = new Mock<IHaModelUnitOfWork>();
        var haStatusUoW = new Mock<IHaStatusUnitOfWork>();
        var haStyleUoW = new Mock<IHaStyleUnitOfWork>();
        var haWarrantyType = new Mock<IHaWarrantyTypeUnitOfWork>();
        var patientsUoW = new Mock<IPatientsUnitOfWork>();
        var providersUoW = new Mock<IProvidersUnitOfWork>();
        var siteUoW = new Mock<ISiteUnitOfWork>();
        batterySizeUoW.Setup(a => a.GetBatterySize(It.IsAny<int>())).Returns(Task.FromResult(new BatterySize()));
        haModelUoW.Setup(h => h.GetHaModel(It.IsAny<int>(), null)).Returns(Task.FromResult(new HaModel()));
        haStatusUoW.Setup(h => h.GetHaStatus(It.IsAny<int>())).Returns(Task.FromResult(new HaStatus()));
        haStyleUoW.Setup(h => h.GetHaStyle(It.IsAny<int>())).Returns(Task.FromResult(new HaStyle()));
        haWarrantyType.Setup(h => h.GetHaWarrantyType(It.IsAny<int>())).Returns(Task.FromResult(new HaWarrantyType()));
        appointmentsUoW.Setup(p => p.GetAppointment(It.IsAny<int>(), null)).Returns(Task.FromResult(new Appointment()));
        patientsUoW
            .Setup(x => x.GetPatient(It.IsAny<long>(), null)).Returns(Task.FromResult(new Patient()));
        providersUoW
            .Setup(x => x.GetProvider(It.IsAny<int>(), null)).Returns(Task.FromResult(new Provider()));
        var validator = new HaHistoryValidator(batterySizeUoW.Object, patientsUoW.Object, providersUoW.Object,
            siteUoW.Object, haModelUoW.Object, haWarrantyType.Object, appointmentsUoW.Object, haStyleUoW.Object,
            haStatusUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new HaHistory { SyncSiteId = siteId }),
            OperationType.Update => await validator.Update(new HaHistory { SyncSiteId = siteId }),
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
    public async void ValidateHaModelExists_ReturnsError(OperationType operationType)
    {
        // Arrange
        var batterySizeUoW = new Mock<IBatterySizeUnitOfWork>();
        var appointmentsUoW = new Mock<IAppointmentsUnitOfWork>();
        var haModelUoW = new Mock<IHaModelUnitOfWork>();
        var haStatusUoW = new Mock<IHaStatusUnitOfWork>();
        var haStyleUoW = new Mock<IHaStyleUnitOfWork>();
        var haWarrantyType = new Mock<IHaWarrantyTypeUnitOfWork>();
        var patientsUoW = new Mock<IPatientsUnitOfWork>();
        var providersUoW = new Mock<IProvidersUnitOfWork>();
        var siteUoW = new Mock<ISiteUnitOfWork>();
        batterySizeUoW.Setup(a => a.GetBatterySize(It.IsAny<int>())).Returns(Task.FromResult(new BatterySize()));
        haStatusUoW.Setup(h => h.GetHaStatus(It.IsAny<int>())).Returns(Task.FromResult(new HaStatus()));
        haStyleUoW.Setup(h => h.GetHaStyle(It.IsAny<int>())).Returns(Task.FromResult(new HaStyle()));
        haWarrantyType.Setup(h => h.GetHaWarrantyType(It.IsAny<int>())).Returns(Task.FromResult(new HaWarrantyType()));
        appointmentsUoW.Setup(p => p.GetAppointment(It.IsAny<int>(), null)).Returns(Task.FromResult(new Appointment()));
        patientsUoW
            .Setup(x => x.GetPatient(It.IsAny<long>(), null)).Returns(Task.FromResult(new Patient()));
        providersUoW
            .Setup(x => x.GetProvider(It.IsAny<int>(), null)).Returns(Task.FromResult(new Provider()));
        siteUoW
            .Setup(x => x.GetSite(It.IsAny<int>())).Returns(Task.FromResult(new Site()));
        var validator = new HaHistoryValidator(batterySizeUoW.Object, patientsUoW.Object, providersUoW.Object,
            siteUoW.Object, haModelUoW.Object, haWarrantyType.Object, appointmentsUoW.Object, haStyleUoW.Object,
            haStatusUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new HaHistory()),
            OperationType.Update => await validator.Update(new HaHistory()),
            _ => null
        };

        // Assert
        Debug.Assert(validationResults != null, nameof(validationResults) + " != null");
        var validationResult = Assert.Single(validationResults);
        Assert.Equal("HaModel must exist", validationResult.Message);
        Assert.Equal(Severity.Error, validationResult.Severity);
    }

    [Theory]
    [InlineData(OperationType.AddNew, 5)]
    [InlineData(OperationType.Update, 5)]
    public async void ValidateHaWarrantyExists_ReturnsError(OperationType operationType, int warrantyId)
    {
        // Arrange
        var batterySizeUoW = new Mock<IBatterySizeUnitOfWork>();
        var appointmentsUoW = new Mock<IAppointmentsUnitOfWork>();
        var haModelUoW = new Mock<IHaModelUnitOfWork>();
        var haStatusUoW = new Mock<IHaStatusUnitOfWork>();
        var haStyleUoW = new Mock<IHaStyleUnitOfWork>();
        var haWarrantyType = new Mock<IHaWarrantyTypeUnitOfWork>();
        var patientsUoW = new Mock<IPatientsUnitOfWork>();
        var providersUoW = new Mock<IProvidersUnitOfWork>();
        var siteUoW = new Mock<ISiteUnitOfWork>();
        batterySizeUoW.Setup(a => a.GetBatterySize(It.IsAny<int>())).Returns(Task.FromResult(new BatterySize()));
        haModelUoW.Setup(h => h.GetHaModel(It.IsAny<int>(), null)).Returns(Task.FromResult(new HaModel()));
        haStatusUoW.Setup(h => h.GetHaStatus(It.IsAny<int>())).Returns(Task.FromResult(new HaStatus()));
        haStyleUoW.Setup(h => h.GetHaStyle(It.IsAny<int>())).Returns(Task.FromResult(new HaStyle()));
        appointmentsUoW.Setup(p => p.GetAppointment(It.IsAny<int>(), null)).Returns(Task.FromResult(new Appointment()));
        patientsUoW
            .Setup(x => x.GetPatient(It.IsAny<long>(), null)).Returns(Task.FromResult(new Patient()));
        providersUoW
            .Setup(x => x.GetProvider(It.IsAny<int>(), null)).Returns(Task.FromResult(new Provider()));
        siteUoW
            .Setup(x => x.GetSite(It.IsAny<int>())).Returns(Task.FromResult(new Site()));
        var validator = new HaHistoryValidator(batterySizeUoW.Object, patientsUoW.Object, providersUoW.Object,
            siteUoW.Object, haModelUoW.Object, haWarrantyType.Object, appointmentsUoW.Object, haStyleUoW.Object,
            haStatusUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new HaHistory { WarrantyTypeId = warrantyId }),
            OperationType.Update => await validator.Update(new HaHistory { WarrantyTypeId = warrantyId }),
            _ => null
        };

        // Assert
        Debug.Assert(validationResults != null, nameof(validationResults) + " != null");
        var validationResult = Assert.Single(validationResults);
        Assert.Equal("WarrantyType must exist", validationResult.Message);
        Assert.Equal(Severity.Error, validationResult.Severity);
    }

    [Theory]
    [InlineData(OperationType.AddNew, 5)]
    [InlineData(OperationType.Update, 5)]
    public async void ValidateHaStyleExists_ReturnsError(OperationType operationType, int haStyleId)
    {
        // Arrange
        var batterySizeUoW = new Mock<IBatterySizeUnitOfWork>();
        var appointmentsUoW = new Mock<IAppointmentsUnitOfWork>();
        var haModelUoW = new Mock<IHaModelUnitOfWork>();
        var haStatusUoW = new Mock<IHaStatusUnitOfWork>();
        var haStyleUoW = new Mock<IHaStyleUnitOfWork>();
        var haWarrantyType = new Mock<IHaWarrantyTypeUnitOfWork>();
        var patientsUoW = new Mock<IPatientsUnitOfWork>();
        var providersUoW = new Mock<IProvidersUnitOfWork>();
        var siteUoW = new Mock<ISiteUnitOfWork>();
        batterySizeUoW.Setup(a => a.GetBatterySize(It.IsAny<int>())).Returns(Task.FromResult(new BatterySize()));
        haModelUoW.Setup(h => h.GetHaModel(It.IsAny<int>(), null)).Returns(Task.FromResult(new HaModel()));
        haStatusUoW.Setup(h => h.GetHaStatus(It.IsAny<int>())).Returns(Task.FromResult(new HaStatus()));
        haWarrantyType.Setup(h => h.GetHaWarrantyType(It.IsAny<int>())).Returns(Task.FromResult(new HaWarrantyType()));
        appointmentsUoW.Setup(p => p.GetAppointment(It.IsAny<int>(), null)).Returns(Task.FromResult(new Appointment()));
        patientsUoW
            .Setup(x => x.GetPatient(It.IsAny<long>(), null)).Returns(Task.FromResult(new Patient()));
        providersUoW
            .Setup(x => x.GetProvider(It.IsAny<int>(), null)).Returns(Task.FromResult(new Provider()));
        siteUoW
            .Setup(x => x.GetSite(It.IsAny<int>())).Returns(Task.FromResult(new Site()));
        var validator = new HaHistoryValidator(batterySizeUoW.Object, patientsUoW.Object, providersUoW.Object,
            siteUoW.Object, haModelUoW.Object, haWarrantyType.Object, appointmentsUoW.Object, haStyleUoW.Object,
            haStatusUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new HaHistory { HaStyleId = haStyleId }),
            OperationType.Update => await validator.Update(new HaHistory { HaStyleId = haStyleId }),
            _ => null
        };

        // Assert
        Debug.Assert(validationResults != null, nameof(validationResults) + " != null");
        var validationResult = Assert.Single(validationResults);
        Assert.Equal("HaStyle must exist", validationResult.Message);
        Assert.Equal(Severity.Error, validationResult.Severity);
    }

    [Theory]
    [InlineData(OperationType.AddNew, 5)]
    [InlineData(OperationType.Update, 5)]
    public async void ValidateHaStatusExists_ReturnsError(OperationType operationType, int haStatusId)
    {
        // Arrange
        var batterySizeUoW = new Mock<IBatterySizeUnitOfWork>();
        var appointmentsUoW = new Mock<IAppointmentsUnitOfWork>();
        var haModelUoW = new Mock<IHaModelUnitOfWork>();
        var haStatusUoW = new Mock<IHaStatusUnitOfWork>();
        var haStyleUoW = new Mock<IHaStyleUnitOfWork>();
        var haWarrantyType = new Mock<IHaWarrantyTypeUnitOfWork>();
        var patientsUoW = new Mock<IPatientsUnitOfWork>();
        var providersUoW = new Mock<IProvidersUnitOfWork>();
        var siteUoW = new Mock<ISiteUnitOfWork>();
        batterySizeUoW.Setup(a => a.GetBatterySize(It.IsAny<int>())).Returns(Task.FromResult(new BatterySize()));
        haModelUoW.Setup(h => h.GetHaModel(It.IsAny<int>(), null)).Returns(Task.FromResult(new HaModel()));
        haStyleUoW.Setup(h => h.GetHaStyle(It.IsAny<int>())).Returns(Task.FromResult(new HaStyle()));
        haWarrantyType.Setup(h => h.GetHaWarrantyType(It.IsAny<int>())).Returns(Task.FromResult(new HaWarrantyType()));
        appointmentsUoW.Setup(p => p.GetAppointment(It.IsAny<int>(), null)).Returns(Task.FromResult(new Appointment()));
        patientsUoW
            .Setup(x => x.GetPatient(It.IsAny<long>(), null)).Returns(Task.FromResult(new Patient()));
        providersUoW
            .Setup(x => x.GetProvider(It.IsAny<int>(), null)).Returns(Task.FromResult(new Provider()));
        siteUoW
            .Setup(x => x.GetSite(It.IsAny<int>())).Returns(Task.FromResult(new Site()));
        var validator = new HaHistoryValidator(batterySizeUoW.Object, patientsUoW.Object, providersUoW.Object,
            siteUoW.Object, haModelUoW.Object, haWarrantyType.Object, appointmentsUoW.Object, haStyleUoW.Object,
            haStatusUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new HaHistory { HaStatusId = haStatusId }),
            OperationType.Update => await validator.Update(new HaHistory { HaStatusId = haStatusId }),
            _ => null
        };

        // Assert
        Debug.Assert(validationResults != null, nameof(validationResults) + " != null");
        var validationResult = Assert.Single(validationResults);
        Assert.Equal("HaStatus must exist", validationResult.Message);
        Assert.Equal(Severity.Error, validationResult.Severity);
    }
}