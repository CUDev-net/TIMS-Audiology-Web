using System.Collections.Generic;
using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IDiagnosisCodeRepository
{
    Task<DiagnosisCode> Add(DiagnosisCode diagnosisCode);
    void Delete(int id);
    Task<DiagnosisCode> Get(int id);
    Task<List<DiagnosisCode>> GetAll(bool includeInactive);
    Task<DiagnosisCode> Update(DiagnosisCode diagnosisCode);
}

public class DiagnosisCodeRepository : IDiagnosisCodeRepository
{
    private readonly IDiagnosisCodeUnitOfWork _diagnosisCodeUnitOfWork;

    public DiagnosisCodeRepository(IDiagnosisCodeUnitOfWork diagnosisCodeUnitOfWork)
    {
        _diagnosisCodeUnitOfWork = diagnosisCodeUnitOfWork;
    }

    public async Task<DiagnosisCode> Add(DiagnosisCode diagnosisCode)
    {
        return await _diagnosisCodeUnitOfWork.Add(diagnosisCode);
    }

    public void Delete(int id)
    {
        _diagnosisCodeUnitOfWork.Delete(id);
    }

    public async Task<DiagnosisCode> Get(int id)
    {
        return await _diagnosisCodeUnitOfWork.GetDiagnosisCode(id);
    }

    public async Task<List<DiagnosisCode>> GetAll(bool includeInactive)
    {
        return await _diagnosisCodeUnitOfWork.GetDiagnosisCodes(x => includeInactive || !x.Inactive);
    }

    public async Task<DiagnosisCode> Update(DiagnosisCode diagnosisCode)
    {
        return await _diagnosisCodeUnitOfWork.Update(diagnosisCode);
    }
}