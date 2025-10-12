using System.Diagnostics;
using System.Linq.Expressions;
using Moq;
using TIMS_X.BLL.Validation;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X_Tests.Repositories;

public class SubmitterInfoTests : TestObjectBase<SubmitterInfo>
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
        var submitterInformation = new List<SubmitterInfo>
        {
            new() { Name = "New" }
        };
        var submitterInfoUoW = new Mock<ISubmitterInfoUnitOfWork>();
        submitterInfoUoW
            .Setup(x => x.GetSubmitterInfos(It.IsAny<Expression<Func<SubmitterInfo, bool>>>(), null,
                null)).Returns(Task.FromResult(submitterInformation));
        var validator = new SubmitterInfoValidator(submitterInfoUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new SubmitterInfo { Name = newName }),
            OperationType.Update => await validator.Update(new SubmitterInfo { Name = newName }),
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
    public async void ValidateSubmitterInfoIsUnique_ReturnsNoError(OperationType operationType, string newName)
    {
        // Arrange
        var submitterInfoUoW = new Mock<ISubmitterInfoUnitOfWork>();
        submitterInfoUoW
            .Setup(x => x.GetSubmitterInfos(It.IsAny<Expression<Func<SubmitterInfo, bool>>>(), null,
                null)).Returns(Task.FromResult(new List<SubmitterInfo>()));
        var validator = new SubmitterInfoValidator(submitterInfoUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new SubmitterInfo { Name = newName }),
            OperationType.Update => await validator.Update(new SubmitterInfo { Name = newName }),
            _ => null
        };

        // Assert
        if (validationResults != null) Assert.Empty(validationResults);
    }
}