using System.Diagnostics;
using System.Linq.Expressions;
using Moq;
using TIMS_X.BLL.Validation;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X_Tests.Repositories;

public class AppointmentTypeTests : TestObjectBase<AppointmentType>
{
    [Theory]
    [InlineData(OperationType.Add)]
    [InlineData(OperationType.Update)]
    public override async Task TestAddAndUpdate(OperationType operationType)
    {
        await base.TestAddAndUpdate(operationType);
    }

    [Theory]
    [InlineData(OperationType.AddNew, "New")]
    [InlineData(OperationType.Update, "New")]
    public async void ValidateAlreadyExists_ReturnsError(OperationType operationType, string newName)
    {
        // Arrange
        var currentTypes = new List<AppointmentType>
        {
            new() { Name = "New", Duration = 10 }
        };
        var currentHistoryTypes = new List<HistoryType>
        {
            new() { Name = "New" }
        };
        var appointTypeUoW = new Mock<IAppointmentTypeUnitOfWork>();
        var historyTypeUoW = new Mock<IHistoryTypeUnitOfWork>();
        appointTypeUoW
            .Setup(x => x.GetAppointmentTypes(It.IsAny<Expression<Func<AppointmentType, bool>>>(), null,
                null)).Returns(Task.FromResult(currentTypes));
        historyTypeUoW
            .Setup(x => x.GetHistoryTypes(It.IsAny<Expression<Func<HistoryType, bool>>>(), null,
                null)).Returns(Task.FromResult(currentHistoryTypes));
        var validator = new AppointmentTypeValidator(appointTypeUoW.Object, historyTypeUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new AppointmentType { Name = newName, Duration = 10 }),
            OperationType.Update => await validator.Update(new AppointmentType { Name = newName, Duration = 10 }),
            _ => null
        };

        // Assert
        Debug.Assert(validationResults != null, nameof(validationResults) + " != null");
        var validationResult = Assert.Single(validationResults);
        Assert.Equal("Name must be unique", validationResult.Message);
        Assert.Equal(Severity.Error, validationResult.Severity);
    }

    [Theory]
    [InlineData(OperationType.AddNew, "Different")]
    [InlineData(OperationType.Update, "Different")]
    public async void ValidateAppointmentTypeIsUnique_ReturnsNoError(OperationType operationType, string newName)
    {
        // Arrange
        var appointTypeUoW = new Mock<IAppointmentTypeUnitOfWork>();
        var historyTypeUoW = new Mock<IHistoryTypeUnitOfWork>();
        appointTypeUoW
            .Setup(x => x.GetAppointmentTypes(It.IsAny<Expression<Func<AppointmentType, bool>>>(), null,
                null)).Returns(Task.FromResult(new List<AppointmentType>()));
        historyTypeUoW
            .Setup(x => x.GetHistoryTypes(It.IsAny<Expression<Func<HistoryType, bool>>>(), null,
                null)).Returns(Task.FromResult(new List<HistoryType>()));
        var validator = new AppointmentTypeValidator(appointTypeUoW.Object, historyTypeUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new AppointmentType { Name = newName, Duration = 10 }),
            OperationType.Update => await validator.Update(new AppointmentType { Name = newName, Duration = 10 }),
            _ => null
        };

        // Assert
        if (validationResults != null) Assert.Empty(validationResults);
    }
}