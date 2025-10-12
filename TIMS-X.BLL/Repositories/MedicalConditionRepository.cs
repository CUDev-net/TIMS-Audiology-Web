using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IMedicalConditionRepository
{
    Task<MedicalCondition> Add(MedicalCondition medicalCondition);
    void Delete(int id);
    Task<MedicalCondition> Get(int id);
    Task<List<MedicalCondition>> GetAll(bool includeInactive);
    Task<MedicalCondition> Update(MedicalCondition medicalCondition);
}

public class MedicalConditionRepository : IMedicalConditionRepository
{
    private readonly IMedicalConditionUnitOfWork _medicalConditionUnitOfWork;

    public MedicalConditionRepository(IMedicalConditionUnitOfWork medicalConditionUnitOfWork)
    {
        _medicalConditionUnitOfWork = medicalConditionUnitOfWork;
    }

    public async Task<MedicalCondition> Add(MedicalCondition medicalCondition)
    {
        return await _medicalConditionUnitOfWork.Add(medicalCondition);
    }

    public async Task<MedicalCondition> Get(int id)
    {
        return await _medicalConditionUnitOfWork.GetMedicalCondition(id);
    }

    public async Task<List<MedicalCondition>> GetAll(bool includeInactive)
    {
        return await _medicalConditionUnitOfWork.GetMedicalConditions(x => includeInactive || !x.Inactive);
    }

    public async Task<MedicalCondition> Update(MedicalCondition medicalCondition)
    {
        return await _medicalConditionUnitOfWork.Update(medicalCondition);
    }

    public void Delete(int id)
    {
        _medicalConditionUnitOfWork.Delete(id);
    }
}