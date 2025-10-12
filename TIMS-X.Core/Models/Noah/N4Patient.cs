using System;
using System.Collections.Generic;
using System.Text;

namespace TIMS_X.Core.Models.Noah
{
    public class N4Patient
    {
        public int Id { get; set; }
        public Guid PatientGuid { get; set; }
        public short ActivePatient { get; set; }
        public string PatientNo { get; set; }
        public DateTime DateCreated { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public short Gender { get; set; }
        public DateTime Birthdate { get; set; }
        public string Salutation { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string State { get; set; }
        public string Title { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string HomePhone { get; set; }
        public string WorkPhone { get; set; }
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        public string Ssn { get; set; }
        public string Insurance1 { get; set; }
        public string Insurance2 { get; set; }
        public string Other1 { get; set; }
        public string Other2 { get; set; }
        public string Physician { get; set; }
        public string Referral { get; set; }
        public string CreatedBy { get; set; }
        public int SessionCount { get; set; }
    }
}
