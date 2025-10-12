using System.Diagnostics;
using System.Linq.Expressions;
using Moq;
using TIMS_X.BLL.Validation;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X_Tests.Repositories;

public class AuthorizationTests : TestObjectBase<Authorization>
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
        var authorizations = new List<Authorization>
        {
            new() { Name = "New" }
        };
        var authorizationUoW = new Mock<IAuthorizationUnitOfWork>();
        authorizationUoW
            .Setup(x => x.GetAuthorizations(It.IsAny<Expression<Func<Authorization, bool>>>(), null,
                null)).Returns(Task.FromResult(authorizations));
        var validator = new AuthorizationValidator(authorizationUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new Authorization { Name = newName }),
            OperationType.Update => await validator.Update(new Authorization { Name = newName }),
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
    public async void ValidateAppointmentTypeIsUnique_ReturnsNoError(OperationType operationType, string newName)
    {
        // Arrange
        var authorizationUoW = new Mock<IAuthorizationUnitOfWork>();
        authorizationUoW
            .Setup(x => x.GetAuthorizations(It.IsAny<Expression<Func<Authorization, bool>>>(), null,
                null)).Returns(Task.FromResult(new List<Authorization>()));
        var validator = new AuthorizationValidator(authorizationUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new Authorization { Name = newName }),
            OperationType.Update => await validator.Update(new Authorization { Name = newName }),
            _ => null
        };

        // Assert
        if (validationResults != null) Assert.Empty(validationResults);
    }
}