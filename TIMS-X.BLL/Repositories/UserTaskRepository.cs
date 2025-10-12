using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IUserTaskRepository
{
    Task<UserTask> Add(UserTask userTask);
    void Delete(int id);
    Task<UserTask> Get(int id);
    Task<List<UserTask>> GetAll(bool includeInactive);
    Task<UserTask> Update(UserTask userTask);
}

public class UserTaskRepository : IUserTaskRepository
{
    private readonly IUserTaskUnitOfWork _userTaskUnitOfWork;

    public UserTaskRepository(IUserTaskUnitOfWork userTaskUnitOfWork)
    {
        _userTaskUnitOfWork = userTaskUnitOfWork;
    }

    public async Task<UserTask> Add(UserTask userTask)
    {
        userTask.UpdatedDate = DateTime.Now;
        return await _userTaskUnitOfWork.Add(userTask);
    }

    public async Task<UserTask> Get(int id)
    {
        return await _userTaskUnitOfWork.GetUserTask(id);
    }

    public async Task<List<UserTask>> GetAll(bool includeInactive)
    {
        return await _userTaskUnitOfWork
            .GetUserTasks(null, null, a => a.Include(x => x.UserTaskType))
            .ToListAsync();
    }

    public async Task<UserTask> Update(UserTask userTask)
    {
        userTask.UpdatedDate = DateTime.Now;
        return await _userTaskUnitOfWork.Update(userTask);
    }

    public void Delete(int id)
    {
        _userTaskUnitOfWork.Delete(id);
    }
}