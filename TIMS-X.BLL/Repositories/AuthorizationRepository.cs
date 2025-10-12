using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IAuthorizationRepository
{
    Task<Authorization> Add(Authorization authorizations);
    void Delete(int id);
    Task<Authorization> Get(int id);
    Task<List<Authorization>> GetAll(bool includeInactive);
    Task<Authorization> Update(Authorization authorizations);
}

public class AuthorizationRepository : IAuthorizationRepository
{
    private readonly IAuthorizationUnitOfWork _authorizationUnitOfWork;

    public AuthorizationRepository(IAuthorizationUnitOfWork authorizationUnitOfWork)
    {
        _authorizationUnitOfWork = authorizationUnitOfWork;
    }

    public async Task<Authorization> Add(Authorization authorization)
    {
        return await _authorizationUnitOfWork.Add(authorization);
    }

    public void Delete(int id)
    {
        _authorizationUnitOfWork.Delete(id);
    }

    public async Task<Authorization> Get(int id)
    {
        return await _authorizationUnitOfWork.GetAuthorization(id);
    }

    public async Task<List<Authorization>> GetAll(bool includeInactive)
    {
        return await _authorizationUnitOfWork.GetAuthorizations(x => includeInactive || !x.Inactive);
    }

    public async Task<Authorization> Update(Authorization authorizations)
    {
        return await _authorizationUnitOfWork.Update(authorizations);
    }
}