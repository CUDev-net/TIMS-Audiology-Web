using Moq;
using TIMS_X.Core.Domain.Base;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X_Tests.Repositories;

public abstract class TestObjectBase<T> where T : Entity, new()
{
    public enum OperationType
    {
        AddNew,
        Update,
        Add
    }

    public virtual async Task TestAddAndUpdate(OperationType operationType)
    {
        var mock = new Mock<IUnitOfWork>();

        var input = new T();
        switch (operationType)
        {
            case OperationType.Add:
            {
                var returned = new T();
                mock.Setup(x => x.Add(input)).Returns(Task.FromResult(returned));
                break;
            }
            case OperationType.Update:
                mock.Setup(x => x.Update(input)).Returns(Task.FromResult(input));
                break;
        }

        var unitOfWork = mock.Object;

        switch (operationType)
        {
            case OperationType.Add:
            {
                var created = await unitOfWork.Add(input);
                Assert.NotNull(created);
                break;
            }
            case OperationType.Update:
            {
                var created = await unitOfWork.Update(input);
                Assert.NotNull(created);
                break;
            }
        }
    }
}