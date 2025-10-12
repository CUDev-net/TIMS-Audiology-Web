using System.Diagnostics;
using System.Linq.Expressions;
using Moq;
using TIMS_X.BLL.Validation;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X_Tests.Repositories;

public class CustomerMessageTests : TestObjectBase<CustomerMessage>
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
        var customerMessages = new List<CustomerMessage>
        {
            new() { Name = "New" }
        };
        var customerMessageUoW = new Mock<ICustomerMessageUnitOfWork>();
        customerMessageUoW
            .Setup(x => x.GetCustomerMessages(It.IsAny<Expression<Func<CustomerMessage, bool>>>(),
                null,
                null)).Returns(Task.FromResult(customerMessages));
        var validator = new CustomerMessageValidator(customerMessageUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new CustomerMessage { Name = newName }),
            OperationType.Update => await validator.Update(new CustomerMessage { Name = newName }),
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
    public async void ValidateCustomerMessageIsUnique_ReturnsNoError(OperationType operationType,
        string newName)
    {
        // Arrange
        var customerMessageUoW = new Mock<ICustomerMessageUnitOfWork>();
        customerMessageUoW
            .Setup(x => x.GetCustomerMessages(It.IsAny<Expression<Func<CustomerMessage, bool>>>(),
                null,
                null)).Returns(Task.FromResult(new List<CustomerMessage>()));
        var validator = new CustomerMessageValidator(customerMessageUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new CustomerMessage { Name = newName }),
            OperationType.Update => await validator.Update(new CustomerMessage { Name = newName }),
            _ => null
        };

        // Assert
        if (validationResults != null) Assert.Empty(validationResults);
    }
}