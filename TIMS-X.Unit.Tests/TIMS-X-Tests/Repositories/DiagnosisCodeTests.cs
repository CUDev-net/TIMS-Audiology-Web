using System.Diagnostics;
using System.Linq.Expressions;
using Moq;
using TIMS_X.BLL.Validation;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X_Tests.Repositories;

public class DiagnosisCodeTests : TestObjectBase<DiagnosisCode>
{
    [Theory]
    [InlineData(OperationType.Add)]
    [InlineData(OperationType.Update)]
    public override async Task TestAddAndUpdate(OperationType operationType)
    {
        await base.TestAddAndUpdate(operationType);
    }

    [Theory]
    [InlineData(OperationType.AddNew, "New", 5)]
    [InlineData(OperationType.Update, "New", 5)]
    public async void ValidateAlreadyExists_ReturnsError(OperationType operationType, string newName, int categoryId)
    {
        // Arrange
        var diagnosisCodes = new List<DiagnosisCode>
        {
            new() { Name = "New", CategoryId = 5 }
        };
        var diagnosisCodeCategories = new List<DiagnosisCodeCategory>
        {
            new() { Name = "New", Id = 5 }
        };
        var diagnosisCodeUoW = new Mock<IDiagnosisCodeUnitOfWork>();
        var diagnosisCodeCategoryUoW = new Mock<IDiagnosisCodeCategoryUnitOfWork>();
        diagnosisCodeUoW
            .Setup(x => x.GetDiagnosisCodes(It.IsAny<Expression<Func<DiagnosisCode, bool>>>(),
                null,
                null)).Returns(Task.FromResult(diagnosisCodes));
        diagnosisCodeCategoryUoW
            .Setup(x => x.GetDiagnosisCodeCategory(diagnosisCodeCategories[0].Id))
            .Returns(Task.FromResult(diagnosisCodeCategories[0]));
        var validator = new DiagnosisCodeValidator(diagnosisCodeUoW.Object, diagnosisCodeCategoryUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(
                new DiagnosisCode { Name = newName, CategoryId = categoryId }),
            OperationType.Update => await validator.Update(new DiagnosisCode { Name = newName, CategoryId = 5 }),
            _ => null
        };

        // Assert
        Debug.Assert(validationResults != null, nameof(validationResults) + " != null");
        var validationResult = Assert.Single(validationResults);
        Assert.Equal("Name must be unique", validationResult.Message);
        Assert.Equal(Severity.Error, validationResult.Severity);
    }

    [Theory]
    [InlineData(OperationType.AddNew, "Different", 5)]
    [InlineData(OperationType.Update, "Different", 5)]
    public async void ValidateDiagnosisCodeIsUnique_ReturnsNoError(OperationType operationType, string newName,
        int categoryId)
    {
        var diagnosisCodeCategories = new List<DiagnosisCodeCategory>
        {
            new() { Name = "New", Id = 5 }
        };

        // Arrange
        var diagnosisCodeCategoryUoW = new Mock<IDiagnosisCodeCategoryUnitOfWork>();
        diagnosisCodeCategoryUoW
            .Setup(x => x.GetDiagnosisCodeCategory(diagnosisCodeCategories[0].Id))
            .Returns(Task.FromResult(diagnosisCodeCategories[0]));

        var diagnosisCodeUoW = new Mock<IDiagnosisCodeUnitOfWork>();
        diagnosisCodeUoW
            .Setup(x => x.GetDiagnosisCodes(It.IsAny<Expression<Func<DiagnosisCode, bool>>>(),
                null,
                null)).Returns(Task.FromResult(new List<DiagnosisCode>()));
        var validator = new DiagnosisCodeValidator(diagnosisCodeUoW.Object, diagnosisCodeCategoryUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(
                new DiagnosisCode { Name = newName, CategoryId = categoryId }),
            OperationType.Update => await validator.Update(
                new DiagnosisCode { Name = newName, CategoryId = categoryId }),
            _ => null
        };

        // Assert
        if (validationResults != null) Assert.Empty(validationResults);
    }

    [Theory]
    [InlineData(OperationType.AddNew, "Different", 5)]
    [InlineData(OperationType.Update, "Different", 5)]
    public async void ValidateDiagnosisCodeCategoryIdExists_ReturnsError(OperationType operationType, string newName,
        int categoryId)
    {
        // Arrange
        var diagnosisCodes = new List<DiagnosisCode>
        {
            new() { CategoryId = 1 }
        };
        var diagnosisCodeUoW = new Mock<IDiagnosisCodeUnitOfWork>();
        var diagnosisCodeCategoryUoW = new Mock<IDiagnosisCodeCategoryUnitOfWork>();
        diagnosisCodeUoW
            .Setup(x => x.GetDiagnosisCodes(It.IsAny<Expression<Func<DiagnosisCode, bool>>>(),
                null,
                null)).Returns(Task.FromResult(diagnosisCodes));
        diagnosisCodeCategoryUoW
            .Setup(x => x.GetDiagnosisCodeCategory(diagnosisCodes[0].CategoryId));
        var validator = new DiagnosisCodeValidator(diagnosisCodeUoW.Object, diagnosisCodeCategoryUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(
                new DiagnosisCode { Name = newName, CategoryId = categoryId }),
            OperationType.Update => await validator.Update(
                new DiagnosisCode { Name = newName, CategoryId = categoryId }),
            _ => null
        };

        // Assert
        Debug.Assert(validationResults != null, nameof(validationResults) + " != null");
        var validationResult = Assert.Single(validationResults);
        Assert.Equal("Category must exist", validationResult.Message);
        Assert.Equal(Severity.Error, validationResult.Severity);
    }
}