using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators;

internal class HistoryModelCreator : IModelCreator
{
    public void CreateModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<History>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.HasKey(i => i.Id);
            entity.Ignore(i => i.PendingDelete);
            entity.Ignore(i => i.HasStateBeenSet);

            entity.HasIndex(e => e.AppointmentId);

            entity.HasIndex(e => e.HistoryGuid)
                .HasDatabaseName("uq_History_HistoryGUID")
                .IsUnique();

            entity.HasIndex(e => e.PatientId);

            entity.HasIndex(e => new { e.Id, e.PatientId, e.HistoryDate })
                .HasDatabaseName("Tree View");

            entity.Property(e => e.Id).HasColumnName("ID");

            entity.Property(e => e.ActionId).HasColumnName("ActionID");

            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");

            entity.Property(e => e.AvailableDate)
                .HasColumnName("DtAvailable")
                .HasColumnType("datetime");

            entity.Property(e => e.CreatedDate)
                .HasColumnName("DtCreated")
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getutcdate())");

            entity.Property(e => e.HistoryDate)
                .HasColumnName("DtHistory")
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.UpdatedDate)
                .HasColumnName("DtUpdated")
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.ExportDate).HasColumnType("datetime");

            entity.Property(e => e.HistoryGuid)
                .HasColumnName("HistoryGUID")
                .HasDefaultValueSql("(newsequentialid())");

            entity.Property(e => e.HistoryTypeId).HasColumnName("HistoryTypeID");

            entity.HasOne(h => h.HistoryType)
                .WithMany()
                .HasForeignKey(h => h.HistoryTypeId);

            entity.HasOne(a => a.Appointment)
                .WithMany()
                .HasForeignKey(a => a.AppointmentId);

            entity.HasOne(s => s.Site)
                .WithMany()
                .HasForeignKey(s => s.SyncSiteId);

            entity.HasOne(p => p.Provider)
                .WithMany()
                .HasForeignKey(p => p.ProviderId);

            entity.HasOne(m => m.MarketingReference)
                .WithMany()
                .HasForeignKey(r => r.ReferralSourceId);

            entity.Property(e => e.LockDate).HasColumnType("datetime");

            entity.Property(e => e.ParentId).HasColumnName("ParentID");

            entity.Property(e => e.PatientId).HasColumnName("PatID");

            entity.HasOne(p => p.Patient)
                .WithMany()
                .HasForeignKey(p => p.PatientId);

            entity.Property(e => e.PatientInteractionId).HasColumnName("PatientInteractionID");

            entity.Property(e => e.ProviderId).HasColumnName("ProviderID");

            entity.Property(e => e.ReferralSourceId).HasColumnName("ReferralSourceID");

            entity.Property(e => e.RowVersion)
                .IsRequired()
                .HasColumnName("rowVersion")
                .IsRowVersion();

            entity.Property(e => e.SlpArticulation).HasColumnName("SLPArticulation");

            entity.Property(e => e.SlpAttendingSkills).HasColumnName("SLPAttendingSkills");

            entity.Property(e => e.SlpAwarenessOfOthers).HasColumnName("SLPAwarenessOfOthers");

            entity.Property(e => e.SlpCommunicativeIntent).HasColumnName("SLPCommunicativeIntent");

            entity.Property(e => e.SlpCooperation).HasColumnName("SLPCooperation");

            entity.Property(e => e.SlpDiagnosis).HasColumnName("SLPDiagnosis");

            entity.Property(e => e.SlpEnvironmentalAwareness).HasColumnName("SLPEnvironmentalAwareness");

            entity.Property(e => e.SlpExpressiveLanguage).HasColumnName("SLPExpressiveLanguage");

            entity.Property(e => e.SlpFluency).HasColumnName("SLPFluency");

            entity.Property(e => e.SlpFluency2).HasColumnName("SLPFluency2");

            entity.Property(e => e.SlpFluencyVoiceNotes)
                .HasColumnName("SLPFluencyVoiceNotes")
                .HasMaxLength(250);

            entity.Property(e => e.SlpGoals).HasColumnName("SLPGoals");

            entity.Property(e => e.SlpGoalsStatus).HasColumnName("SLPGoalsStatus");

            entity.Property(e => e.SlpLevelOfActivity).HasColumnName("SLPLevelOfActivity");

            entity.Property(e => e.SlpPragmatics).HasColumnName("SLPPragmatics");

            entity.Property(e => e.SlpPrognosis).HasColumnName("SLPPrognosis");

            entity.Property(e => e.SlpProgressNotes).HasColumnName("SLPProgressNotes");

            entity.Property(e => e.SlpReceptiveLanguage).HasColumnName("SLPReceptiveLanguage");

            entity.Property(e => e.SlpRecommendationNotes).HasColumnName("SLPRecommendationNotes");

            entity.Property(e => e.SlpReliabilityOfScores).HasColumnName("SLPReliabilityOfScores");

            entity.Property(e => e.SlpResponseRate).HasColumnName("SLPResponseRate");

            entity.Property(e => e.SlpSocialInteractions).HasColumnName("SLPSocialInteractions");

            entity.Property(e => e.SlpVoice).HasColumnName("SLPVoice");

            entity.Property(e => e.SlpVoice2).HasColumnName("SLPVoice2");

            entity.Property(e => e.SyncSiteId).HasColumnName("SyncSiteID");

            entity.Property(e => e.UpdatedUserId).HasColumnName("UID");

            entity.Property(e => e.UpdatedSiteId).HasColumnName("UpdatedSiteID");

            entity.ToTable(nameof(History));
        });
    }
}