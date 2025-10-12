using System.Diagnostics;
using System.Linq.Expressions;
using Moq;
using TIMS_X.BLL.Validation;
using TIMS_X.DAL.DAL.UoWs;
using MedicalCondition = TIMS_X.Core.Domain.MedicalCondition;

namespace TIMS_X_Tests.Repositories;

public class MedicalConditionTests : TestObjectBase<MedicalCondition>
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
        var medicalConditions = new List<MedicalCondition>
        {
            new() { Name = "New" }
        };
        var medicalConditionUoW = new Mock<IMedicalConditionUnitOfWork>();
        medicalConditionUoW
            .Setup(x => x.GetMedicalConditions(It.IsAny<Expression<Func<MedicalCondition, bool>>>(), null,
                null)).Returns(Task.FromResult(medicalConditions));
        var validator = new MedicalConditionValidator(medicalConditionUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new MedicalCondition { Name = newName }),
            OperationType.Update => await validator.Update(new MedicalCondition { Name = newName }),
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
    public async void ValidateMedicalConditionIsUnique_ReturnsNoError(OperationType operationType, string newName)
    {
        // Arrange
        var medicalConditionUoW = new Mock<IMedicalConditionUnitOfWork>();
        medicalConditionUoW
            .Setup(x => x.GetMedicalConditions(It.IsAny<Expression<Func<MedicalCondition, bool>>>(), null,
                null)).Returns(Task.FromResult(new List<MedicalCondition>()));
        var validator = new MedicalConditionValidator(medicalConditionUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new MedicalCondition { Name = newName }),
            OperationType.Update => await validator.Update(new MedicalCondition { Name = newName }),
            _ => null
        };

        // Assert
        if (validationResults != null) Assert.Empty(validationResults);
    }
}