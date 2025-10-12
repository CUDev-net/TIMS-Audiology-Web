using System.Diagnostics;
using System.Linq.Expressions;
using Moq;
using TIMS_X.BLL.Validation;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X_Tests.Repositories;

public class PatientStatusTests : TestObjectBase<PatientStatus>
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
        var patientStatuses = new List<PatientStatus>
        {
            new() { Name = "New" }
        };
        var patientStatusUoW = new Mock<IPatientStatusUnitOfWork>();
        patientStatusUoW
            .Setup(x => x.GetPatientStatuses(It.IsAny<Expression<Func<PatientStatus, bool>>>(), null,
                null)).Returns(Task.FromResult(patientStatuses));
        var validator = new PatientStatusValidator(patientStatusUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new PatientStatus { Name = newName }),
            OperationType.Update => await validator.Update(new PatientStatus { Name = newName }),
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
    public async void ValidatePatientStatusIsUnique_ReturnsNoError(OperationType operationType, string newName)
    {
        // Arrange
        var patientStatusUoW = new Mock<IPatientStatusUnitOfWork>();
        patientStatusUoW
            .Setup(x => x.GetPatientStatuses(It.IsAny<Expression<Func<PatientStatus, bool>>>(), null,
                null)).Returns(Task.FromResult(new List<PatientStatus>()));
        var validator = new PatientStatusValidator(patientStatusUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new PatientStatus { Name = newName }),
            OperationType.Update => await validator.Update(new PatientStatus { Name = newName }),
            _ => null
        };

        // Assert
        if (validationResults != null) Assert.Empty(validationResults);
    }
}