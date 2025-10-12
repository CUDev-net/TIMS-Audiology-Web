using System.Diagnostics;
using System.Linq.Expressions;
using Moq;
using TIMS_X.BLL.Validation;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X_Tests.Repositories;

public class ManufacturerTests : TestObjectBase<Manufacturer>
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
        var manufacturers = new List<Manufacturer>
        {
            new() { Name = "New" }
        };
        var manufacturerUoW = new Mock<IManufacturerUnitOfWork>();
        manufacturerUoW
            .Setup(x => x.GetManufacturers(It.IsAny<Expression<Func<Manufacturer, bool>>>(), null,
                null)).Returns(Task.FromResult(manufacturers));
        var validator = new ManufacturerValidator(manufacturerUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new Manufacturer { Name = newName }),
            OperationType.Update => await validator.Update(new Manufacturer { Name = newName }),
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
    public async void ValidateManufacturerIsUnique_ReturnsNoError(OperationType operationType, string newName)
    {
        // Arrange
        var manufacturerUoW = new Mock<IManufacturerUnitOfWork>();
        manufacturerUoW
            .Setup(x => x.GetManufacturers(It.IsAny<Expression<Func<Manufacturer, bool>>>(), null,
                null)).Returns(Task.FromResult(new List<Manufacturer>()));
        var validator = new ManufacturerValidator(manufacturerUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new Manufacturer { Name = newName }),
            OperationType.Update => await validator.Update(new Manufacturer { Name = newName }),
            _ => null
        };

        // Assert
        if (validationResults != null) Assert.Empty(validationResults);
    }
}