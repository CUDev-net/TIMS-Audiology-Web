using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Enums;
using TIMS_X.Core.Utils;

namespace TIMS_X.Core.Models
{
    public class DetailedPatientItem
    {
        public DetailedPatientItem()
        {

        }
        public DetailedPatientItem(Patient patient)
        {
            Id = patient.Id;
            SiteId = patient.SiteId;
            Inactive = patient.Inactive;
            FirstName = patient.FirstName;
            LastName = patient.LastName;
            Initial = patient.Initial;

            BirthDate = patient.BirthDate;
            MarketingSource = patient.MarketingId.ToString();
            DeceasedDate = patient.DeathDate;
            AccountNo = patient.AccountNo;
            Address1 = patient.Address1;
            Address2 = patient.Address2;
            City = patient.City;
            State = patient.State;
            ZipCode = patient.ZipCode;
            Email = patient.Email;
            HomePhone = patient.HomePhone;
            MobilePhone = patient.MobilePhone;
            Gender = patient.Sex == "M" ? "Male" : (patient.Sex == "F" ? "Female" : string.Empty);
            Race = EnumUtilities.GetDescriptionFromEnum(patient.Race);
            Language = EnumUtilities.GetDescriptionFromEnum(patient.Language);
            Ethnicity = EnumUtilities.GetDescriptionFromEnum(patient.Ethnicity);
            LegalRepAddress1 = patient.LegalRepAddress1;
            LegalRepAddress2 = patient.LegalRepAddress1;
            LegalRepCity = patient.LegalRepCity;
            LegalRepState = patient.LegalRepState;
            LegalRepZipCode = patient.LegalRepZipCode;
            LegalRepPhone = patient.LegalRepPhone;
            AlternateContact = patient.AlternateContact;
            AlternateContactPhone = patient.AlternateContactPhone;
            Restrictions = patient.Restrictions.Select(r => r.CommunicationRestriction.Name).ToList();
            Notes = patient.Notes;
        }

        public int Id { get; set; }
        public int? SiteId { get; set; }
        public bool Inactive { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Initial { get; set; }
        public DateTime? BirthDate { get; set; }
        
        public string MarketingSource { get; set; }
        public List<string> Restrictions { get; set; }

        public DateTime? DeceasedDate { get; set; }
        public string AccountNo { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Email { get; set; }
        public string HomePhone { get; set; }
        public string MobilePhone { get; set; }
        
        public string Gender { get; set; }
        public string Language { get; set; }
        public string Race { get; set; }
        public string Ethnicity { get; set; }

        public string LegalRepFirstName { get; set; }
        public string LegalRepLastName { get; set; }
        public string LegalRepInitial { get; set; }
        public string LegalRepAddress1 { get; set; }
        public string LegalRepAddress2 { get; set; }
        public string LegalRepCity { get; set; }
        public string LegalRepState { get; set; }
        public string LegalRepZipCode { get; set; }
        public string LegalRepPhone { get; set; }

        public string AlternateContact { get; set; }
        public string AlternateContactPhone { get; set; }

        public string Notes { get; set; }

    }
}
