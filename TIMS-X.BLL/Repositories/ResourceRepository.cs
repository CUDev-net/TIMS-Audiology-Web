using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IResourceRepository
{
    Task<Resource> Add(Resource resource);
    void Delete(int id);
    Task<Resource> Get(int id);
    Task<List<Resource>> GetAll(bool includeInactive);
    Task<Resource> Update(Resource resource);
}

public class ResourceRepository : IResourceRepository
{
    private readonly IResourceUnitOfWork _resourceUnitOfWork;

    public ResourceRepository(IResourceUnitOfWork resourceUnitOfWork)
    {
        _resourceUnitOfWork = resourceUnitOfWork;
    }

    public async Task<Resource> Add(Resource resource)
    {
        return await _resourceUnitOfWork.Add(resource);
    }

    public void Delete(int id)
    {
        _resourceUnitOfWork.Delete(id);
    }

    public async Task<Resource> Get(int id)
    {
        return await _resourceUnitOfWork.GetResource(id);
    }

    public async Task<List<Resource>> GetAll(bool includeInactive)
    {
        return await _resourceUnitOfWork.GetResources(x => includeInactive || !x.Inactive);
    }

    public async Task<Resource> Update(Resource resource)
    {
        return await _resourceUnitOfWork.Update(resource);
    }
}