using System.Diagnostics;
using System.Linq.Expressions;
using Moq;
using TIMS_X.BLL.Validation;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X_Tests.Repositories;

public class InsurancePayerTests : TestObjectBase<InsurancePayer>
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
        var insurancePayers = new List<InsurancePayer>
        {
            new() { Name = "New" }
        };
        var insurancePayerUoW = new Mock<IInsurancePayerUnitOfWork>();
        insurancePayerUoW
            .Setup(x => x.GetInsurancePayers(It.IsAny<Expression<Func<InsurancePayer, bool>>>(), null,
                null)).Returns(Task.FromResult(insurancePayers));
        var validator = new InsurancePayerValidator(insurancePayerUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new InsurancePayer { Name = newName }),
            OperationType.Update => await validator.Update(new InsurancePayer { Name = newName }),
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
    public async void ValidateInsurancePayerIsUnique_ReturnsNoError(OperationType operationType, string newName)
    {
        // Arrange
        var insurancePayerUoW = new Mock<IInsurancePayerUnitOfWork>();
        insurancePayerUoW
            .Setup(x => x.GetInsurancePayers(It.IsAny<Expression<Func<InsurancePayer, bool>>>(), null,
                null)).Returns(Task.FromResult(new List<InsurancePayer>()));
        var validator = new InsurancePayerValidator(insurancePayerUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new InsurancePayer { Name = newName }),
            OperationType.Update => await validator.Update(new InsurancePayer { Name = newName }),
            _ => null
        };

        // Assert
        if (validationResults != null) Assert.Empty(validationResults);
    }
}