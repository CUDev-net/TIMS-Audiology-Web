using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IMedicationRepository
{
    Task<Medication> Add(Medication medication);
    void Delete(int id);
    Task<Medication> Get(int id);
    Task<List<Medication>> GetAll(bool includeInactive);
    Task<Medication> Update(Medication medication);
}

public class MedicationRepository : IMedicationRepository
{
    private readonly IMedicationUnitOfWork _medicationUnitOfWork;

    public MedicationRepository(IMedicationUnitOfWork medicationUnitOfWork)
    {
        _medicationUnitOfWork = medicationUnitOfWork;
    }

    public async Task<Medication> Add(Medication medication)
    {
        return await _medicationUnitOfWork.Add(medication);
    }

    public void Delete(int id)
    {
        _medicationUnitOfWork.Delete(id);
    }

    public async Task<Medication> Get(int id)
    {
        return await _medicationUnitOfWork.GetMedication(id);
    }

    public async Task<List<Medication>> GetAll(bool includeInactive)
    {
        return await _medicationUnitOfWork.GetMedications(x => includeInactive || !x.Inactive);
    }

    public async Task<Medication> Update(Medication medication)
    {
        return await _medicationUnitOfWork.Update(medication);
    }
}