using System.Diagnostics;
using System.Linq.Expressions;
using Moq;
using TIMS_X.BLL.Validation;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X_Tests.Repositories;

public class TaxItemTests : TestObjectBase<TaxItem>
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
    public async void ValidateTaxItemAlreadyExists_ReturnsError(OperationType operationType, string newName)
    {
        // Arrange
        var taxItems = new List<TaxItem>
        {
            new() { Name = "New" }
        };
        var taxItemUoW = new Mock<ITaxItemUnitOfWork>();
        var taxAgencyUoW = new Mock<ITaxAgencyUnitOfWork>();
        taxAgencyUoW.Setup(x => x.GetTaxAgency(It.IsAny<int>())).Returns(Task.FromResult(new TaxAgency()));
        taxItemUoW
            .Setup(x => x.GetTaxItems(It.IsAny<Expression<Func<TaxItem, bool>>>(), null,
                null)).Returns(Task.FromResult(taxItems));
        var validator = new TaxItemValidator(taxItemUoW.Object, taxAgencyUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new TaxItem { Name = newName }),
            OperationType.Update => await validator.Update(new TaxItem { Name = newName }),
            _ => null
        };

        // Assert
        Debug.Assert(validationResults != null, nameof(validationResults) + " != null");
        var validationResult = Assert.Single(validationResults);
        Assert.Equal("Name must be unique", validationResult.Message);
        Assert.Equal(Severity.Error, validationResult.Severity);
    }

    [Theory]
    [InlineData(OperationType.AddNew, "New")]
    [InlineData(OperationType.Update, "New")]
    public async void ValidateTaxAgencyIdExists_ReturnsError(OperationType operationType, string newName)
    {
        // Arrange
        var taxItemUoW = new Mock<ITaxItemUnitOfWork>();
        var taxAgencyUoW = new Mock<ITaxAgencyUnitOfWork>();
        taxItemUoW
            .Setup(x => x.GetTaxItems(It.IsAny<Expression<Func<TaxItem, bool>>>(), null,
                null)).Returns(Task.FromResult(new List<TaxItem>()));
        var validator = new TaxItemValidator(taxItemUoW.Object, taxAgencyUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new TaxItem { Name = newName, AgencyId = 4}),
            OperationType.Update => await validator.Update(new TaxItem { Name = newName, AgencyId = 4}),
            _ => null
        };

        // Assert
        Debug.Assert(validationResults != null, nameof(validationResults) + " != null");
        var validationResult = Assert.Single(validationResults);
        Assert.Equal("Tax Agency must exist", validationResult.Message);
        Assert.Equal(Severity.Error, validationResult.Severity);
    }

    [Theory]
    [InlineData(OperationType.AddNew, "Different")]
    [InlineData(OperationType.Update, "Different")]
    public async void ValidateTaxItemIsUnique_ReturnsNoError(OperationType operationType, string newName)
    {
        // Arrange
        var taxItemUoW = new Mock<ITaxItemUnitOfWork>();
        var taxAgencyUoW = new Mock<ITaxAgencyUnitOfWork>();
        taxAgencyUoW.Setup(x => x.GetTaxAgency(It.IsAny<int>())).Returns(Task.FromResult(new TaxAgency()));
        taxItemUoW
            .Setup(x => x.GetTaxItems(It.IsAny<Expression<Func<TaxItem, bool>>>(), null,
                null)).Returns(Task.FromResult(new List<TaxItem>()));
        var validator = new TaxItemValidator(taxItemUoW.Object, taxAgencyUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new TaxItem { Name = newName }),
            OperationType.Update => await validator.Update(new TaxItem { Name = newName }),
            _ => null
        };

        // Assert
        if (validationResults != null) Assert.Empty(validationResults);
    }
}