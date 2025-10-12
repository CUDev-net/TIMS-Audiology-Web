using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IManufacturerRepository
{
    Task<Manufacturer> Add(Manufacturer manufacturer);
    void Delete(int id);
    Task<Manufacturer> Get(int id);
    Task<List<Manufacturer>> GetAll(bool includeInactive);
    Task<Manufacturer> Update(Manufacturer manufacturer);
}

public class ManufacturerRepository : IManufacturerRepository
{
    private readonly IManufacturerUnitOfWork _manufacturerUnitOfWork;

    public ManufacturerRepository(IManufacturerUnitOfWork manufacturerUnitOfWork)
    {
        _manufacturerUnitOfWork = manufacturerUnitOfWork;
    }

    public async Task<Manufacturer> Add(Manufacturer manufacturer)
    {
        return await _manufacturerUnitOfWork.Add(manufacturer);
    }

    public void Delete(int id)
    {
        _manufacturerUnitOfWork.Delete(id);
    }

    public async Task<Manufacturer> Get(int id)
    {
        return await _manufacturerUnitOfWork.GetManufacturer(id);
    }

    public async Task<List<Manufacturer>> GetAll(bool includeInactive)
    {
        return await _manufacturerUnitOfWork.GetManufacturers(x => includeInactive || !x.Inactive);
    }

    public async Task<Manufacturer> Update(Manufacturer manufacturer)
    {
        return await _manufacturerUnitOfWork.Update(manufacturer);
    }
}