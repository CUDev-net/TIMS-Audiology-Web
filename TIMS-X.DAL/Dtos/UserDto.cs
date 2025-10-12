using System.Collections.Generic;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.Dtos
{
    public class UserDto
    {
        public UserDto(User user)
        {
            User = user;
            LastPatientSummaries = new List<PatientSummary>();
        }

        public User User { get; }

        public List<PatientSummary> LastPatientSummaries { get; set; }
    }
}
