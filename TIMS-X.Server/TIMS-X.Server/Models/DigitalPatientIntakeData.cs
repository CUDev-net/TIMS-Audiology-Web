using System;
using TIMS_X.Core.Enums;

namespace TIMS_X.Server.Models;

public class DigitalPatientIntakeData
{
	public bool AssignBenefits { get; set; }
	public DateTime? AssignBenefitsDate { get; set; }
	public string Contact { get; set; }
	public string ContactPhone { get; set; }
	public bool EmailNotifications { get; set; }
	public string EmplStatusId { get; set; }
	public EthnicityEnum? Ethnicity { get; set; }

	public byte[] FormScreenshot { get; set; }
	public string InsAddress1 { get; set; }
	public string InsAddress2 { get; set; }
	public string InsCity { get; set; }

	// Primary Insurance
	public string InsCoName { get; set; }
	public DateTime? InsDtOfBirth { get; set; }
	public string InsFirstName { get; set; }
	public string InsGroupNum { get; set; }
	public string InsIdNum { get; set; }
	public string InsInitial { get; set; }
	public string InsLastName { get; set; }
	public string InsPhone { get; set; }
	public string InsSex { get; set; }
	public string InsState { get; set; }
	public string InsZip { get; set; }
	public LanguageEnum? Language { get; set; }
	public string MaritalStatusId { get; set; }

	public string PatientAddr1 { get; set; }
	public string PatientAddr2 { get; set; }
	public string PatientCity { get; set; }
	public DateTime? PatientDtOfBirth { get; set; }
	public string PatientEmail { get; set; }
	public string PatientFirstName { get; set; }
	public string PatientHomePhone { get; set; }
	public int PatientId { get; set; }
	public string PatientInitial { get; set; }
	public string PatientLastName { get; set; }
	public string PatientMobilePhone { get; set; }
	public string PatientOtherPhone { get; set; }

	public PrimaryPhoneEnum PatientPrimaryPhone { get; set; }
	public string PatientState { get; set; }
	public string PatientWorkPhone { get; set; }
	public string PatientZip { get; set; }
	public RaceEnum? Race { get; set; }
	public string RefPhysician { get; set; }
	public string RelationToInsured { get; set; }

	public bool ReleaseSignature { get; set; }
	public DateTime? ReleaseSignatureDate { get; set; }

	public string ResponsibleParty { get; set; }
	public string SecInsAddress1 { get; set; }
	public string SecInsAddress2 { get; set; }
	public string SecInsCity { get; set; }

	// Secondary Insurance
	public string SecInsCoName { get; set; }
	public DateTime? SecInsDtOfBirth { get; set; }
	public string SecInsFirstName { get; set; }
	public string SecInsGroupNum { get; set; }
	public string SecInsIdNum { get; set; }
	public string SecInsInitial { get; set; }
	public string SecInsLastName { get; set; }
	public string SecInsPhone { get; set; }
	public string SecInsSex { get; set; }
	public string SecInsState { get; set; }
	public string SecInsZip { get; set; }
	public string SecRelationToInsured { get; set; }
	public string Sex { get; set; }
	public bool TextNotifications { get; set; }
	public bool VoiceNotifications { get; set; }
}