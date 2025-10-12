using System.Diagnostics;
using System.Linq.Expressions;
using Moq;
using TIMS_X.BLL.Validation;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X_Tests.Repositories;

public class PaymentMethodTests : TestObjectBase<PaymentMethod>
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
        var paymentMethods = new List<PaymentMethod>
        {
            new() { Name = "New" }
        };
        var paymentMethodUoW = new Mock<IPaymentMethodUnitOfWork>();
        paymentMethodUoW
            .Setup(x => x.GetPaymentMethods(It.IsAny<Expression<Func<PaymentMethod, bool>>>(), null,
                null)).Returns(Task.FromResult(paymentMethods));
        var validator = new PaymentMethodValidator(paymentMethodUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new PaymentMethod { Name = newName }),
            OperationType.Update => await validator.Update(new PaymentMethod { Name = newName }),
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
    public async void ValidatePaymentMethodIsUnique_ReturnsNoError(OperationType operationType, string newName)
    {
        // Arrange
        var paymentMethodUoW = new Mock<IPaymentMethodUnitOfWork>();
        paymentMethodUoW
            .Setup(x => x.GetPaymentMethods(It.IsAny<Expression<Func<PaymentMethod, bool>>>(), null,
                null)).Returns(Task.FromResult(new List<PaymentMethod>()));
        var validator = new PaymentMethodValidator(paymentMethodUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new PaymentMethod { Name = newName }),
            OperationType.Update => await validator.Update(new PaymentMethod { Name = newName }),
            _ => null
        };

        // Assert
        if (validationResults != null) Assert.Empty(validationResults);
    }
}