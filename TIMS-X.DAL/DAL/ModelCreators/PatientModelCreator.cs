using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;

namespace TIMS_X.DAL.DAL.ModelCreators
{
    public class PatientModelCreator : IModelCreator
    {
        public void CreateModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
                entity.HasKey(i => i.Id);
                entity.Ignore(i => i.PendingDelete);
                entity.Ignore(i => i.HasStateBeenSet);
                entity.Ignore(i => i.HasBeenAudited);
                entity.Ignore(i => i.Guid);
				entity.Ignore(i => i.PatientTypeIds);
				entity.Ignore(i => i.RestrictionIds);
				entity.Ignore(i => i.AuthorizationIds);
				entity.Ignore(i => i.PatientTypeReferences);

				entity.HasIndex(e => new { e.Id, e.Inactive })
                    .HasDatabaseName("Pat_Active");

                entity.HasIndex(e => new { e.LastName, e.FirstName, e.Initial, e.Id, e.Inactive, e.SiteId })
                    .HasDatabaseName("Full Name")
                    .IsUnique();

                entity.Property(e => e.AccountNo).HasColumnName("AccountNum").HasMaxLength(35);
                entity.Property(e => e.Address1).HasColumnName("Addr1").HasMaxLength(30);
                entity.Property(e => e.Address2).HasColumnName("Addr2").HasMaxLength(30);
                entity.Property(e => e.City).HasMaxLength(20);

                entity.Property(e => e.ResponsibleParty).HasMaxLength(50);
                entity.Property(e => e.AlternateContact).HasColumnName("Contact").HasMaxLength(50);
                entity.Property(e => e.MaritalStatusId).HasColumnName("MaritalStatusID").HasMaxLength(2);
                entity.Property(e => e.EmplStatusId).HasColumnName("EmploymentStatusID").HasMaxLength(2);
                entity.Property(e => e.AlternateContactPhone).HasColumnName("ContactPhone").HasMaxLength(15);
                entity.Property(e => e.CreatedUserId).HasColumnName("CreatedByUserID");
                entity.Property(e => e.ProviderId).HasColumnName("DefaultProviderID");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("DtCreated")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("DtUpdated")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.HasIntakeData).HasColumnName("PCPForClaim");
                entity.Property(e => e.BirthDate).HasColumnName("DtOfBirth").HasColumnType("datetime");
                entity.Property(e => e.DeathDate).HasColumnName("DtOfDeath").HasColumnType("datetime");
                entity.Property(e => e.Email).HasMaxLength(99);
                entity.Property(e => e.FirstName).HasColumnName("FName").HasMaxLength(25);
                entity.Property(e => e.HomePhone).HasMaxLength(15);

                entity.Property(e => e.Initial)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnName("LName")
                    .HasMaxLength(35);

                entity.Property(e => e.LegalRepFirstName).HasColumnName("LegalRepFName").HasMaxLength(12);
                entity.Property(e => e.LegalRepLastName).HasColumnName("LegalRepLName").HasMaxLength(20);
                entity.Property(e => e.LegalRepInitial).HasMaxLength(1);
                entity.Property(e => e.LegalRepAddress1).HasColumnName("LegalRepAddr1").HasMaxLength(30);
                entity.Property(e => e.LegalRepAddress2).HasColumnName("LegalRepAddr2").HasMaxLength(30);
                entity.Property(e => e.LegalRepCity).HasMaxLength(20);
                entity.Property(e => e.LegalRepState).HasMaxLength(3);
                entity.Property(e => e.LegalRepZipCode).HasMaxLength(15);
                entity.Property(e => e.LegalRepPhone).HasMaxLength(15);

                entity.Property(e => e.Language).HasColumnName("PreferredLanguage");
                entity.Property(e => e.MarketingId).HasColumnName("MarketingID");
                entity.Property(e => e.CustomDate1).HasColumnName("MiscDt1").HasColumnType("datetime");
                entity.Property(e => e.CustomDate2).HasColumnName("MiscDt2").HasColumnType("datetime");
                entity.Property(e => e.CustomText1).HasColumnName("MiscText1").HasMaxLength(100);
                entity.Property(e => e.CustomText2).HasColumnName("MiscText2").HasMaxLength(100);
                entity.Property(e => e.MobilePhone).HasMaxLength(15);
                entity.Property(e => e.Notes).HasMaxLength(2500);
                entity.Property(e => e.OtStatusId).HasColumnName("OTStatus");

                entity.Property(e => e.PatientStatusId).HasColumnName("PatientStatusID");
                entity.Property(e => e.PatientTypeId).HasColumnName("PatientTypeID");
                entity.Property(e => e.PreferredName).HasMaxLength(25);
                entity.Property(e => e.PrimaryCareId).HasColumnName("PrimaryCareID");
                entity.Property(e => e.PrimaryPhone)
                    .HasConversion<int>()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.ReferringPhysicianId).HasColumnName("ReferralID");
                entity.Property(e => e.SalutationId).HasColumnName("SalutationID");
                entity.Property(e => e.Sex).HasMaxLength(2);
                entity.Property(e => e.SiteId).HasColumnName("SiteID");
                entity.Property(e => e.State).HasMaxLength(3);
                entity.Property(e => e.UpdatedUserId).HasColumnName("UID");
                entity.Property(e => e.UpdatedSiteId).HasColumnName("UpdatedSiteID");

                entity.Property(e => e.WorkPhone).HasMaxLength(15);
                entity.Property(e => e.OtherPhone).HasMaxLength(15);

                entity.Property(e => e.QBID).HasMaxLength(50);

                entity.Property(e => e.ZipCode).HasMaxLength(15);

                entity.Property(e => e.ReleaseSignature).HasColumnName("PatientReleaseSignature");

                entity.Property(e => e.ReleaseSignatureDate)
                    .HasColumnName("DtPatientSignature")
                    .HasColumnType("datetime");

                entity.Property(e => e.AssignBenefitsDate)
                    .HasColumnName("DtInsuredSignature")
                    .HasColumnType("datetime");

                entity.Property(e => e.AssignBenefits).HasColumnName("InsuredAssignmentSig");

                entity.Property(e => e.Ssn)
                    .HasColumnName("SSN")
                    .HasMaxLength(12);

                entity.Property(e => e.InsuredInsurancePayerId)
                    .HasColumnName("InsuredInsuranceCarrierID");

                entity.Property(e => e.SecondaryAddress1)
                    .HasColumnName("SecondaryAddr1");

                entity.Property(e => e.SecondaryAddress2)
                    .HasColumnName("SecondaryAddr2");

                entity.HasOne(e => e.Salutation)
                    .WithMany()
                    .HasForeignKey(e => e.SalutationId);

                entity.HasOne(e => e.Provider)
                    .WithMany()
                    .HasForeignKey(e => e.ProviderId);

                entity.HasOne(e => e.PrimaryCarePhysician)
                    .WithMany()
                    .HasForeignKey(e => e.PrimaryCareId);

                entity.HasOne(e => e.Marketing)
                    .WithMany()
                    .HasForeignKey(e => e.MarketingId);

                entity.HasOne(e => e.ReferringPhysician)
                    .WithMany()
                    .HasForeignKey(e => e.ReferringPhysicianId);

                entity.HasOne(e => e.UpdatedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.UpdatedUserId);
                
                entity.HasMany(e => e.Restrictions)
                    .WithOne()
                    .HasForeignKey(e => e.PatientId);

                entity.HasMany(e => e.MedicalConditions)
                    .WithOne()
                    .HasForeignKey(e => e.PatientId);

                entity.HasMany(e => e.AuthorizationReferences)
	                .WithOne()
	                .HasForeignKey(e => e.PatientId);

				entity.Property(e => e.PrivacyDate)
	                .HasColumnName("DtPrivacy")
	                .HasColumnType("datetime");

                entity.Property(e => e.ReleaseInformationDate)
	                .HasColumnName("DtReleaseInformation")
	                .HasColumnType("datetime");

                entity.Property(e => e.ConsentDate)
	                .HasColumnName("DtConsent")
	                .HasColumnType("datetime");

                entity.Property(e => e.MarketingAuthorizationDate)
	                .HasColumnName("DtMarketingAuthorization")
	                .HasColumnType("datetime");

                entity.Property(e => e.AuthorizedParties).HasMaxLength(2500);

                entity.Property(e => e.StudentStatusId)
	                .HasColumnName("StudentStatusID");

				//entity.HasMany(e => e.PatientInsurances)
				//    .WithOne()
				//    .HasForeignKey(e => e.PatientId);

				entity.ToTable("Patient");
            });
        }
    }
}