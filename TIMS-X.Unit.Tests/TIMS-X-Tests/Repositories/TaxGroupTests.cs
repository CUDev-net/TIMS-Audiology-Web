using System.Diagnostics;
using System.Linq.Expressions;
using Moq;
using TIMS_X.BLL.Validation;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X_Tests.Repositories;

public class TaxGroupTests : TestObjectBase<TaxGroup>
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
        var taxGroups = new List<TaxGroup>
        {
            new() { Name = "New" }
        };
        var taxGroupUoW = new Mock<ITaxGroupUnitOfWork>();
        taxGroupUoW
            .Setup(x => x.GetTaxGroups(It.IsAny<Expression<Func<TaxGroup, bool>>>(), null,
                null)).Returns(Task.FromResult(taxGroups));
        var validator = new TaxGroupValidator(taxGroupUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new TaxGroup { Name = newName }),
            OperationType.Update => await validator.Update(new TaxGroup { Name = newName }),
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
    public async void ValidateTaxGroupIsUnique_ReturnsNoError(OperationType operationType, string newName)
    {
        // Arrange
        var taxGroupUoW = new Mock<ITaxGroupUnitOfWork>();
        taxGroupUoW
            .Setup(x => x.GetTaxGroups(It.IsAny<Expression<Func<TaxGroup, bool>>>(), null,
                null)).Returns(Task.FromResult(new List<TaxGroup>()));
        var validator = new TaxGroupValidator(taxGroupUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new TaxGroup { Name = newName }),
            OperationType.Update => await validator.Update(new TaxGroup { Name = newName }),
            _ => null
        };

        // Assert
        if (validationResults != null) Assert.Empty(validationResults);
    }
}