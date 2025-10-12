using System;
using System.Collections.Generic;
using System.Text;
using TIMS_X.Core.Domain;

namespace TIMS_X.Core.Models
{
    public class SiteItem
    {
        public SiteItem() { }

        public SiteItem(Site site)
        {
            Id = site.Id;
            Inactive = site.Inactive;
            Color = site.Color;
            Name = site.Name;
            Address1 = site.Address1;
            Address2 = site.Address2;
            City = site.City;
            State = site.State;
            ZipCode = site.Zip;
            Phone = site.Phone;

            WeeklySchedule = new WeeklySchedule
            {
                SundayStart = site.SunStart,
                SundayEnd = site.SunEnd,
                MondayStart = site.MonStart,
                MondayEnd = site.MonEnd,
                TuesdayStart = site.TuesStart,
                TuesdayEnd = site.TuesEnd,
                WednesdayStart = site.WedStart,
                WednesdayEnd = site.WedEnd,
                ThursdayStart = site.ThurStart,
                ThursdayEnd = site.ThurEnd,
                FridayStart = site.FriStart,
                FridayEnd = site.FriEnd,
                SaturdayStart = site.SatStart,
                SaturdayEnd = site.SatEnd
            };
            Resources = site.Resources;
        }
        public int Id { get; set; }
        public bool Inactive { get; set; }

        public int? Color { get; set; }

        public string Name { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Phone { get; set; }

        public WeeklySchedule WeeklySchedule { get; set; }

        public ICollection<Resource> Resources { get; set; }

    }
}
