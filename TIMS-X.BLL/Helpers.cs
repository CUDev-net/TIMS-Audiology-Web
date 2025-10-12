using System.Collections.Generic;
using TIMS_X.DAL.Dtos;

namespace TIMS_X.BLL;

public static class Helpers
{
	public static List<MedicareSecondaryCodeDto> GetMedicareSecondaryCodes()
	{
        var list = new List<MedicareSecondaryCodeDto>
		{
			new MedicareSecondaryCodeDto() {Name = "12", Description = "Working Aged Beneficiary"},
			new MedicareSecondaryCodeDto() {Name = "13", Description = "End-Stage Renal Disease Beneficiary"},
			new MedicareSecondaryCodeDto() {Name = "14", Description = "No-fault Insurance"},
			new MedicareSecondaryCodeDto() {Name = "15", Description = "Worker’s Compensation"},
			new MedicareSecondaryCodeDto() {Name = "16", Description = "Public Health Service (PHS)or Other Federal Agency"},
			new MedicareSecondaryCodeDto() {Name = "41", Description = "Black Lung"},
			new MedicareSecondaryCodeDto() {Name = "42", Description = "Veteran’s Administration"},
			new MedicareSecondaryCodeDto() {Name = "43", Description = "Disabled Beneficiary Under Age 65"},
			new MedicareSecondaryCodeDto() {Name = "47", Description = "Other Liability Insurance is Primary"}
		};

		return list;
	}
    
	public static List<PatientRelationDto> GetPatientRelationTable()
    {
        var patRel = new List<PatientRelationDto>();
        var patientRelation1 = new PatientRelationDto
        {
            ID = 1,
            Name = "01",
            Description = "Self"
        };
        patRel.Add(patientRelation1);
        patientRelation1 = new PatientRelationDto
        {
            ID = 2,
            Name = "02",
            Description = "Spouse"
        };
        patRel.Add(patientRelation1);
        patientRelation1 = new PatientRelationDto
        {
            ID = 3,
            Name = "03",
            Description = "Child"
        };
        patRel.Add(patientRelation1);
        patientRelation1 = new PatientRelationDto
        {
            ID = 4,
            Name = "99",
            Description = "Other"
        };
        patRel.Add(patientRelation1);
        patientRelation1 = new PatientRelationDto
        {
            ID = 5,
            Name = "10",
            Description = "Self-Other"
        };
        patRel.Add(patientRelation1);
        return patRel;
    }
}