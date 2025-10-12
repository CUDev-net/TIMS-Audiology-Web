using System.Diagnostics;
using Moq;
using TIMS_X.BLL.Validation;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X_Tests.Repositories;

public class PreviousHistoryTests : TestObjectBase<PreviousHistory>
{
    [Theory]
    [InlineData(OperationType.Add)]
    [InlineData(OperationType.Update)]
    public override async Task TestAddAndUpdate(OperationType operationType)
    {
        await base.TestAddAndUpdate(operationType);
    }

    [Theory]
    [InlineData(OperationType.AddNew, true)]
    [InlineData(OperationType.Update, true)]
    public async void ValidateMedicalConditionExists_ReturnsError(OperationType operationType, bool _protected)
    {
        // Arrange
        var medicalConditionUoW = new Mock<IMedicalConditionUnitOfWork>();
        medicalConditionUoW
            .Setup(x => x.GetMedicalCondition(It.IsAny<int>())).Returns(Task.FromResult(new MedicalCondition()));
        var validator = new PreviousHistoryValidator(medicalConditionUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new PreviousHistory { Protected = _protected }),
            OperationType.Update => await validator.Update(new PreviousHistory { Protected = _protected }),
            _ => null
        };

        // Assert
        Debug.Assert(validationResults != null, nameof(validationResults) + " != null");
        var validationResult = Assert.Single(validationResults);
        Assert.Equal("Patient medical condition must exist", validationResult.Message);
        Assert.Equal(Severity.Error, validationResult.Severity);
    }

    [Theory]
    [InlineData(OperationType.AddNew, false)]
    [InlineData(OperationType.Update, false)]
    public async void ValidatePreviousHistoryExists_ReturnsError(OperationType operationType, bool _protected)
    {
        // Arrange
        var medicalConditionUoW = new Mock<IMedicalConditionUnitOfWork>();
        var validator = new PreviousHistoryValidator(medicalConditionUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new PreviousHistory { Protected = _protected }),
            OperationType.Update => await validator.Update(new PreviousHistory { Protected = _protected }),
            _ => null
        };

        // Assert
        Debug.Assert(validationResults != null, nameof(validationResults) + " != null");
        var validationResult = Assert.Single(validationResults);
        Assert.Equal("Previous History must exist", validationResult.Message);
        Assert.Equal(Severity.Error, validationResult.Severity);
    }

    [Theory]
    [InlineData(OperationType.AddNew, true)]
    [InlineData(OperationType.Update, true)]
    public async void ValidatePreviousHistory_ReturnsNoError(OperationType operationType, bool _protected)
    {
        // Arrange
        var medicalConditionUoW = new Mock<IMedicalConditionUnitOfWork>();
        var validator = new PreviousHistoryValidator(medicalConditionUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new PreviousHistory { Protected = _protected }),
            OperationType.Update => await validator.Update(new PreviousHistory { Protected = _protected }),
            _ => null
        };

        // Assert
        if (validationResults != null) Assert.Empty(validationResults);
    }
}