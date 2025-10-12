using System;
using System.Collections.Generic;
using System.Text;
using TIMS_X.Core.Domain;
using TIMS_X.Core.Enums;

namespace TIMS_X.Core.Models
{
    public class PatientItem
    {
        public PatientItem()
        {

        }
        public PatientItem(Patient patient)
        {
            Id = patient.Id;
            SiteId = patient.SiteId;
            Inactive = patient.Inactive;
            FirstName = patient.FirstName;
            LastName = patient.LastName;
            Initial = patient.Initial;
            Gender = patient.Sex == "M" ? Gender.Male : (patient.Sex == "F" ? Gender.Female : Gender.Unknown);
            BirthDate = patient.BirthDate;
            
        }

        public int Id { get; set; }
        public Gender Gender { get; set; }
        public int? SiteId { get; set; }
        public bool Inactive { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Initial { get; set; }
        public DateTime? BirthDate { get; set; }

        public string Name => LastName + ", " + FirstName + (string.IsNullOrEmpty(Initial) ? string.Empty : " " + Initial + ".");

        public string NameAndInactive => LastName + ", " + FirstName + (string.IsNullOrEmpty(Initial) ? string.Empty : " " + Initial + ".") + (Inactive? " (Inactive)" : string.Empty);

        public string IdAndDob => $"(ID: {Id}, DOB: {(BirthDate.HasValue ? BirthDate.Value.ToString("MM/dd/yyyy") : "NONE")}{(Inactive ? ", Inactive" : string.Empty)})";
        public string NameAndDob =>
            $"{Name} (ID: {Id}, DOB: {(BirthDate.HasValue ? BirthDate.Value.ToString("MM/dd/yyyy") : "NONE")}{(Inactive ? ", Inactive" : string.Empty)})";

    }
}
