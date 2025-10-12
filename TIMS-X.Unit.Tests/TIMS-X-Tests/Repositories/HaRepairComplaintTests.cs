using System.Diagnostics;
using System.Linq.Expressions;
using Moq;
using TIMS_X.BLL.Validation;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X_Tests.Repositories;

public class HaRepairComplaintTests : TestObjectBase<HaRepairComplaint>
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
        var haRepairComplaints = new List<HaRepairComplaint>
        {
            new() { Name = "New" }
        };
        var haRepairComplaintUoW = new Mock<IHaRepairComplaintUnitOfWork>();
        haRepairComplaintUoW
            .Setup(x => x.GetHaRepairComplaints(It.IsAny<Expression<Func<HaRepairComplaint, bool>>>(),
                null,
                null)).Returns(Task.FromResult(haRepairComplaints));
        var validator = new HaRepairComplaintValidator(haRepairComplaintUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new HaRepairComplaint { Name = newName }),
            OperationType.Update => await validator.Update(new HaRepairComplaint { Name = newName }),
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
    public async void ValidateHaRepairComplaintIsUnique_ReturnsNoError(OperationType operationType,
        string newName)
    {
        // Arrange
        var haRepairComplaintUoW = new Mock<IHaRepairComplaintUnitOfWork>();
        haRepairComplaintUoW
            .Setup(x => x.GetHaRepairComplaints(It.IsAny<Expression<Func<HaRepairComplaint, bool>>>(),
                null,
                null)).Returns(Task.FromResult(new List<HaRepairComplaint>()));
        var validator = new HaRepairComplaintValidator(haRepairComplaintUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new HaRepairComplaint { Name = newName }),
            OperationType.Update => await validator.Update(new HaRepairComplaint { Name = newName }),
            _ => null
        };

        // Assert
        if (validationResults != null) Assert.Empty(validationResults);
    }
}