using System.Diagnostics;
using System.Linq.Expressions;
using Moq;
using TIMS_X.BLL.Validation;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X_Tests.Repositories;

public class ResultTests : TestObjectBase<Result>
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
        var results = new List<Result>
        {
            new() { Name = "New" }
        };
        var resultUoW = new Mock<IResultUnitOfWork>();
        resultUoW
            .Setup(x => x.GetResults(It.IsAny<Expression<Func<Result, bool>>>(), null,
                null)).Returns(Task.FromResult(results));
        var validator = new ResultValidator(resultUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new Result { Name = newName }),
            OperationType.Update => await validator.Update(new Result { Name = newName }),
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
    public async void ValidateResultIsUnique_ReturnsNoError(OperationType operationType, string newName)
    {
        // Arrange
        var resultUoW = new Mock<IResultUnitOfWork>();
        resultUoW
            .Setup(x => x.GetResults(It.IsAny<Expression<Func<Result, bool>>>(), null,
                null)).Returns(Task.FromResult(new List<Result>()));
        var validator = new ResultValidator(resultUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new Result { Name = newName }),
            OperationType.Update => await validator.Update(new Result { Name = newName }),
            _ => null
        };

        // Assert
        if (validationResults != null) Assert.Empty(validationResults);
    }
}