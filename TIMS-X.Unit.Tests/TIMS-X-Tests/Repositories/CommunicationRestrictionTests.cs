using System.Diagnostics;
using System.Linq.Expressions;
using Moq;
using TIMS_X.BLL.Validation;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X_Tests.Repositories;

public class CommunicationRestrictionTests : TestObjectBase<CommunicationRestriction>
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
        var authorizations = new List<CommunicationRestriction>
        {
            new() { Name = "New" }
        };
        var communicationRestrictionUoW = new Mock<ICommunicationRestrictionUnitOfWork>();
        communicationRestrictionUoW
            .Setup(x => x.GetCommunicationRestrictions(It.IsAny<Expression<Func<CommunicationRestriction, bool>>>(),
                null,
                null)).Returns(Task.FromResult(authorizations));
        var validator = new CommunicationRestrictionValidator(communicationRestrictionUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new CommunicationRestriction { Name = newName }),
            OperationType.Update => await validator.Update(new CommunicationRestriction { Name = newName }),
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
    public async void ValidateCommunicationRestrictionIsUnique_ReturnsNoError(OperationType operationType,
        string newName)
    {
        // Arrange
        var communicationRestrictionUoW = new Mock<ICommunicationRestrictionUnitOfWork>();
        communicationRestrictionUoW
            .Setup(x => x.GetCommunicationRestrictions(It.IsAny<Expression<Func<CommunicationRestriction, bool>>>(),
                null,
                null)).Returns(Task.FromResult(new List<CommunicationRestriction>()));
        var validator = new CommunicationRestrictionValidator(communicationRestrictionUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new CommunicationRestriction { Name = newName }),
            OperationType.Update => await validator.Update(new CommunicationRestriction { Name = newName }),
            _ => null
        };

        // Assert
        if (validationResults != null) Assert.Empty(validationResults);
    }
}