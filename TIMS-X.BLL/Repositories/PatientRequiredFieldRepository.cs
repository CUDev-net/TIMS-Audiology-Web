using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IPatientRequiredFieldRepository
{
    Task<List<PatientRequiredField>> GetRequired();
}

public class PatientRequiredFieldRepository : IPatientRequiredFieldRepository
{
    private readonly IPatientRequiredFieldUnitOfWork _patientRequiredFieldUnitOfWork;

    public PatientRequiredFieldRepository(IPatientRequiredFieldUnitOfWork patientRequiredFieldUnitOfWork)
    {
        _patientRequiredFieldUnitOfWork = patientRequiredFieldUnitOfWork;
    }

    public async Task<List<PatientRequiredField>> GetRequired()
    {
        return await _patientRequiredFieldUnitOfWork.GetPatientRequiredFields(x => x.Required)
            .ToListAsync();
    }
}