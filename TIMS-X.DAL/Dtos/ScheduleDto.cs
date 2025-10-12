using System.Collections.Generic;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.Dtos
{
    public class ScheduleDto
    {
        public ScheduleDto(Schedule schedule)
        {
            Schedule = schedule;
        }

        public string Provider_Color { get; set; }
        public string ProviderName { get; set; }
        public Schedule Schedule { get; }
        public ScheduleRecurringItemSummary RecurringItemSummary { get; set; }
        public string Site_Color { get; set; }
        public string SiteName { get; set; }
        public ICollection<ScheduleProviderDto> Providers { get; set; }
    }

    public class ScheduleProviderDto
    {
        public int Id { get; set; }
        public string Color { get; set; }
        public string Name { get; set; }
    }
}