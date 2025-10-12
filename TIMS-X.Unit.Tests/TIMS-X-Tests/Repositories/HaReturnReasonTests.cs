using System.Diagnostics;
using System.Linq.Expressions;
using Moq;
using TIMS_X.BLL.Validation;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X_Tests.Repositories;

public class HaReturnReasonTests : TestObjectBase<HaReturnReason>
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
        var haReturnReasons = new List<HaReturnReason>
        {
            new() { Name = "New" }
        };
        var haReturnReasonUoW = new Mock<IHaReturnReasonUnitOfWork>();
        haReturnReasonUoW
            .Setup(x => x.GetReturnReasons(It.IsAny<Expression<Func<HaReturnReason, bool>>>(), null,
                null)).Returns(Task.FromResult(haReturnReasons));
        var validator = new HaReturnReasonValidator(haReturnReasonUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new HaReturnReason { Name = newName }),
            OperationType.Update => await validator.Update(new HaReturnReason { Name = newName }),
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
    public async void ValidateHaReturnReasonIsUnique_ReturnsNoError(OperationType operationType, string newName)
    {
        // Arrange
        var haReturnReasonUoW = new Mock<IHaReturnReasonUnitOfWork>();
        haReturnReasonUoW
            .Setup(x => x.GetReturnReasons(It.IsAny<Expression<Func<HaReturnReason, bool>>>(), null,
                null)).Returns(Task.FromResult(new List<HaReturnReason>()));
        var validator = new HaReturnReasonValidator(haReturnReasonUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new HaReturnReason { Name = newName }),
            OperationType.Update => await validator.Update(new HaReturnReason { Name = newName }),
            _ => null
        };

        // Assert
        if (validationResults != null) Assert.Empty(validationResults);
    }
}