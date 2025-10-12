using System.Diagnostics;
using System.Linq.Expressions;
using Moq;
using TIMS_X.BLL.Validation;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X_Tests.Repositories;

public class UserTaskTypeTests : TestObjectBase<UserTaskType>
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
        var userTaskTypes = new List<UserTaskType>
        {
            new() { Name = "New" }
        };
        var userTaskTypeUoW = new Mock<IUserTaskTypeUnitOfWork>();
        userTaskTypeUoW
            .Setup(x => x.GetUserTaskTypes(It.IsAny<Expression<Func<UserTaskType, bool>>>(), null,
                null)).Returns(Task.FromResult(userTaskTypes));
        var validator = new UserTaskTypeValidator(userTaskTypeUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new UserTaskType { Name = newName }),
            OperationType.Update => await validator.Update(new UserTaskType { Name = newName }),
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
    public async void ValidateUserTaskTypeIsUnique_ReturnsNoError(OperationType operationType, string newName)
    {
        // Arrange
        var userTaskTypeUoW = new Mock<IUserTaskTypeUnitOfWork>();
        userTaskTypeUoW
            .Setup(x => x.GetUserTaskTypes(It.IsAny<Expression<Func<UserTaskType, bool>>>(), null,
                null)).Returns(Task.FromResult(new List<UserTaskType>()));
        var validator = new UserTaskTypeValidator(userTaskTypeUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new UserTaskType { Name = newName }),
            OperationType.Update => await validator.Update(new UserTaskType { Name = newName }),
            _ => null
        };

        // Assert
        if (validationResults != null) Assert.Empty(validationResults);
    }
}