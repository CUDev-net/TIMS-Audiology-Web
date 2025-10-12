using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IPatientInsuranceRepository
{
    Task<PatientInsurance> Add(PatientInsurance patientInsurance);
    void Delete(int id);
    Task<PatientInsurance> Get(int id);
    Task<List<PatientInsurance>> GetAllForPatient(int patientid);
    Task<PatientInsurance> Update(PatientInsurance patientInsurance);
}

public class PatientInsuranceRepository : IPatientInsuranceRepository
{
    private readonly IPatientInsuranceUnitOfWork _PatientInsuranceUnitOfWork;

    public PatientInsuranceRepository(IPatientInsuranceUnitOfWork patientInsuranceUnitOfWork)
    {
        _PatientInsuranceUnitOfWork = patientInsuranceUnitOfWork;
    }

    public async Task<PatientInsurance> Add(PatientInsurance patientInsurance)
    {
	    patientInsurance.FirstName ??= string.Empty;
	    patientInsurance.LastName ??= string.Empty;
		return await _PatientInsuranceUnitOfWork.Add(patientInsurance);
    }

    public void Delete(int id)
    {
        _PatientInsuranceUnitOfWork.Delete(id);
    }

    public async Task<PatientInsurance> Get(int id)
    {
        return await _PatientInsuranceUnitOfWork.GetPatientInsurance(id);
    }

    public async Task<List<PatientInsurance>> GetAllForPatient(int patientId)
    {
        return await _PatientInsuranceUnitOfWork.GetPatientInsurancees(x => x.PatientId == patientId);
    }

    public async Task<PatientInsurance> Update(PatientInsurance patientInsurance)
    {
        patientInsurance.FirstName ??= string.Empty;
        patientInsurance.LastName ??= string.Empty;
		return await _PatientInsuranceUnitOfWork.Update(patientInsurance);
    }
}