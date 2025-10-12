using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IPatientStatusRepository
{
    Task<PatientStatus> Add(PatientStatus patientStatus);
    void Delete(int id);
    Task<PatientStatus> Get(int id);
    Task<List<PatientStatus>> GetAll(bool includeInactive);
    Task<PatientStatus> Update(PatientStatus patientStatus);
}

public class PatientStatusRepository : IPatientStatusRepository
{
    private readonly IPatientStatusUnitOfWork _patientStatusUnitOfWork;

    public PatientStatusRepository(IPatientStatusUnitOfWork patientStatusUnitOfWork)
    {
        _patientStatusUnitOfWork = patientStatusUnitOfWork;
    }

    public async Task<PatientStatus> Add(PatientStatus patientStatus)
    {
        return await _patientStatusUnitOfWork.Add(patientStatus);
    }

    public async Task<PatientStatus> Get(int id)
    {
        return await _patientStatusUnitOfWork.GetPatientStatus(id);
    }

    public async Task<List<PatientStatus>> GetAll(bool includeInactive)
    {
        return await _patientStatusUnitOfWork.GetPatientStatuses(x => includeInactive || !x.Inactive);
    }

    public async Task<PatientStatus> Update(PatientStatus patientStatus)
    {
        return await _patientStatusUnitOfWork.Update(patientStatus);
    }

    public void Delete(int id)
    {
        _patientStatusUnitOfWork.Delete(id);
    }
}