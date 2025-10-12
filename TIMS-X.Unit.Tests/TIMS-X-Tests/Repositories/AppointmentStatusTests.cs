using System.Diagnostics;
using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using TIMS_X.BLL.Validation;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X_Tests.Repositories;

public class AppointmentStatusTests : TestObjectBase<AppointmentStatus>
{
    [Theory]
    [InlineData(OperationType.Add)]
    [InlineData(OperationType.Update)]
    public override async Task TestAddAndUpdate(OperationType operationType)
    {
        await base.TestAddAndUpdate(operationType);
    }

    [Theory]
    [InlineData(OperationType.AddNew)]
    [InlineData(OperationType.Update)]
    public async void TestInMemory_ReturnsError(OperationType operationType)
    {
        // Arrange
        var appointmentStatus = new AppointmentStatus { Id = 3, Name = "Different" };
        var context = new Mock<HttpContext>();
        context.Setup(c => c.User.Claims).Returns(new List<Claim> { new("User", "1") });
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        mockHttpContextAccessor.Setup(h => h.HttpContext).Returns(context.Object);
        var options = new DbContextOptionsBuilder<TimsContext>()
            .UseInMemoryDatabase(nameof(AppointmentStatus)).Options;
        var appointmentStatusDbSet = new TimsContext(options);
        await appointmentStatusDbSet.Database.EnsureDeletedAsync();
        appointmentStatusDbSet.Add(new AppointmentStatus { Id = 1, Name = "New" });
        appointmentStatusDbSet.Add(new AppointmentStatus { Id = 2, Name = "Different" });
        await appointmentStatusDbSet.SaveChangesAsync();
        var appointmentStatusUoW =
            new AppointmentStatusUnitOfWork(appointmentStatusDbSet, mockHttpContextAccessor.Object);
        var validator = new AppointmentStatusValidator(appointmentStatusUoW);

        //Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(appointmentStatus),
            OperationType.Update => await validator.Update(appointmentStatus),
            _ => null
        };

        // Assert
        Debug.Assert(validationResults != null, nameof(validationResults) + " != null");
        var validationResult = Assert.Single(validationResults);
        Assert.Equal("Name must be unique", validationResult.Message);
        Assert.Equal(Severity.Error, validationResult.Severity);
    }

    [Theory]
    [InlineData(OperationType.AddNew, "New")]
    [InlineData(OperationType.Update, "New")]
    public async void ValidateAlreadyExists_ReturnsError(OperationType operationType, string newName)
    {
        // Arrange
        var currentStatuses = new List<AppointmentStatus>
        {
            new() { Name = "New" }
        };
        var appointStatusUoW = new Mock<IAppointmentStatusUnitOfWork>();
        appointStatusUoW
            .Setup(x => x.GetAppointmentStatuses(It.IsAny<Expression<Func<AppointmentStatus, bool>>>(), null,
                null)).Returns(Task.FromResult(currentStatuses));
        var validator = new AppointmentStatusValidator(appointStatusUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new AppointmentStatus { Name = newName }),
            OperationType.Update => await validator.Update(new AppointmentStatus { Name = newName }),
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
    public async void ValidateAppointmentStatusIsUnique_ReturnsNoError(OperationType operationType, string newName)
    {
        // Arrange
        var appointStatusUoW = new Mock<IAppointmentStatusUnitOfWork>();
        appointStatusUoW
            .Setup(x => x.GetAppointmentStatuses(It.IsAny<Expression<Func<AppointmentStatus, bool>>>(), null,
                null)).Returns(Task.FromResult(new List<AppointmentStatus>()));
        var validator = new AppointmentStatusValidator(appointStatusUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new AppointmentStatus { Name = newName }),
            OperationType.Update => await validator.Update(new AppointmentStatus { Name = newName }),
            _ => null
        };

        // Assert
        if (validationResults != null) Assert.Empty(validationResults);
    }
}