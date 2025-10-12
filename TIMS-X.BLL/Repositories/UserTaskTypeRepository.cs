using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IUserTaskTypeRepository
{
    Task<UserTaskType> Add(UserTaskType userTaskType);
    void Delete(int id);
    Task<UserTaskType> Get(int id);
    Task<List<UserTaskType>> GetAll(bool includeInactive);
    Task<UserTaskType> Update(UserTaskType userTaskType);
}

public class UserTaskTypeRepository : IUserTaskTypeRepository
{
    private readonly IUserTaskTypeUnitOfWork _userTaskTypeUnitOfWork;

    public UserTaskTypeRepository(IUserTaskTypeUnitOfWork userTaskTypeUnitOfWork)
    {
        _userTaskTypeUnitOfWork = userTaskTypeUnitOfWork;
    }

    public async Task<UserTaskType> Add(UserTaskType userTaskType)
    {
        return await _userTaskTypeUnitOfWork.Add(userTaskType);
    }

    public async Task<UserTaskType> Get(int id)
    {
        return await _userTaskTypeUnitOfWork.GetUserTaskType(id);
    }

    public async Task<List<UserTaskType>> GetAll(bool includeInactive)
    {
        return await _userTaskTypeUnitOfWork
            .GetUserTaskTypes(x => includeInactive || !x.Inactive);
    }

    public async Task<UserTaskType> Update(UserTaskType userTaskType)
    {
        return await _userTaskTypeUnitOfWork.Update(userTaskType);
    }

    public void Delete(int id)
    {
        _userTaskTypeUnitOfWork.Delete(id);
    }
}