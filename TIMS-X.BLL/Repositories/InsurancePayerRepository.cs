using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IInsurancePayerRepository
{
    Task<InsurancePayer> Add(InsurancePayer insurancePayer);
    void Delete(int id);
    Task<InsurancePayer> Get(int id);
    Task<List<InsurancePayer>> GetAll(bool includeInactive);
    Task<InsurancePayer> Update(InsurancePayer insurancePayer);
}

public class InsurancePayerRepository : IInsurancePayerRepository
{
    private readonly IInsurancePayerUnitOfWork _insurancePayerUnitOfWork;

    public InsurancePayerRepository(IInsurancePayerUnitOfWork insurancePayerUnitOfWork)
    {
        _insurancePayerUnitOfWork = insurancePayerUnitOfWork;
    }

    public async Task<InsurancePayer> Add(InsurancePayer insurancePayer)
    {
        return await _insurancePayerUnitOfWork.Add(insurancePayer);
    }

    public async Task<InsurancePayer> Get(int id)
    {
        return await _insurancePayerUnitOfWork.GetInsurancePayer(id);
    }

    public async Task<List<InsurancePayer>> GetAll(bool includeInactive)
    {
        return await _insurancePayerUnitOfWork.GetInsurancePayers(x => includeInactive || !x.Inactive);
    }

    public async Task<InsurancePayer> Update(InsurancePayer insurancePayer)
    {
        return await _insurancePayerUnitOfWork.Update(insurancePayer);
    }

    public void Delete(int id)
    {
        _insurancePayerUnitOfWork.Delete(id);
    }
}