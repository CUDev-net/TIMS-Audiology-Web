using Microsoft.EntityFrameworkCore;
using Tims.Dal.Models;

namespace TIMS_X.DAL.DAL.ModelCreators;

internal class AppointmentItemSummaryModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppointmentItemSummary>(entity =>
        {
            entity.HasKey(i => i.id);

            entity.Ignore(i => i.appointment_type_web_color);
            entity.Ignore(i => i.provider_web_color);
            entity.Ignore(i => i.site_web_color);

            entity.ToTable("vw_web_scheduler_appointments");
        });
    }
}