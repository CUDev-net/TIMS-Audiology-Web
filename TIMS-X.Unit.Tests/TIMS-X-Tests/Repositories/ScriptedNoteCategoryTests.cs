using System.Diagnostics;
using System.Linq.Expressions;
using Moq;
using TIMS_X.BLL.Validation;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X_Tests.Repositories;

public class ScriptedNoteCategoryTests : TestObjectBase<ScriptedNoteCategory>
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
        var scriptedNoteCategories = new List<ScriptedNoteCategory>
        {
            new() { Name = "New" }
        };
        var scriptedNoteCategoryUoW = new Mock<IScriptedNoteCategoryUnitOfWork>();
        scriptedNoteCategoryUoW
            .Setup(x => x.GetScriptedNoteCategories(It.IsAny<Expression<Func<ScriptedNoteCategory, bool>>>(), null,
                null)).Returns(Task.FromResult(scriptedNoteCategories));
        var validator = new ScriptedNoteCategoryValidator(scriptedNoteCategoryUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new ScriptedNoteCategory { Name = newName }),
            OperationType.Update => await validator.Update(new ScriptedNoteCategory { Name = newName }),
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
    public async void ValidateScriptedNoteCategoryIsUnique_ReturnsNoError(OperationType operationType, string newName)
    {
        // Arrange
        var scriptedNoteCategoryUoW = new Mock<IScriptedNoteCategoryUnitOfWork>();
        scriptedNoteCategoryUoW
            .Setup(x => x.GetScriptedNoteCategories(It.IsAny<Expression<Func<ScriptedNoteCategory, bool>>>(), null,
                null)).Returns(Task.FromResult(new List<ScriptedNoteCategory>()));
        var validator = new ScriptedNoteCategoryValidator(scriptedNoteCategoryUoW.Object);

        // Act
        List<ValidationResult>? validationResults = operationType switch
        {
            OperationType.AddNew => await validator.AddNew(new ScriptedNoteCategory { Name = newName }),
            OperationType.Update => await validator.Update(new ScriptedNoteCategory { Name = newName }),
            _ => null
        };

        // Assert
        if (validationResults != null) Assert.Empty(validationResults);
    }
}