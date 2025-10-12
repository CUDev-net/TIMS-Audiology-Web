using System.Diagnostics;
using System.Linq.Expressions;
using Moq;
using TIMS_X.BLL.Validation;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X_Tests.Repositories;

public class ResourceTests : TestObjectBase<Resource>
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
        var resources = new List<Resource>
        {
            new() { Name = "New" }
        };
        var resourcesUoW = new Mock<IResourceUnitOfWork>();
        resourcesUoW
            .Setup(x => x.GetResources(It.IsAny<Expression<Func<Resource, bool>>>(), null,
                null)).Returns(Task.FromResult(resources));
        var validator = new ResourceValidator(resourcesUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new Resource { Name = newName }),
            OperationType.Update => await validator.Update(new Resource { Name = newName }),
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
    public async void ValidateResourceIsUnique_ReturnsNoError(OperationType operationType, string newName)
    {
        // Arrange
        var resourceUoW = new Mock<IResourceUnitOfWork>();
        resourceUoW
            .Setup(x => x.GetResources(It.IsAny<Expression<Func<Resource, bool>>>(), null,
                null)).Returns(Task.FromResult(new List<Resource>()));
        var validator = new ResourceValidator(resourceUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new Resource { Name = newName }),
            OperationType.Update => await validator.Update(new Resource { Name = newName }),
            _ => null
        };

        // Assert
        if (validationResults != null) Assert.Empty(validationResults);
    }
}