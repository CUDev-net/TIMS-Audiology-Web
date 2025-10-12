using System.Diagnostics;
using System.Linq.Expressions;
using Moq;
using TIMS_X.BLL.Validation;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X_Tests.Repositories;

public class ResultTypeTests : TestObjectBase<ResultType>
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
        var resultTypes = new List<ResultType>
        {
            new() { Name = "New" }
        };
        var resultTypeUoW = new Mock<IResultTypeUnitOfWork>();
        resultTypeUoW
            .Setup(x => x.GetResultTypes(It.IsAny<Expression<Func<ResultType, bool>>>(), null,
                null)).Returns(Task.FromResult(resultTypes));
        var validator = new ResultTypeValidator(resultTypeUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new ResultType { Name = newName }),
            OperationType.Update => await validator.Update(new ResultType { Name = newName }),
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
    public async void ValidateResultTypeIsUnique_ReturnsNoError(OperationType operationType, string newName)
    {
        // Arrange
        var resultTypeUoW = new Mock<IResultTypeUnitOfWork>();
        resultTypeUoW
            .Setup(x => x.GetResultTypes(It.IsAny<Expression<Func<ResultType, bool>>>(), null,
                null)).Returns(Task.FromResult(new List<ResultType>()));
        var validator = new ResultTypeValidator(resultTypeUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new ResultType { Name = newName }),
            OperationType.Update => await validator.Update(new ResultType { Name = newName }),
            _ => null
        };

        // Assert
        if (validationResults != null) Assert.Empty(validationResults);
    }
}