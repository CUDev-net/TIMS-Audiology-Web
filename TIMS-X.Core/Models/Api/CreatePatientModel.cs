using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace TIMS_X.Core.Models.Api
{
    /*
     [Required] string firstName, [Required] string lastName, string email, string mobilePhone,
            string homePhone, DateTime? birthDate
     */
    public class CreatePatientModel
    {
        /// <summary>
        /// The first name of the patient
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of the patient
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// The email of the patient
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The mobile phone number of the patient
        /// </summary>
        public string MobilePhone { get; set; }

        /// <summary>
        /// The home phone number of the patient
        /// </summary>
        public string HomePhone { get; set; }

        /// <summary>
        /// The date of birth of the patient
        /// </summary>
        public DateTime BirthDate { get; set; }
    }
}
