using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators
{
    public class PatientInsuranceModelCreator : IModelCreator
    {
        public void CreateModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PatientInsurance>(entity =>
            {
				entity.HasIndex(e => e.PatientId, "IX_PatientInsurance_PatientID");

				entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
				entity.HasKey(i => i.Id);
				entity.Ignore(i => i.PendingDelete);
				entity.Ignore(i => i.HasStateBeenSet);
				entity.Ignore(i => i.UpdatedDate);

				entity.Property(e => e.Address1).HasMaxLength(55);

				entity.Property(e => e.Address2).HasMaxLength(55);

				entity.Property(e => e.BirthDate).HasColumnType("datetime");

				entity.Property(e => e.CarrierCode).HasMaxLength(128);

				entity.Property(e => e.City).HasMaxLength(30);

				entity.Property(e => e.ClaimFilingIndicator).HasMaxLength(50);

				entity.Property(e => e.CreatedDate).HasColumnName("DateCreated").HasColumnType("datetime");

				entity.Property(e => e.Employer).HasMaxLength(33);

				entity.Property(e => e.EmploymentStatus).HasMaxLength(2);

				entity.Property(e => e.FirstName)
					.IsRequired()
					.HasMaxLength(35);

				entity.Property(e => e.IdNumber)
					.HasMaxLength(50)
					.HasColumnName("IDNumber");

				entity.Property(e => e.InsurancePayerId).HasColumnName("InsuranceCarrierID");

				entity.Property(e => e.LastName)
					.IsRequired()
					.HasMaxLength(60);

				entity.Property(e => e.MiddleName).HasMaxLength(25);

				entity.Property(e => e.PatientId).HasColumnName("PatientID");

				entity.Property(e => e.PatientSignatureDate).HasColumnType("datetime");

				entity.Property(e => e.Phone).HasMaxLength(15);

				entity.Property(e => e.PolicyGroupName).HasMaxLength(50);

				entity.Property(e => e.PolicyGroupNum).HasMaxLength(50);

				entity.Property(e => e.PolicyType).HasMaxLength(50);

				entity.Property(e => e.RelationtoInsured).HasMaxLength(25);

				entity.Property(e => e.RetireDate).HasColumnType("datetime");

				entity.Property(e => e.Sex).HasMaxLength(2);

				entity.Property(e => e.SignatureDate).HasColumnType("datetime");

				entity.Property(e => e.State).HasMaxLength(3);

				entity.Property(e => e.WorkPhone).HasMaxLength(15);

				entity.Property(e => e.ZipCode).HasMaxLength(15);

				entity.HasOne(e => e.InsurancePayer)
				   .WithMany()
                    .HasForeignKey(e => e.InsurancePayerId)
                    .IsRequired(false);

				entity.Property(e => e.UpdatedUserId).HasColumnName("LastModifiedBy");

				entity.ToTable(nameof(PatientInsurance));
			});
        }
    }
}