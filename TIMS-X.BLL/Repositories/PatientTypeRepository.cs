using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IPatientTypeRepository
{
    Task<PatientType> Add(PatientType patientType);
    void Delete(int id);
    Task<PatientType> Get(int id);
    Task<List<PatientType>> GetAll(bool includeInactive);
    Task<PatientType> Update(PatientType patientType);
}

public class PatientTypeRepository : IPatientTypeRepository
{
    private readonly IPatientTypeUnitOfWork _patientTypeUnitOfWork;

    public PatientTypeRepository(IPatientTypeUnitOfWork patientTypeUnitOfWork)
    {
        _patientTypeUnitOfWork = patientTypeUnitOfWork;
    }

    public async Task<PatientType> Add(PatientType patientType)
    {
        return await _patientTypeUnitOfWork.Add(patientType);
    }

    public async Task<PatientType> Get(int id)
    {
        return await _patientTypeUnitOfWork.GetPatientType(id);
    }

    public async Task<List<PatientType>> GetAll(bool includeInactive)
    {
        return await _patientTypeUnitOfWork.GetPatientTypes(x => includeInactive || !x.Inactive);
    }

    public async Task<PatientType> Update(PatientType patientType)
    {
        return await _patientTypeUnitOfWork.Update(patientType);
    }

    public void Delete(int id)
    {
        _patientTypeUnitOfWork.Delete(id);
    }
}