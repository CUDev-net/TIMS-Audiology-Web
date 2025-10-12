using System;
using System.Reflection;

using Microsoft.EntityFrameworkCore;

using TIMS_X.Core.Domain;
using TIMS_X.Core.Domain.Imaging;
using TIMS_X.Core.Domain.Noah;
using TIMS_X.Core.Enums;
using TIMS_X.Server.Exceptions;

using Version = TIMS_X.Core.Domain.Version;

namespace TIMS_X.Server.Config
{
	public static class ModelConfigurator
	{
		#region ModelConfigurator Members

		public static void Configure( ModelBuilder modelBuilder, Type type )
		{
			// use reflection to invoke the correct configuration method.
			var configureMethod = typeof( ModelConfigurator ).GetMethod( $"Configure{type.Name}",
				BindingFlags.Public | BindingFlags.Static );

			if( configureMethod != null )
			{
				// for static methods, pass null for obj parameter
				configureMethod.Invoke( null, new[] { modelBuilder } );
			}
			else
			{
				throw new UnknownModelException( type );
			}
		}

		public static void ConfigureAlert( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<Alert>( entity =>
			 {
				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.AlertObjectId ).HasColumnName( "AlertTypeID" );

				 entity.Property( e => e.AlertUserId ).HasColumnName( "AlertUserID" );

				 entity.Property( e => e.CreatedUserId ).HasColumnName( "CreatedByUserID" );

				 entity.Property( e => e.CreatedDate ).HasColumnType( "datetime" );

				 entity.Property( e => e.DueDate ).HasColumnType( "datetime" );

				 entity.Property( e => e.Name )
					 .IsRequired()
					 .HasMaxLength( 25 );

				 entity.ToTable( nameof( Alert ) );
			 } );
		}

		public static void ConfigureAppointment( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<Appointment>( entity =>
			 {
				//entity.HasIndex(e => e.Guid)
				//    .HasName("uq__Appointment_AppointmentGUID")
				//    .IsUnique();
				entity.HasIndex( e => e.ProviderId );

				 entity.HasIndex( e => e.SiteId );

				 entity.HasIndex( e => new { e.Id, e.SiteId, e.PatientId, e.ProviderId } )
					 .HasDatabaseName( "Appt_QBSync" );


                 entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
                 entity.HasKey(i => i.Id);
                 entity.Ignore(i => i.PendingDelete);
                 entity.Ignore(i => i.HasStateBeenSet);
                 entity.Ignore(i => i.OpportunityDescription);
                 entity.Ignore(i => i.UpdatedByUserName);

                 //entity.Property(e => e.Guid)
                 //    .HasColumnName("AppointmentGUID")
                 //    .HasDefaultValueSql("(newsequentialid())")
                 //    .ValueGeneratedOnAdd();
                 entity.Property( e => e.EndsAt )
					 .HasColumnName( "ApptEnd" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.StartsAt )
					 .HasColumnName( "ApptStart" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.AppointmentStatusId ).HasColumnName( "ApptStatusID" );

				 entity.Property( e => e.AppointmentTypeId ).HasColumnName( "ApptTypeID" );

				//entity.Property(e => e.AuthorizationId).HasColumnName("AuthorizationID");
				//entity.Property(e => e.BillToProviderId).HasColumnName("BillToProviderID");
				entity.Property( e => e.CreatedUserId ).HasColumnName( "CreatedByUserID" );

				 entity.Property( e => e.CreatedDate )
					 .HasColumnName( "DtCreated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.NextContactDate ).HasColumnName( "DtNextContact" ).HasColumnType( "datetime" );

				 entity.Property( e => e.UpdatedDate )
					 .HasColumnName( "DtUpdated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				//entity.Property(e => e.FacilityId).HasColumnName("FacilityID");
				entity.Property( e => e.MarketingId ).HasColumnName( "MarketingID" );

				 entity.Property( e => e.Notes ).HasColumnType( "nvarchar(max)" );

				 entity.Property( e => e.OtStatus ).HasColumnName( "OTStatus" );

				 entity.Property( e => e.PatientId ).HasColumnName( "PatID" );

				 entity.Property( e => e.ProviderId ).HasColumnName( "ProviderID" );

				 entity.Property( e => e.RecurringIntervalId ).HasColumnName( "RecurringIntervalID" );

				 entity.Property( e => e.RecurringParentId ).HasColumnName( "RecurringParentID" );

				 entity.Property( e => e.ReferringPhysicianId ).HasColumnName( "ReferralSourceID" );

				 entity.Property( e => e.ResourceId ).HasColumnName( "ResourceID" );

				//entity.Property(e => e.RowVersion)
				//    .IsRequired()
				//    .HasColumnName("rowVersion")
				//    .IsRowVersion();
				entity.Property( e => e.SiteId ).HasColumnName( "SiteID" );

				//entity.Property(e => e.SupervisingId).HasColumnName("SupervisingID");
				entity.Property( e => e.SyncSiteId ).HasColumnName( "SyncSiteID" );

				//entity.Property(e => e.Title).HasMaxLength(35);
				entity.Property( e => e.UpdatedUserId ).HasColumnName( "UID" );

				 entity.Property( e => e.UpdatedSiteId ).HasColumnName( "UpdatedSiteID" );

				 entity.HasOne( e => e.AppointmentType )
					 .WithMany()
					 .HasForeignKey( e => e.AppointmentTypeId );

				 entity.HasOne( e => e.AppointmentStatus )
					 .WithMany()
					 .HasForeignKey( e => e.AppointmentStatusId );

				 entity.HasOne( e => e.Patient )
					 .WithMany()
					 .HasForeignKey( e => e.PatientId );

				 entity.HasOne( e => e.Provider )
					 .WithMany()
					 .HasForeignKey( e => e.ProviderId );

				 entity.HasOne( e => e.Site )
					 .WithMany()
					 .HasForeignKey( e => e.SiteId );

                 entity.HasMany(e => e.PosDocuments)
					.WithOne()
					.HasForeignKey(e => e.AppointmentId);

                 entity.HasOne( e => e.RecurringInterval )
					 .WithMany()
					 .HasForeignKey( e => e.RecurringIntervalId );

				 entity.ToTable( "Appointment" );

			 } );
		}

		public static void ConfigureAppointmentStatus( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<AppointmentStatus>( entity =>
			 {
                 entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
                 entity.HasKey(i => i.Id);
                 entity.Ignore(i => i.PendingDelete);
                 entity.Ignore(i => i.HasStateBeenSet);

                 entity.Property( e => e.Description ).HasMaxLength( 50 );

				 entity.Property( e => e.UpdatedDate )
					 .HasColumnName( "DtUpdated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.Name )
					 .IsRequired()
					 .HasMaxLength( 50 );

				 entity.Property( e => e.Show )
					 .IsRequired()
					 .HasDefaultValueSql( "((1))" );

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UID" );
				 entity.ToTable( "AppointmentStatus" );
			 } );
		}

		public static void ConfigureAppointmentType( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<AppointmentType>( entity =>
			 {
                 entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
                 entity.HasKey(i => i.Id);
                 entity.Ignore(i => i.PendingDelete);
                 entity.Ignore(i => i.HasStateBeenSet);
                 entity.Ignore(i => i.CreatedDate);

                 entity.Property( e => e.Description ).HasMaxLength( 50 );

				 entity.Property( e => e.UpdatedDate )
					 .HasColumnName( "DtUpdated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.Duration ).HasDefaultValueSql( "((30))" );

				 entity.Property( e => e.HistoryTypeId ).HasColumnName( "HistoryTypeID" );

				 entity.Property( e => e.Name )
					 .IsRequired()
					 .HasMaxLength( 50 );

				 entity.Property( e => e.ScheduleBlockId ).HasColumnName( "ScheduleBlockID" );

				 entity.Property( e => e.Slp ).HasColumnName( "SLP" );

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UID" );
				 entity.ToTable( "AppointmentType" );
			 } );
		}

		public static void ConfigureApptRecurringInterval( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<ApptRecurringInterval>( entity =>
			 {
                 entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
                 entity.HasKey(i => i.Id);
                 entity.Ignore(i => i.PendingDelete);
                 entity.Ignore(i => i.HasStateBeenSet);

                 entity.Property( e => e.EndDate )
					 .HasColumnName( "DtEnd" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.StartDate )
					 .HasColumnName( "DtStart" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.DayInterval ).HasColumnName( "T1Days" );

				 entity.Property( e => e.IsFridaySet ).HasColumnName( "T2Friday" );

				 entity.Property( e => e.IsMondaySet ).HasColumnName( "T2Monday" );

				 entity.Property( e => e.IsSaturdaySet ).HasColumnName( "T2Saturday" );

				 entity.Property( e => e.IsSundaySet ).HasColumnName( "T2Sunday" );

				 entity.Property( e => e.IsThursdaySet ).HasColumnName( "T2Thursday" );

				 entity.Property( e => e.IsTuesdaySet ).HasColumnName( "T2Tuesday" );

				 entity.Property( e => e.IsWednesdaySet ).HasColumnName( "T2Wednesday" );

				 entity.Property( e => e.WeekInterval ).HasColumnName( "T2WeekCnt" );

				 entity.Property( e => e.DayOfMonth ).HasColumnName( "T34DayNum" );

				 entity.Property( e => e.DayQualifier ).HasColumnName( "T34DayQualID" );

				 entity.Property( e => e.DayOfWeek ).HasColumnName( "T34DayTypeID" );

				 entity.Property( e => e.MonthInterval ).HasColumnName( "T3MonthCnt" );

				 entity.Property( e => e.Month ).HasColumnName( "T4MonthID" );

				 entity.HasMany( e => e.DeletedOccurrences )
					 .WithOne()
					 .HasForeignKey( e => e.RecurringIntervalId );

				 entity.ToTable( nameof( ApptRecurringInterval ) );
			 } );
		}

		public static void ConfigureApptRecurringIntervalRemoved( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<ApptRecurringIntervalRemoved>( entity =>
			 {
				 entity.HasIndex( e => e.RecurringIntervalId );

                 entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
                 entity.HasKey(i => i.Id);
                 entity.Ignore(i => i.PendingDelete);
                 entity.Ignore(i => i.HasStateBeenSet);

                 entity.Property( e => e.RecurringIntervalId ).HasColumnName( "ApptRecurringInvervalID" );

				 entity.ToTable( nameof( ApptRecurringIntervalRemoved ) );
			 } );
		}

		public static void ConfigureArea( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<Area>( entity =>
			 {
				 entity.HasIndex( e => e.AreaId, "uq__Area_AreaID" )
					 .IsUnique();

				 entity.HasIndex( e => new { e.Name, e.CountryId }, "uq__Area_Name_CountryID" )
					 .IsUnique();

				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.AreaId )
					 .HasColumnName( "AreaID" )
					 .HasDefaultValueSql( "(newsequentialid())" );

				 entity.Property( e => e.CountryId ).HasColumnName( "CountryID" );

				 entity.Property( e => e.Name )
					 .IsRequired()
					 .HasMaxLength( 3 );

				 entity.ToTable( nameof( Area ) );
			 } );
		}

		public static void ConfigureBatterySize( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<BatterySize>( entity =>
			 {
                 entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
                 entity.HasKey(i => i.Id);
                 entity.Ignore(i => i.PendingDelete);
                 entity.Ignore(i => i.HasStateBeenSet);

                 entity.Property( e => e.Description ).HasMaxLength( 50 );

				 entity.Property( e => e.QbModifiedDate )
					 .HasColumnName( "DtQBModified" )
					 .HasColumnType( "datetime" );

				 entity.Property( e => e.UpdatedDate )
					 .HasColumnName( "DtUpdated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.Name )
					 .IsRequired()
					 .HasMaxLength( 50 );

				 entity.Property( e => e.PosItemTypeId ).HasColumnName( "POSItemTypeID" );

				 entity.Property( e => e.Price ).HasColumnType( "money" );

				 entity.Property( e => e.QbAcctId )
					 .HasColumnName( "QBAcctID" )
					 .HasMaxLength( 50 );

				 entity.Property( e => e.QbId )
					 .HasColumnName( "QBID" )
					 .HasMaxLength( 50 );

				 entity.Property( e => e.QbTypeId ).HasColumnName( "QBTypeID" );

				 entity.Property( e => e.TaxGroupId ).HasColumnName( "TaxGroupID" );

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UID" );
				 entity.ToTable( nameof( BatterySize ) );
			 } );
		}

		public static void ConfigureClaimTransaction( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<ClaimTransaction>( entity =>
			 {
				 entity.HasIndex( e => e.AppointmentId )
					 .HasDatabaseName( "IX_ClaimTransaction_AppointmentId" );

				 entity.HasIndex( e => e.ClaimId )
					 .HasDatabaseName( "IX_ClaimTransaction_ClaimId" );

				 entity.HasIndex( e => new { e.Id, e.Deleted, e.Action, e.Status } )
					 .HasDatabaseName( "Claim_QBSync" );

				 entity.HasIndex( e => new { e.ClaimId, e.AppointmentId, e.QbInvoice, e.Deleted, e.Action } )
					 .HasDatabaseName( "ClaimT_QBSync_mls2" );

				 entity.HasIndex( e => new { e.ClaimId, e.AppointmentId, e.Deleted, e.Action, e.QbAdjudicationUpdateDate, e.Adjustment } )
					 .HasDatabaseName( "ClaimT_QBSync_mls1" );

				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.Adjustment ).HasColumnType( "money" );

				 entity.Property( e => e.AppointmentId ).HasColumnName( "AppointmentID" );

				 entity.Property( e => e.ApprovedAmount ).HasColumnType( "money" );

				 entity.Property( e => e.ArVoid ).HasColumnName( "ARVoid" );

				 entity.Property( e => e.Charges ).HasColumnType( "money" );

				 entity.Property( e => e.ClaimId ).HasColumnName( "ClaimID" );

				 entity.Property( e => e.CoInsurance ).HasColumnType( "money" );

				 entity.Property( e => e.CptCode ).HasColumnName( "CPTCode" );

				 entity.Property( e => e.CreatedUserId ).HasColumnName( "CreatedUserID" );

				 entity.Property( e => e.CreatedDate )
					 .HasColumnName( "DateCreated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.UpdatedDate )
					 .HasColumnName( "DateUpdated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.Deductible ).HasColumnType( "money" );

				 entity.Property( e => e.Diagnosis1 )
					 .HasMaxLength( 50 )
					 .IsUnicode( false );

				 entity.Property( e => e.Diagnosis2 )
					 .HasMaxLength( 50 )
					 .IsUnicode( false );

				 entity.Property( e => e.Diagnosis3 )
					 .HasMaxLength( 50 )
					 .IsUnicode( false );

				 entity.Property( e => e.Diagnosis4 )
					 .HasMaxLength( 50 )
					 .IsUnicode( false );

				 entity.Property( e => e.DisallowedCostCont ).HasColumnType( "money" );

				 entity.Property( e => e.DisallowedOther ).HasColumnType( "money" );

				 entity.Property( e => e.AdjustmentDate )
					 .HasColumnName( "DtAdjustment" )
					 .HasColumnType( "datetime" );

				 entity.Property( e => e.ArUpdateDate )
					 .HasColumnName( "DtARUpdate" )
					 .HasColumnType( "datetime" );

				 entity.Property( e => e.FromDate )
					 .HasColumnName( "DtFrom" )
					 .HasColumnType( "datetime" );

				 entity.Property( e => e.PatientPaidDate ).HasColumnType( "datetime" );

				 entity.Property( e => e.PrimaryPaidDate ).HasColumnType( "datetime" );

				 entity.Property( e => e.QbAdjudicationUpdateDate )
					 .HasColumnName( "DtQBAdjUpdate" )
					 .HasColumnType( "datetime" );

				 entity.Property( e => e.QbPatientUpdateDate )
					 .HasColumnName( "DtQBPatUpdate" )
					 .HasColumnType( "datetime" );

				 entity.Property( e => e.QbPrimaryUpdateDate )
					 .HasColumnName( "DtQBPrimUpdate" )
					 .HasColumnType( "datetime" );

				 entity.Property( e => e.QbSecondaryUpdateDate )
					 .HasColumnName( "DtQBSecUpdate" )
					 .HasColumnType( "datetime" );

				 entity.Property( e => e.QbUpdatedDate )
					 .HasColumnName( "DtQBUpdated" )
					 .HasColumnType( "datetime" );

				 entity.Property( e => e.SecondaryPaidDate )
					 .HasColumnName( "DtSecondaryPaid" )
					 .HasColumnType( "datetime" );

				 entity.Property( e => e.ToDate ).HasColumnName( "DtTo" ).HasColumnType( "datetime" );

				 entity.Property( e => e.FacilityId ).HasColumnName( "FacilityID" );

				 entity.Property( e => e.MarketingId ).HasColumnName( "MarketingID" );

				 entity.Property( e => e.NarrativeData ).HasMaxLength( 255 );

				 entity.Property( e => e.Notes ).HasMaxLength( 3500 );

				 entity.Property( e => e.ObligatedToAcceptAmount )
					 .HasColumnName( "ObligatedToAcceptAmt" )
					 .HasColumnType( "money" );

				 entity.Property( e => e.OrderNumber )
					 .HasMaxLength( 30 )
					 .IsUnicode( false );

				 entity.Property( e => e.OrderingId ).HasColumnName( "OrderingID" );

				 entity.Property( e => e.OriginalId ).HasColumnName( "OriginalID" );

				 entity.Property( e => e.OverrideAccountListId )
					 .HasColumnName( "OverrideAccountListID" )
					 .HasMaxLength( 50 );

				 entity.Property( e => e.PatientPaymentReference )
					 .HasColumnName( "PatPmtRef" )
					 .HasMaxLength( 50 );

				 entity.Property( e => e.PatientPaidDate )
					 .HasColumnName( "DtPatientPaid" );

				 entity.Property( e => e.PatientResponsibilityAmount )
					 .HasColumnName( "PatientRespAmount" )
					 .HasColumnType( "money" );

				 entity.Property( e => e.PrimaryPaidDate )
					 .HasColumnName( "DtPrimaryPaid" );

				 entity.Property( e => e.PrimaryPaymentReference )
					 .HasColumnName( "PrimPmtRef" )
					 .HasMaxLength( 50 );

				 entity.Property( e => e.PatientAmountPaid ).HasColumnType( "money" );

				 entity.Property( e => e.PatientResponsibilityAmount ).HasColumnType( "money" );

				 entity.Property( e => e.Place ).HasDefaultValueSql( "((1))" );

				 entity.Property( e => e.PosDocumentId ).HasColumnName( "POSDocumentID" );

				 entity.Property( e => e.PosLineItemId ).HasColumnName( "POSLineItemID" );

				 entity.Property( e => e.PrimaryPaymentReference ).HasMaxLength( 50 );

				 entity.Property( e => e.PrimaryAllowedAmount ).HasColumnType( "money" );

				 entity.Property( e => e.PrimaryAmountPaid ).HasColumnType( "money" );

				 entity.Property( e => e.ProviderId ).HasColumnName( "ProviderID" );

				 entity.Property( e => e.PurchaseServiceCharge )
					 .HasColumnName( "PurchaseServiceChge" )
					 .HasColumnType( "money" );

				 entity.Property( e => e.QbInvoice )
					 .HasColumnName( "QBInvoice" )
					 .HasMaxLength( 50 );

				 entity.Property( e => e.QbInvoiceTransactionId )
					 .HasColumnName( "QBInvoiceTxnID" )
					 .HasMaxLength( 50 );

				 entity.Property( e => e.ReferralId ).HasColumnName( "ReferralID" );

				 entity.Property( e => e.ResubmittalId ).HasColumnName( "ResubmittalID" );

				 entity.Property( e => e.ReviewByCode )
					 .HasMaxLength( 50 )
					 .IsUnicode( false );

				 entity.Property( e => e.SecondaryPaymentReference )
					 .HasColumnName( "SecPmtRef" )
					 .HasMaxLength( 50 );

				 entity.Property( e => e.SecondaryAmountDisallowed ).HasColumnType( "money" );

				 entity.Property( e => e.SecondaryAmountPaid ).HasColumnType( "money" );

				 entity.Property( e => e.ServiceId ).HasColumnName( "ServiceID" );

				 entity.Property( e => e.SpecialPricingIndicator )
					 .HasColumnName( "SpecPricingIndicator" )
					 .HasMaxLength( 50 )
					 .IsUnicode( false );

				 entity.Property( e => e.Status )
					 .IsRequired()
					 .HasMaxLength( 2 )
					 .IsUnicode( false )
					 .HasDefaultValueSql( "('0')" );

				 entity.Property( e => e.SupervisingId ).HasColumnName( "SupervisingID" );

				 entity.Property( e => e.Type ).HasDefaultValueSql( "((1))" );

				 entity.Property( e => e.Units ).HasColumnType( "money" );

				 entity.Property( e => e.UpdatedSiteId ).HasColumnName( "UpdatedSiteID" );

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UpdatedUserID" );

				 entity.ToTable( nameof( ClaimTransaction ) );
			 } );
		}

		public static void ConfigureEmailLog( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<EmailLog>( entity =>
			 {
				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.AppointmentId ).HasColumnName( "AppointmentID" );

				 entity.Property( e => e.Bcc ).HasColumnName( "BCC" );

				 entity.Property( e => e.BodyText ).IsRequired();

				 entity.Property( e => e.Cc ).HasColumnName( "CC" );

				 entity.Property( e => e.CreatedDate )
					 .HasColumnName( "DtCreated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.From )
					 .IsRequired()
					 .HasMaxLength( 100 );

				 entity.Property( e => e.Identifier ).HasMaxLength( 100 );

				 entity.Property( e => e.MessageTemplateId ).HasColumnName( "MessageTemplateID" );

				 entity.Property( e => e.Subject ).HasMaxLength( 100 );

				 entity.Property( e => e.To ).IsRequired();

				 entity.Property( e => e.PatientId ).HasColumnName( "PatientID" );

				 entity.HasOne( e => e.MessageTemplate )
					 .WithMany()
					 .HasForeignKey( e => e.MessageTemplateId );

				 entity.ToTable( nameof( EmailLog ) );
			 } );
		}

		public static void ConfigureEmailTracking( ModelBuilder modelBuilder )
		{

			modelBuilder.Entity<EmailTracking>( entity =>
			 {
				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.CreatedDate )
					 .HasColumnName( "DtCreated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.EmailLogId ).HasColumnName( "EmailLogID" );

				 entity.Property( e => e.Status )
					 .IsRequired()
					 .HasMaxLength( 100 );

				 entity.ToTable( nameof( EmailTracking ) );
			 } );
		}

		public static void ConfigureEmplStatus( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<EmplStatus>( entity =>
			 {
				 entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
				 entity.HasKey(i => i.Id);
				 entity.Ignore(i => i.PendingDelete);
				 entity.Ignore(i => i.HasStateBeenSet);

				 entity.Property(e => e.Name)
					 .IsRequired()
					 .HasMaxLength(50);

				 entity.ToTable("EmplStatus");
			 } );
		}

		public static void ConfigureHaAttachment( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<HaAttachment>( entity =>
			 {
				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.Cost ).HasColumnType( "money" );

				 entity.Property( e => e.CreatedUserId ).HasColumnName( "CreatedByUserID" );

				 entity.Property( e => e.CreatedDate )
					 .HasColumnName( "DateCreated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.ReceivedDate )
					 .HasColumnName( "DtReceived" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.ReturnedDate )
					 .HasColumnName( "DtReturned" )
					 .HasColumnType( "datetime" );

				 entity.Property( e => e.UpdatedDate )
					 .HasColumnName( "DtUpdated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.WarrantyDate )
					 .HasColumnName( "DtWarranty" )
					 .HasColumnType( "datetime" );

				 entity.Property( e => e.HaAttachmentTypeId ).HasColumnName( "HAAttachmentTypeID" );

				 entity.Property( e => e.HaHistoryId ).HasColumnName( "HAHistoryID" );

				 entity.Property( e => e.Invoice ).HasMaxLength( 128 );

				 entity.Property( e => e.ManufactureModel ).HasMaxLength( 50 );

				 entity.Property( e => e.Notes ).HasMaxLength( 1000 );

				 entity.Property( e => e.SerialNumber ).HasMaxLength( 50 );

				 entity.Property( e => e.StatusId ).HasColumnName( "StatusID" );

				 entity.Property( e => e.UdiNumber )
					 .HasColumnName( "UDINumber" )
					 .HasMaxLength( 50 );

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UID" );

				 entity.Property( e => e.UpdatedSiteId ).HasColumnName( "UpdatedSiteID" );

				 entity.ToTable( "HAAttachment" );
			 } );
		}

		public static void ConfigureHaAttachmentType( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<HaComponent>( entity =>
			 {
				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.Description ).HasMaxLength( 50 );

				 entity.Property( e => e.UpdatedDate )
					 .HasColumnName( "DtUpdated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.Name )
					 .IsRequired()
					 .HasMaxLength( 50 );

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UID" );

				 entity.ToTable( "HAAttachmentType" );
			 } );
		}

		public static void ConfigureHaHistory( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<HaHistory>( entity =>
			 {
				 entity.HasIndex( e => e.PatientId );
				 entity.Property( e => e.PatientId ).HasColumnName( "PatID" );

                 entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
                 entity.HasKey(i => i.Id);
                 entity.Ignore(i => i.PendingDelete);
                 entity.Ignore(i => i.HasStateBeenSet);

                 entity.Property( e => e.AppointmentId ).HasColumnName( "AppointmentID" );

				 entity.Property( e => e.BatterySizeId ).HasColumnName( "BatterySizeID" );

				 entity.Property( e => e.Cost ).HasColumnType( "money" );

				 entity.Property( e => e.CrMemoId ).HasColumnName( "CrMemoID" );

				 entity.Property( e => e.CreatedUserId ).HasColumnName( "CreatedByUserID" );

				 entity.Property( e => e.Discount ).HasColumnType( "money" );

				 entity.Property( e => e.DiscountId ).HasColumnName( "DiscountID" );
				 entity.Property( e => e.IsEarmold ).HasColumnName( "Earmold" );

				 entity.Property( e => e.CreatedDate )
					 .HasColumnName( "DtCreated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.FitDate ).HasColumnName( "DtFit" ).HasColumnType( "datetime" );

				 entity.Property( e => e.LastPurchaseDate ).HasColumnName( "DtLastPurchase" ).HasColumnType( "datetime" );

				 entity.Property( e => e.LossDamageDate ).HasColumnName( "DtLossDamage" ).HasColumnType( "datetime" );

				 entity.Property( e => e.OrigWarrantyDate ).HasColumnName( "DtOrigWarranty" ).HasColumnType( "datetime" );

				 entity.Property( e => e.PurchaseDate ).HasColumnName( "DtPurchase" ).HasColumnType( "datetime" );

				 entity.Property( e => e.QbUpdateDate )
					 .HasColumnName( "DtQBUpdate" )
					 .HasColumnType( "datetime" );

				 entity.Property( e => e.UpdatedDate )
					 .HasColumnName( "DtUpdated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.WarrantyDate ).HasColumnName( "DtWarranty" ).HasColumnType( "datetime" );

				 entity.Property( e => e.HaModelId ).HasColumnName( "HAModelID" );

				 entity.Property( e => e.HaOrderId ).HasColumnName( "HAOrderID" );

				 entity.Property( e => e.HaStatusId ).HasColumnName( "HAStatusID" );

				 entity.Property( e => e.HaStockItemId ).HasColumnName( "HAStockItemID" );

				 entity.Property( e => e.HaStyleId ).HasColumnName( "HAStyleID" );

				 entity.Property( e => e.Invoice ).HasMaxLength( 50 );

				 entity.Property( e => e.IsCros ).HasColumnName( "IsCROS" );

				 entity.Property( e => e.LdWarrantyTypeId ).HasColumnName( "LDWarrantyTypeID" );

				 entity.Property( e => e.Notes ).HasMaxLength( 4000 );

				 entity.Property( e => e.OriginalId ).HasColumnName( "OriginalID" );

				 entity.Property( e => e.OtherSideId ).HasColumnName( "OtherSideID" );

				 entity.Property( e => e.PosDocId ).HasColumnName( "POSDocID" );

				 entity.Property( e => e.Price ).HasColumnType( "money" );

				 entity.Property( e => e.ProviderId ).HasColumnName( "ProviderID" );

				 entity.Property( e => e.QbInvoice )
					 .HasColumnName( "QBInvoice" )
					 .HasMaxLength( 50 );

				 entity.Property( e => e.SerialNumber ).HasMaxLength( 50 );

				 entity.Property( e => e.SyncSiteId ).HasColumnName( "SyncSiteID" );

				 entity.Property( e => e.UdiNumber )
					 .HasColumnName( "UDINumber" )
					 .HasMaxLength( 50 );

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UID" );

				 entity.Property( e => e.UpdatedSiteId ).HasColumnName( "UpdatedSiteID" );

				 entity.Property( e => e.WarrantyTypeId ).HasColumnName( "WarrantyTypeID" );

				 entity.HasOne( e => e.Status )
					 .WithMany()
					 .HasForeignKey( e => e.HaStatusId );

				 entity.ToTable( "HAHistory" );
			 } );
		}

		public static void ConfigureHaHistoryOption( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<HaHistoryOption>( entity =>
			 {
				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.HaHistoryId ).HasColumnName( "HAHistoryID" );

				 entity.Property( e => e.OptionId ).HasColumnName( "OptionID" );

				 entity.ToTable( "HAHistoryOption" );
			 } );
		}

		public static void ConfigureHaLoaner( ModelBuilder modelBuilder )
		{

			modelBuilder.Entity<HaLoaner>( entity =>
			 {
				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.CreatedUserId ).HasColumnName( "CreatedByUserID" );

				 entity.Property( e => e.CreatedDate )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.InDate ).HasColumnName( "DtIn" ).HasColumnType( "datetime" );

				 entity.Property( e => e.OutDate )
					 .HasColumnName( "DtOut" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.UpdatedDate )
					 .HasColumnName( "DtUpdated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.DueBackDate )
					 .HasColumnName( "DueBack" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.HaHistoryId ).HasColumnName( "HAHistoryID" );

				 entity.Property( e => e.HaModelId ).HasColumnName( "HAModelID" );

				 entity.Property( e => e.HaStockItemId ).HasColumnName( "HAStockItemID" );

				 entity.Property( e => e.ManufacturerId ).HasColumnName( "ManufacturerID" );

				 entity.Property( e => e.Notes ).HasMaxLength( 255 );

				 entity.Property( e => e.PatientId ).HasColumnName( "PatID" );

				 entity.Property( e => e.SerialNumber )
					 .IsRequired()
					 .HasMaxLength( 50 )
					 .HasDefaultValueSql( "('None Given')" );

				 entity.Property( e => e.SyncSiteId ).HasColumnName( "SyncSiteID" );

				 entity.Property( e => e.UdiNumber )
					 .HasColumnName( "UDINumber" )
					 .HasMaxLength( 50 );

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UID" );

				 entity.Property( e => e.UpdatedSiteId ).HasColumnName( "UpdatedBySiteID" );

				 entity.ToTable( "HALoaner" );
			 } );
		}

		public static void ConfigureHaModel( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<HaModel>( entity =>
			 {

                 entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
                 entity.HasKey(i => i.Id);
                 entity.Ignore(i => i.PendingDelete);
                 entity.Ignore(i => i.HasStateBeenSet);

                 entity.Property( e => e.AvailableForSale )
					 .IsRequired()
					 .HasDefaultValueSql( "((1))" );

				 entity.Property( e => e.BatterySizeId ).HasColumnName( "BatterySizeID" );

				 entity.Property( e => e.CatalogModelId ).HasColumnName( "CatalogModelID" );

				 entity.Property( e => e.Cost ).HasColumnType( "money" );

				 entity.Property( e => e.Description ).HasMaxLength( 50 );

				 entity.Property( e => e.QbModifiedDate )
					 .HasColumnName( "DtQBModified" )
					 .HasColumnType( "datetime" );

				 entity.Property( e => e.UpdatedDate )
					 .HasColumnName( "DtUpdated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.HaStyleId ).HasColumnName( "HAStyleID" );

				 entity.Property( e => e.HaTypeId ).HasColumnName( "HATypeID" );

				 entity.Property( e => e.IsAccessory ).HasDefaultValueSql( "((0))" );

				 entity.Property( e => e.ManufacturerId ).HasColumnName( "ManufacturerID" );

				 entity.Property( e => e.Name )
					 .IsRequired()
					 .HasMaxLength( 24 );

				 entity.Property( e => e.PosItemTypeId ).HasColumnName( "POSItemTypeID" );

				 entity.Property( e => e.Price ).HasColumnType( "money" );

				 entity.Property( e => e.QbAcctId )
					 .HasColumnName( "QBAcctID" )
					 .HasMaxLength( 50 );

				 entity.Property( e => e.QbId )
					 .HasColumnName( "QBID" )
					 .HasMaxLength( 50 );

				 entity.Property( e => e.QbTypeId ).HasColumnName( "QBTypeID" );

				 entity.Property( e => e.TaxGroupId ).HasColumnName( "TaxGroupID" );

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UID" );

				 entity.ToTable( "HAModel" );
			 } );
		}

		public static void ConfigureHaModelOption( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<HaModelOption>( entity =>
			 {
				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.HaModelId ).HasColumnName( "HAModelID" );

				 entity.Property( e => e.HaOptionId ).HasColumnName( "HAOptionID" );

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UID" );

				 entity.ToTable( "HAModelOption" );
			 } );
		}

		public static void ConfigureHaOption( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<HaOption>( entity =>
			 {
				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.Description ).HasMaxLength( 50 );

				 entity.Property( e => e.UpdatedDate )
					 .HasColumnName( "DtUpdated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.Name )
					 .IsRequired()
					 .HasMaxLength( 50 );

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UID" );

				 entity.ToTable( "HAOption" );
			 } );
		}

		public static void ConfigureHaOrder( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<HaOrder>( entity =>
			 {
				 entity.HasIndex( e => e.PatientId );

                 entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
                 entity.HasKey(i => i.Id);
                 entity.Ignore(i => i.PendingDelete);
                 entity.Ignore(i => i.HasStateBeenSet);

                 entity.Property( e => e.Cost ).HasColumnType( "money" );

				 entity.Property( e => e.CreatedUserId ).HasColumnName( "CreatedByUserID" );

				 entity.Property( e => e.CreatedDate )
					 .HasColumnName( "DateCreated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.ExpectedInDate )
					 .HasColumnName( "DtExpectedIn" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.OrderDate )
					 .HasColumnName( "DtOrdered" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.ReceivedDate )
					 .HasColumnName( "DtReceived" )
					 .HasColumnType( "datetime" );

				 entity.Property( e => e.UpdatedDate )
					 .HasColumnName( "DtUpdated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.HaHistoryId ).HasColumnName( "HAHistoryID" );

				 entity.Property( e => e.ManufacturerId ).HasColumnName( "ManufacturerID" );

				 entity.Property( e => e.Notes ).HasMaxLength( 255 );

				 entity.Property( e => e.OrderNumber ).HasMaxLength( 50 );

				 entity.Property( e => e.PatientId ).HasColumnName( "PatID" );

				 entity.Property( e => e.PdfOrderData ).HasColumnName( "PDFOrderData" );

				 entity.Property( e => e.PdfOrderNumber )
					 .HasColumnName( "PDFOrderNumber" )
					 .HasMaxLength( 25 );

				 entity.Property( e => e.Price ).HasColumnType( "money" );

				 entity.Property( e => e.SyncSiteId ).HasColumnName( "SyncSiteID" );

				 entity.Property( e => e.TrackingNumber ).HasMaxLength( 50 );

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UID" );

				 entity.Property( e => e.UpdatedSiteId ).HasColumnName( "UpdatedBySiteID" );

				 entity.ToTable( "HAOrder" );
			 } );
		}

		public static void ConfigureHaOrderImage( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<HaOrderImage>( entity =>
			 {
				 entity.Property( e => e.Id )
					 .HasColumnName( "ID" )
					 .ValueGeneratedNever();

				 entity.Property( e => e.CreatedDate )
					 .HasColumnName( "DateCreated" )
					 .HasColumnType( "datetime" );

				 entity.Property( e => e.Description )
					 .IsRequired()
					 .HasMaxLength( 50 )
					 .IsUnicode( false );

				 entity.Property( e => e.DocumentTypeId ).HasColumnName( "DocumentTypeID" );

				 entity.Property( e => e.HaOrderId ).HasColumnName( "HAOrderID" );

				 entity.Property( e => e.ImageId ).HasColumnName( "ImageID" );

				 entity.Property( e => e.ImageServerId ).HasColumnName( "ImageServerID" );

				 entity.Property( e => e.Notes )
					 .IsRequired()
					 .HasMaxLength( 255 )
					 .IsUnicode( false );

				 entity.Property( e => e.RowVersion )
					 .IsRequired()
					 .HasColumnName( "rowVersion" )
					 .IsRowVersion();

				 entity.Property( e => e.CreatedUserId ).HasColumnName( "UID" );

				 entity.ToTable( "HAOrderImage" );
			 } );
		}

		public static void ConfigureHaReturn( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<HaReturn>( entity =>
			{
				 entity.HasIndex( e => e.HaHistoryId );

                 entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
                 entity.HasKey(i => i.Id);
                 entity.Ignore(i => i.PendingDelete);
                 entity.Ignore(i => i.HasStateBeenSet);

                 entity.Property( e => e.ReturnDate )
					 .HasColumnName( "DtReturn" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.ReturnedToManufacturerDate ).HasColumnName( "DtReturnedToManu" ).HasColumnType( "datetime" );

				 entity.Property( e => e.UpdatedDate )
					 .HasColumnName( "DtUpdated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.HaHistoryId ).HasColumnName( "HAHistoryID" );

				 entity.Property( e => e.HaReturnReasonId ).HasColumnName( "HAReturnReasonID" );

				 entity.Property( e => e.Notes ).HasMaxLength( 255 );

				 entity.Property( e => e.PatientId ).HasColumnName( "PatID" );

				 entity.Property( e => e.SyncSiteId ).HasColumnName( "SyncSiteID" );

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UID" );

                 entity.HasOne(e => e.ReturnReason)
					.WithMany()
					.HasForeignKey(e => e.HaReturnReasonId);

                 entity.ToTable( "HAReturn" );
			} );
		}

		public static void ConfigureHaReturnReason( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<HaReturnReason>( entity =>
			 {
				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.Description ).HasMaxLength( 50 );

				 entity.Property( e => e.UpdatedDate )
					 .HasColumnName( "DtUpdated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.Name )
					 .IsRequired()
					 .HasMaxLength( 50 );

				 entity.Property( e => e.Protected ).HasDefaultValueSql( "((0))" );

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UID" );

				 entity.ToTable( "HAReturnReason" );
			 } );
		}

		public static void ConfigureHaStatus( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<HaStatus>( entity =>
			 {
                 entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
                 entity.HasKey(i => i.Id);
                 entity.Ignore(i => i.PendingDelete);
                 entity.Ignore(i => i.HasStateBeenSet);

                 entity.Property( e => e.Description ).HasMaxLength( 50 );

				 entity.Property( e => e.UpdatedDate )
					 .HasColumnName( "DtUpdated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.Name )
					 .IsRequired()
					 .HasMaxLength( 50 );

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UID" );

				 entity.ToTable( "HAStatus" );
			 } );
		}

		public static void ConfigureHaStockItem( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<HaStockItem>( entity =>
			 {
				 entity.ToTable( "HAStockItem" );

				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.Available )
					 .IsRequired()
					 .HasDefaultValueSql( "((1))" );

				 entity.Property( e => e.Cost )
					 .HasColumnType( "money" )
					 .HasDefaultValueSql( "((0.00))" );

				 entity.Property( e => e.CreatedDate )
					 .HasColumnName( "DtCreated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.ReceivedDate )
					 .HasColumnName( "DtReceived" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.UpdatedDate )
					 .HasColumnName( "DtUpdated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.HaModelId ).HasColumnName( "HAModelID" );

				 entity.Property( e => e.HaStyleId ).HasColumnName( "HAStyleID" );

				 entity.Property( e => e.InactiveReason ).HasMaxLength( 50 );

				 entity.Property( e => e.OrderNumber )
					 .HasMaxLength( 50 )
					 .IsUnicode( false );

				 entity.Property( e => e.PoNumber )
					 .HasColumnName( "PONumber" )
					 .HasMaxLength( 30 )
					 .IsUnicode( false );

				 entity.Property( e => e.Price ).HasColumnType( "money" );

				 entity.Property( e => e.QbId )
					 .HasColumnName( "QBID" )
					 .HasMaxLength( 35 )
					 .IsUnicode( false );

				 entity.Property( e => e.QbUpdatedDate )
					 .HasColumnName( "QBUpdated" )
					 .HasColumnType( "datetime" );

				 entity.Property( e => e.RowVersion )
					 .IsRequired()
					 .HasColumnName( "rowVersion" )
					 .IsRowVersion();

				 entity.Property( e => e.SerialNumber ).HasMaxLength( 50 );

				 entity.Property( e => e.SiteId ).HasColumnName( "SiteID" );

				 entity.Property( e => e.UdiNumber )
					 .HasColumnName( "UDINumber" )
					 .HasMaxLength( 50 );

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UID" );
			 } );
		}

		public static void ConfigureHaStockItemImage( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<HaStockItemImage>( entity =>
			 {
				 entity.ToTable( "HAStockItemImage" );

				 entity.Property( e => e.Id )
					 .HasColumnName( "ID" )
					 .ValueGeneratedNever();

				 entity.Property( e => e.CreatedDate )
					 .HasColumnName( "DateCreated" )
					 .HasColumnType( "datetime" );

				 entity.Property( e => e.Description )
					 .IsRequired()
					 .HasMaxLength( 50 )
					 .IsUnicode( false );

				 entity.Property( e => e.DocumentTypeId ).HasColumnName( "DocumentTypeID" );

				 entity.Property( e => e.HaStockItemId ).HasColumnName( "HAStockItemID" );

				 entity.Property( e => e.ImageId ).HasColumnName( "ImageID" );

				 entity.Property( e => e.ImageServerId ).HasColumnName( "ImageServerID" );

				 entity.Property( e => e.Notes )
					 .IsRequired()
					 .HasMaxLength( 255 )
					 .IsUnicode( false );

				 entity.Property( e => e.RowVersion )
					 .IsRequired()
					 .HasColumnName( "rowVersion" )
					 .IsRowVersion();

				 entity.Property( e => e.CreatedUserId ).HasColumnName( "UID" );
			 } );
		}

		public static void ConfigureHaStockOption( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<HaStockOption>( entity =>
			 {
				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.HaOptionId ).HasColumnName( "HAOptionID" );

				 entity.Property( e => e.HaStockItemId ).HasColumnName( "HAStockItemID" );

				 entity.ToTable( "HAStockOption" );
			 } );
		}

		public static void ConfigureHaStyle( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<HaStyle>( entity =>
			 {
				 entity.ToTable( "HAStyle" );

                 entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
                 entity.HasKey(i => i.Id);
                 entity.Ignore(i => i.PendingDelete);
                 entity.Ignore(i => i.HasStateBeenSet);

                 entity.Property( e => e.Description ).HasMaxLength( 50 );

				 entity.Property( e => e.UpdatedDate )
					 .HasColumnName( "DtUpdated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.Name )
					 .IsRequired()
					 .HasMaxLength( 50 );

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UID" );
			 } );
		}

		public static void ConfigureHaType( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<HaType>( entity =>
			 {
                 entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
                 entity.HasKey(i => i.Id);
                 entity.Ignore(i => i.PendingDelete);
                 entity.Ignore(i => i.HasStateBeenSet);

                 entity.Property( e => e.Description ).HasMaxLength( 50 );

				 entity.Property( e => e.UpdatedDate )
					 .HasColumnName( "DtUpdated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.Name )
					 .IsRequired()
					 .HasMaxLength( 50 );

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UID" );

				 entity.ToTable( "HAType" );
			 } );
		}

		public static void ConfigureHistory( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<History>( entity =>
			 {
				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.HasIndex( e => e.AppointmentId );

				 entity.HasIndex( e => e.HistoryGuid )
					 .HasDatabaseName( "uq_History_HistoryGUID" )
					 .IsUnique();

				 entity.HasIndex( e => e.PatientId );

				 entity.HasIndex( e => new { e.Id, e.PatientId, e.HistoryDate } )
					 .HasDatabaseName( "Tree View" );

				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.ActionId ).HasColumnName( "ActionID" );

				 entity.Property( e => e.AppointmentId ).HasColumnName( "AppointmentID" );

				 entity.Property( e => e.AvailableDate )
					 .HasColumnName( "DtAvailable" )
					 .HasColumnType( "datetime" );

				 entity.Property( e => e.CreatedDate )
					 .HasColumnName( "DtCreated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getutcdate())" );

				 entity.Property( e => e.HistoryDate )
					 .HasColumnName( "DtHistory" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.UpdatedDate )
					 .HasColumnName( "DtUpdated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.ExportDate ).HasColumnType( "datetime" );

				 entity.Property( e => e.HistoryGuid )
					 .HasColumnName( "HistoryGUID" )
					 .HasDefaultValueSql( "(newsequentialid())" );

				 entity.Property( e => e.HistoryTypeId ).HasColumnName( "HistoryTypeID" );

				 entity.Property( e => e.LockDate ).HasColumnType( "datetime" );

				 entity.Property( e => e.ParentId ).HasColumnName( "ParentID" );

				 entity.Property( e => e.PatientId ).HasColumnName( "PatID" );

				 entity.Property( e => e.PatientInteractionId ).HasColumnName( "PatientInteractionID" );

				 entity.Property( e => e.ProviderId ).HasColumnName( "ProviderID" );

				 entity.Property( e => e.ReferralSourceId ).HasColumnName( "ReferralSourceID" );

				 entity.Property( e => e.RowVersion )
					 .IsRequired()
					 .HasColumnName( "rowVersion" )
					 .IsRowVersion();

				 entity.Property( e => e.SlpArticulation ).HasColumnName( "SLPArticulation" );

				 entity.Property( e => e.SlpAttendingSkills ).HasColumnName( "SLPAttendingSkills" );

				 entity.Property( e => e.SlpAwarenessOfOthers ).HasColumnName( "SLPAwarenessOfOthers" );

				 entity.Property( e => e.SlpCommunicativeIntent ).HasColumnName( "SLPCommunicativeIntent" );

				 entity.Property( e => e.SlpCooperation ).HasColumnName( "SLPCooperation" );

				 entity.Property( e => e.SlpDiagnosis ).HasColumnName( "SLPDiagnosis" );

				 entity.Property( e => e.SlpEnvironmentalAwareness ).HasColumnName( "SLPEnvironmentalAwareness" );

				 entity.Property( e => e.SlpExpressiveLanguage ).HasColumnName( "SLPExpressiveLanguage" );

				 entity.Property( e => e.SlpFluency ).HasColumnName( "SLPFluency" );

				 entity.Property( e => e.SlpFluency2 ).HasColumnName( "SLPFluency2" );

				 entity.Property( e => e.SlpFluencyVoiceNotes )
					 .HasColumnName( "SLPFluencyVoiceNotes" )
					 .HasMaxLength( 250 );

				 entity.Property( e => e.SlpGoals ).HasColumnName( "SLPGoals" );

				 entity.Property( e => e.SlpGoalsStatus ).HasColumnName( "SLPGoalsStatus" );

				 entity.Property( e => e.SlpLevelOfActivity ).HasColumnName( "SLPLevelOfActivity" );

				 entity.Property( e => e.SlpPragmatics ).HasColumnName( "SLPPragmatics" );

				 entity.Property( e => e.SlpPrognosis ).HasColumnName( "SLPPrognosis" );

				 entity.Property( e => e.SlpProgressNotes ).HasColumnName( "SLPProgressNotes" );

				 entity.Property( e => e.SlpReceptiveLanguage ).HasColumnName( "SLPReceptiveLanguage" );

				 entity.Property( e => e.SlpRecommendationNotes ).HasColumnName( "SLPRecommendationNotes" );

				 entity.Property( e => e.SlpReliabilityOfScores ).HasColumnName( "SLPReliabilityOfScores" );

				 entity.Property( e => e.SlpResponseRate ).HasColumnName( "SLPResponseRate" );

				 entity.Property( e => e.SlpSocialInteractions ).HasColumnName( "SLPSocialInteractions" );

				 entity.Property( e => e.SlpVoice ).HasColumnName( "SLPVoice" );

				 entity.Property( e => e.SlpVoice2 ).HasColumnName( "SLPVoice2" );

				 entity.Property( e => e.SyncSiteId ).HasColumnName( "SyncSiteID" );

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UID" );

				 entity.Property( e => e.UpdatedSiteId ).HasColumnName( "UpdatedSiteID" );

				 entity.ToTable( nameof( History ) );
			 } );
		}

		public static void ConfigureImageDocumentType( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<ImageDocumentType>( entity =>
			 {
				 entity.Property( e => e.Id )
					 .HasColumnName( "ID" )
					 .ValueGeneratedNever();

				 entity.Property( e => e.AssociatedExtension )
					 .HasMaxLength( 5 )
					 .IsUnicode( false );

				 entity.Property( e => e.CreatedDate ).HasColumnType( "datetime" ).HasColumnName( "DateCreated" );

				 entity.Property( e => e.UpdatedDate ).HasColumnType( "datetime" ).HasColumnName( "DateModified" );

				 entity.Property( e => e.Description )
					 .IsRequired()
					 .HasMaxLength( 255 )
					 .IsUnicode( false );

				 entity.Property( e => e.Name )
					 .IsRequired()
					 .HasMaxLength( 100 )
					 .IsUnicode( false );

				 entity.Property( e => e.RowVersion )
					 .IsRequired()
					 .HasColumnName( "rowVersion" )
					 .IsRowVersion();

				 entity.Property( e => e.ScannerSettings ).HasColumnType( "xml" );

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UID" );
				 entity.ToTable( "ImageDocumentType" );
			 } );
		}

		public static void ConfigureImageServer( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<ImageServer>( entity =>
			 {
				 entity.Property( e => e.Id )
					 .HasColumnName( "ID" )
					 .ValueGeneratedNever();

				 entity.Property( e => e.DatabaseName )
					 .IsRequired()
					 .HasMaxLength( 100 )
					 .IsUnicode( false );

				 entity.Property( e => e.DateCreated ).HasColumnType( "datetime" );

				 entity.Property( e => e.RowVersion )
					 .IsRequired()
					 .HasColumnName( "rowVersion" )
					 .IsRowVersion();

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UID" );
				 entity.ToTable( "ImageServer" );
			 } );
		}

		public static void ConfigureInsurancePayer( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<InsurancePayer>(entity =>
			{
				entity.ToTable("InsuranceCarrier");

                entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
                entity.HasKey(i => i.Id);
                entity.Ignore(i => i.PendingDelete);
                entity.Ignore(i => i.HasStateBeenSet);

                entity.Property(e => e.Address1).HasColumnName("Addr1").HasMaxLength(30);

				entity.Property(e => e.Address2).HasColumnName("Addr2").HasMaxLength(30);

				entity.Property(e => e.CarrierCode).HasMaxLength(128);

				entity.Property(e => e.City).HasMaxLength(20);

				entity.Property(e => e.ClaimOfficeNum)
					.HasMaxLength(128)
					.IsUnicode(false);

				entity.Property(e => e.Contact).HasMaxLength(30);

				entity.Property(e => e.DtQbModified)
					.HasColumnType("datetime")
					.HasColumnName("DtQBModified");

				entity.Property(e => e.UpdatedDate)
					.HasColumnName("DtUpdated")
					.HasColumnType("datetime")
					.HasDefaultValueSql("(getdate())");

				entity.Property(e => e.Email).HasMaxLength(99);

				entity.Property(e => e.EmcProviderId)
					.HasMaxLength(15)
					.HasColumnName("EMCProviderID");

				entity.Property(e => e.Extension).HasMaxLength(30);

				entity.Property(e => e.Fax).HasMaxLength(30);

				entity.Property(e => e.InsuranceType).HasMaxLength(15);

				entity.Property(e => e.IsIcd10).HasColumnName("IsICD10");

				entity.Property(e => e.MipsRequired).HasColumnName("MIPSRequired");

				entity.Property(e => e.Name)
					.IsRequired()
					.HasMaxLength(50);

				entity.Property(e => e.Notes).HasMaxLength(255);

				entity.Property(e => e.Npi)
					.HasMaxLength(15)
					.HasColumnName("NPI");

				entity.Property(e => e.OrganizationId)
					.HasMaxLength(5)
					.HasColumnName("OrganizationID");

				entity.Property(e => e.Phone).HasMaxLength(30);

				entity.Property(e => e.ProviderId)
					.HasMaxLength(15)
					.HasColumnName("ProviderID");

				entity.Property(e => e.QbId)
					.HasMaxLength(50)
					.HasColumnName("QBID");

				entity.Property(e => e.SecondaryId)
					.HasMaxLength(15)
					.HasColumnName("SecondaryID");

				entity.Property(e => e.SecondaryIdQualifier)
					.HasMaxLength(2)
					.HasColumnName("SecondaryIDQualifier");

				entity.Property(e => e.State).HasMaxLength(3);

				entity.Property(e => e.UpdatedUserId).HasColumnName("UID");

				entity.Property(e => e.ZipCode).HasMaxLength(10);
			});
		}

		public static void ConfigureLastPatientList( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<LastPatientList>( entity =>
			 {
				 entity.Property( e => e.Id ).HasColumnName( "ID" );
				 entity.Property( e => e.UserId ).HasColumnName( "UID" );
				 entity.Property( e => e.PatientListXml ).HasColumnName( "Patients" ).HasMaxLength( 200 );
                 entity.Ignore(i => i.PendingDelete);
                 entity.Ignore(i => i.HasStateBeenSet);
                 entity.ToTable( nameof( LastPatientList ) );
			 } );
		}

		public static void ConfigureLoaner( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<HaLoaner>( entity =>
			 {
				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.CreatedUserId ).HasColumnName( "CreatedByUserID" );

				 entity.Property( e => e.CreatedDate )
					 .HasColumnName( "DateCreated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.InDate ).HasColumnName( "DtIn" ).HasColumnType( "datetime" );

				 entity.Property( e => e.OutDate )
					 .HasColumnName( "DtOut" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.UpdatedDate )
					 .HasColumnName( "DtUpdated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.DueBackDate )
					 .HasColumnName( "DueBack" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.HaHistoryId ).HasColumnName( "HAHistoryID" );

				 entity.Property( e => e.HaModelId ).HasColumnName( "HAModelID" );

				 entity.Property( e => e.HaStockItemId ).HasColumnName( "HAStockItemID" );

				 entity.Property( e => e.ManufacturerId ).HasColumnName( "ManufacturerID" );

				 entity.Property( e => e.Notes ).HasMaxLength( 255 );

				 entity.Property( e => e.PatientId ).HasColumnName( "PatID" );

				 entity.Property( e => e.SerialNumber )
					 .IsRequired()
					 .HasMaxLength( 50 )
					 .HasDefaultValueSql( "('None Given')" );

				 entity.Property( e => e.SyncSiteId ).HasColumnName( "SyncSiteID" );

				 entity.Property( e => e.UdiNumber )
					 .HasColumnName( "UDINumber" )
					 .HasMaxLength( 50 );

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UID" );

				 entity.Property( e => e.UpdatedSiteId ).HasColumnName( "UpdatedBySiteID" );

				 entity.ToTable( "HALoaner" );
			 } );
		}

		public static void ConfigureManufacturer( ModelBuilder modelBuilder )
		{

			modelBuilder.Entity<Manufacturer>( entity =>
			 {
                 entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
                 entity.HasKey(i => i.Id);
                 entity.Ignore(i => i.PendingDelete);
                 entity.Ignore(i => i.HasStateBeenSet);

                 entity.Property( e => e.AccountNumber ).HasMaxLength( 100 );

				 entity.Property( e => e.Address1 ).HasColumnName( "Addr1" ).HasMaxLength( 50 );

				 entity.Property( e => e.Address2 ).HasColumnName( "Addr2" ).HasMaxLength( 50 );

				 entity.Property( e => e.City ).HasMaxLength( 50 );

				 entity.Property( e => e.Contact ).HasMaxLength( 50 );

				 entity.Property( e => e.AgreementDate ).HasColumnName( "DtAgreement" ).HasColumnType( "datetime" );

				 entity.Property( e => e.UpdatedDate )
					 .HasColumnName( "DtUpdated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.Ext ).HasMaxLength( 50 );

				 entity.Property( e => e.Fax ).HasMaxLength( 50 );

				 entity.Property( e => e.Name )
					 .IsRequired()
					 .HasMaxLength( 50 );

				 entity.Property( e => e.Notes ).HasMaxLength( 255 );

				 entity.Property( e => e.Phone ).HasMaxLength( 50 );

				 entity.Property( e => e.QbAccountId ).HasColumnName( "QBAccountID" );

				 entity.Property( e => e.State ).HasMaxLength( 3 );

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UID" );

				 entity.Property( e => e.UpdatedUserName )
					 .IsRequired()
					 .HasMaxLength( 50 )
					 .HasDefaultValueSql( "('System')" );

				 entity.Property( e => e.ZipCode ).HasMaxLength( 50 );

				 entity.ToTable( nameof( Manufacturer ) );
			 } );
		}

		public static void ConfigureMaritalStatus( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<MaritalStatus>( entity =>
			 {
				 entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
				 entity.HasKey(i => i.Id);
				 entity.Ignore(i => i.PendingDelete);
				 entity.Ignore(i => i.HasStateBeenSet);

				 entity.Property(e => e.Name)
					 .IsRequired()
					 .HasMaxLength(50);

				 entity.ToTable("MaritalStatus");
			 });
		}

		public static void ConfigureMarketingReference( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<MarketingReference>( entity =>
			 {
                 entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
                 entity.HasKey(i => i.Id);
                 entity.Ignore(i => i.PendingDelete);
                 entity.Ignore(i => i.HasStateBeenSet);

                 entity.Property( e => e.Cost ).HasColumnType( "money" );

				 entity.Property( e => e.Description ).HasMaxLength( 50 );

				 entity.Property( e => e.EndDate ).HasColumnName( "DtEnd" ).HasColumnType( "datetime" );

				 entity.Property( e => e.ReviewDate ).HasColumnName( "DtReview" ).HasColumnType( "datetime" );

				 entity.Property( e => e.StartDate ).HasColumnName( "DtStart" ).HasColumnType( "datetime" );

				 entity.Property( e => e.UpdatedDate )
					 .HasColumnName( "DtUpdated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.CategoryId ).HasColumnName( "MktCategoryID" );

				 entity.Property( e => e.Name )
					 .IsRequired()
					 .HasMaxLength( 50 );

				 entity.Property( e => e.Notes ).HasMaxLength( 4000 );

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UID" );

				 entity.HasMany( e => e.Sites )
					 .WithOne()
					 .HasForeignKey( e => e.SiteId );

				 entity.ToTable( "MktReference" );
			 } );
		}

		public static void ConfigureMarketingReferenceCategory( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<MarketingReferenceCategory>( entity =>
			 {
                 entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
                 entity.HasKey(i => i.Id);
                 entity.Ignore(i => i.PendingDelete);
                 entity.Ignore(i => i.HasStateBeenSet);

                 entity.Property( e => e.Description ).HasMaxLength( 50 );

				 entity.Property( e => e.UpdatedDate )
					 .HasColumnName( "DtUpdated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.Name )
					 .IsRequired()
					 .HasMaxLength( 50 );

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UID" );

				 entity.ToTable( "MktCategory" );
			 } );
		}

		public static void ConfigureMarketingReferenceSite( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<MarketingReferenceSite>( entity =>
			 {
				 entity.HasIndex( e => new { e.MarketingReferenceId, e.SiteId } )
					 .HasDatabaseName( "IX_MktReferenceSite_MktReferenceID" );

				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.MarketingReferenceId ).HasColumnName( "MktReferenceID" );

				 entity.Property( e => e.SiteId ).HasColumnName( "SiteID" );

				 entity.ToTable( "MktReferenceSite" );
			 } );
		}

		public static void ConfigureMedicalCondition( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<MedicalCondition>( entity =>
			 {
				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.Description ).HasMaxLength( 50 );

				 entity.Property( e => e.UpdatedDate )
					 .HasColumnName( "DtUpdated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.Name )
					 .IsRequired()
					 .HasMaxLength( 50 );

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UID" );

				 entity.ToTable( nameof( MedicalCondition ) );
			 } );
		}

		public static void ConfigureMessageSettings( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<MessageSettings>( entity =>
			 {
				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.AccountSid )
					 .HasColumnName( "AccountSID" )
					 .HasMaxLength( 128 );

                 entity.Ignore(i => i.PendingDelete);
                 entity.Ignore(i => i.HasStateBeenSet);

                 entity.Property( e => e.AuthorizationToken ).HasMaxLength( 128 );

				 entity.Property( e => e.UpdatedDate ).HasColumnName( "DtUpdated" ).HasColumnType( "datetime" );

				 entity.Property( e => e.FromEmailAddress ).HasMaxLength( 100 );

				 entity.Property( e => e.FromPhoneNumber ).HasMaxLength( 20 );

				 entity.Property( e => e.FromSmsNumber )
					 .HasColumnName( "FromSMSNumber" )
					 .HasMaxLength( 20 );

				 entity.Property( e => e.InitiateVoiceCallAt ).HasColumnType( "datetime" );

				 entity.Property( e => e.IsVoiceEnabled ).HasDefaultValueSql( "((0))" );

				 entity.Property( e => e.SendEmailAt ).HasColumnType( "datetime" );

				 entity.Property( e => e.SendSmsAt ).HasColumnType( "datetime" );

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UID" );

				 entity.ToTable( nameof( MessageSettings ) );
			 } );
		}

		public static void ConfigureMessageTemplate( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<MessageTemplate>( entity =>
			 {
				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.CreatedDate )
					 .HasColumnName( "DtCreated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.UpdatedDate )
					 .HasColumnName( "DtUpdated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.EmailSubject ).HasMaxLength( 100 );

				 entity.Property( e => e.IsVoiceEnabled ).HasDefaultValueSql( "((0))" );

				 entity.Property( e => e.MessageType )
					 .IsRequired()
					 .HasMaxLength( 50 );

				 entity.Property( e => e.ProviderId ).HasColumnName( "ProviderID" );

				 entity.Property( e => e.SmsBody ).HasMaxLength( 160 );

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UID" );
				 entity.Property( e => e.ConfirmationLinkText ).HasColumnName( "ConfirmationButtonText" );
				 entity.Property( e => e.CallToRescheduleLinkText ).HasColumnName( "CallToRescheduleButtonText" );
				 entity.Property( e => e.CancelLinkText ).HasColumnName( "CancelButtonText" );
				 entity.Ignore( e => e.TemplateType );
				 entity.ToTable( nameof( MessageTemplate ) );
			 } );
		}

		public static void ConfigureN4Action( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<N4Action>( entity =>
			 {

				//entity.HasIndex(e => e.SessionId, "idxActionSessionID");
				//entity.Property(e => e.Id).HasColumnName("ID");
				//entity.Property(e => e.ActionGroup).HasColumnType("datetime");
				//entity.Property(e => e.ActionGroupGuid).HasColumnName("ActionGroupGUID");
				//entity.Property(e => e.ActionGuid)
				//    .HasColumnName("ActionGUID")
				//    .HasDefaultValueSql("(newid())");
				//entity.Property(e => e.CreatedDate).HasColumnType("datetime");
				//entity.Property(e => e.Description)
				//    .HasMaxLength(64)
				//    .HasColumnName("Description_");
				//entity.Property(e => e.FastViewDateCreated).HasColumnType("datetime");
				//entity.Property(e => e.FastViewDateModified).HasColumnType("datetime");
				//entity.Property(e => e.Hidden).HasColumnName("Hidden_");
				////entity.Property(e => e.ImageId).HasColumnName("ImageID");
				////entity.Property(e => e.ImageServerId).HasColumnName("ImageServerID");
				//entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
				//entity.Property(e => e.ModuleId).HasColumnName("ModuleID");
				//entity.Property(e => e.PrivateData).HasColumnType("image");
				//entity.Property(e => e.PublicData).HasColumnType("image");
				//entity.Property(e => e.SessionId).HasColumnName("SessionID");
				//entity.Property(e => e.SetupData).HasMaxLength(4000);
				//entity.Property(e => e.SpeechData).HasMaxLength(4000);
				//entity.Property(e => e.).HasColumnName("UserGUID");
				//entity.Property(e => e.UpdatedUserId).HasColumnName("UserID");
				//entity.Property(e => e.WordRecognitionData).HasMaxLength(4000);
				//entity.HasOne(d => d.Session)
				//    .WithMany(p => p.Actions)
				//    .HasForeignKey(d => d.SessionId)
				//    .HasConstraintName("ActionSessionConstraint");
				//entity.ToTable(nameof(N4Action));
				entity.HasIndex( e => e.SessionId )
					 .HasDatabaseName( "idxActionSessionID" );

				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.ActionGroup ).HasColumnType( "datetime" );

				 entity.Property( e => e.ActionGuid )
					 .HasColumnName( "ActionGUID" )
					 .HasDefaultValueSql( "(newid())" );

				 entity.Property( e => e.CreatedDate ).HasColumnName( "CreateDate" ).HasColumnType( "datetime" );

				 entity.Property( e => e.Description )
					 .HasColumnName( "Description_" )
					 .HasMaxLength( 64 );

				 entity.Property( e => e.Hidden ).HasColumnName( "Hidden_" );

				 entity.Property( e => e.UpdatedDate ).HasColumnName( "LastModifiedDate" ).HasColumnType( "datetime" );

				 entity.Property( e => e.ModuleId ).HasColumnName( "ModuleID" );

				 entity.Property( e => e.PrivateData ).HasColumnType( "image" );

				 entity.Property( e => e.PublicData ).HasColumnType( "image" );

				 entity.Property( e => e.SessionId ).HasColumnName( "SessionID" );

				 entity.Property( e => e.SetupData ).HasMaxLength( 4000 );

				 entity.Property( e => e.SpeechData ).HasMaxLength( 4000 );

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UserID" );

				 entity.Property( e => e.WordRecognitionData ).HasMaxLength( 4000 );

				 entity.HasOne( d => d.Session )
					 .WithMany( a => a.Actions )
					 .IsRequired( true )
					 .HasForeignKey( d => d.SessionId )
					 .HasConstraintName( "ActionSessionConstraint" );

				 entity.HasMany( e => e.ActionReferences )
					 .WithOne( p => p.Action )
					 .HasForeignKey( e => e.ActionId );

				 entity.ToTable( nameof( N4Action ) );
			 } );
		}

		/*

				public static void ConfigureN4ActionLight(ModelBuilder modelBuilder)
				{
					modelBuilder.Entity<N4ActionLight>(entity =>
					{
						entity.ToTable("N4Action");

						entity.HasIndex(e => e.SessionId)
							.HasName("idxActionSessionID");

						entity.Property(e => e.Id).HasColumnName("ID");

						entity.Property(e => e.ActionGroup).HasColumnType("datetime");

						entity.Property(e => e.ActionGuid)
							.HasColumnName("ActionGUID")
							.HasDefaultValueSql("(newid())");

						entity.Property(e => e.CreatedDate).HasColumnName("CreateDate").HasColumnType("datetime");

						entity.Property(e => e.Description)
							.HasColumnName("Description_")
							.HasMaxLength(64);

						entity.Property(e => e.Hidden).HasColumnName("Hidden_");

						entity.Property(e => e.UpdatedDate).HasColumnName("LastModifiedDate").HasColumnType("datetime");

						entity.Property(e => e.ModuleId).HasColumnName("ModuleID");

						entity.Property(e => e.SessionId).HasColumnName("SessionID");

						entity.Property(e => e.SetupData).HasMaxLength(4000);

						entity.Property(e => e.SpeechData).HasMaxLength(4000);

						entity.Property(e => e.UpdatedUserId).HasColumnName("UserID");

						entity.Property(e => e.WordRecognitionData).HasMaxLength(4000);

						entity.HasMany(e => e.ActionReferences)
							.WithOne()
							.HasForeignKey(e => e.ActionId);
						entity.ToTable(nameof(N4Action));
					});
				}*/
		public static void ConfigureN4ActionArchive( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<N4ActionArchive>( entity =>
			 {
				 entity.Property( e => e.Id ).HasColumnName( "ID" );
				 entity.ToTable( nameof( N4ActionArchive ) );
			 } );
		}

		public static void ConfigureN4ActionReference( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<N4ActionReference>( entity =>
			 {
				 entity.HasIndex( e => e.ActionId )
					 .HasDatabaseName( "idxActionReferencesActionID" );

				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.ActionId ).HasColumnName( "ActionID" );

				 entity.Property( e => e.Reference ).HasColumnName( "Reference_" );

				 entity.HasOne( d => d.Action )
					 .WithMany( p => p.ActionReferences )
					 .HasForeignKey( d => d.ActionId )
					 .HasConstraintName( "ActionReferenceConstraint1" );

				 entity.ToTable( nameof( N4ActionReference ) );
			 } );
		}

		public static void ConfigureN4AppPermission( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<N4AppPermission>( entity =>
			 {
				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.Name )
					 .IsRequired()
					 .HasMaxLength( 30 );

				 entity.ToTable( nameof( N4AppPermission ) );
			 } );
		}

		public static void ConfigureN4DashboardAlert( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<N4DashboardAlert>( entity =>
			 {
				 entity.HasKey( e => e.AlertGuid );

				 entity.Property( e => e.AlertGuid )
					 .HasColumnName( "AlertGUID" )
					 .ValueGeneratedNever();

				 entity.Property( e => e.ActionId ).HasColumnName( "ActionID" );

				 entity.Property( e => e.AppModuleId ).HasColumnName( "AppModuleID" );

				 entity.Property( e => e.AssigneeUserId ).HasColumnName( "AssigneeUserID" );

				 entity.Property( e => e.Category )
					 .IsRequired()
					 .HasMaxLength( 50 );

				 entity.Property( e => e.Description )
					 .IsRequired()
					 .HasMaxLength( 1024 );

				 entity.Property( e => e.IconUrl ).HasMaxLength( 1024 );

				 entity.Property( e => e.LastModifiedUtc ).HasColumnType( "datetime" );

				 entity.Property( e => e.ModuleId ).HasColumnName( "ModuleID" );

				 entity.Property( e => e.ModuleParameter ).HasMaxLength( 1024 );

				 entity.Property( e => e.NotificationActionId ).HasColumnName( "NotificationActionID" );

				 entity.Property( e => e.PatientGuid ).HasColumnName( "PatientGUID" );

				 entity.Property( e => e.ReceivedUtc ).HasColumnType( "datetime" );

				 entity.Property( e => e.Url ).HasMaxLength( 1024 );

				 entity.ToTable( nameof( N4DashboardAlert ) );

			 } );
		}

		public static void ConfigureN4DashboardAlertArchive( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<N4DashboardAlertArchive>( entity =>
			 {
				 entity.HasKey( e => e.AlertGuid );

				 entity.Property( e => e.AlertGuid )
					 .HasColumnName( "AlertGUID" )
					 .ValueGeneratedNever();

				 entity.Property( e => e.ActionId ).HasColumnName( "ActionID" );

				 entity.Property( e => e.AppModuleId ).HasColumnName( "AppModuleID" );

				 entity.Property( e => e.AssigneeUserId ).HasColumnName( "AssigneeUserID" );

				 entity.Property( e => e.Category )
					 .IsRequired()
					 .HasMaxLength( 50 );

				 entity.Property( e => e.Description )
					 .IsRequired()
					 .HasMaxLength( 1024 );

				 entity.Property( e => e.IconUrl ).HasMaxLength( 1024 );

				 entity.Property( e => e.LastModifiedUtc ).HasColumnType( "datetime" );

				 entity.Property( e => e.ModuleId ).HasColumnName( "ModuleID" );

				 entity.Property( e => e.ModuleParameter ).HasMaxLength( 1024 );

				 entity.Property( e => e.NotificationActionId ).HasColumnName( "NotificationActionID" );

				 entity.Property( e => e.PatientGuid ).HasColumnName( "PatientGUID" );

				 entity.Property( e => e.ReceivedUtc ).HasColumnType( "datetime" );

				 entity.Property( e => e.Url ).HasMaxLength( 1024 );

				 entity.ToTable( nameof( N4DashboardAlertArchive ) );

			 } );
		}

		public static void ConfigureN4ManualTymp( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<N4ManualTymp>( entity =>
			 {
				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.CanalVolumeUnits ).HasDefaultValueSql( "((0))" );

				 entity.Property( e => e.CreateDate )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.GradientUnits ).HasDefaultValueSql( "((0))" );

				 entity.Property( e => e.MaximumPressureUnits ).HasDefaultValueSql( "((0))" );

				 entity.Property( e => e.PatientId ).HasColumnName( "PatientID" );

				 entity.Property( e => e.PublicData ).HasColumnType( "image" );

				 entity.Property( e => e.Result ).HasDefaultValueSql( "((0))" );

				 entity.Property( e => e.TympDataUnits ).HasDefaultValueSql( "((0))" );

				 entity.Property( e => e.TympId ).HasColumnName( "TympID" );

				 entity.ToTable( nameof( N4ManualTymp ) );
			 } );
		}

		public static void ConfigureN4ManufacturerSetup( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<N4ManufacturerSetup>( entity =>
			 {
				 entity.HasKey( e => e.Id )
					 .IsClustered( false );

				 entity.HasIndex( e => e.ManufacturerId )
					 .IsClustered();

				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.Key ).HasMaxLength( 128 );

				 entity.Property( e => e.ManufacturerId ).HasColumnName( "ManufacturerID" );

				 entity.Property( e => e.RowVersion )
					 .IsRequired()
					 .HasColumnName( "rowVersion" )
					 .IsRowVersion();

				 entity.Property( e => e.SetupData ).HasColumnType( "image" );

				 entity.ToTable( nameof( N4ManufacturerSetup ) );
			 } );
		}

		public static void ConfigureN4MobileApp( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<N4MobileApp>( entity =>
			 {
				 entity.HasKey( e => e.ModuleId );

				 entity.Property( e => e.ModuleId )
					 .HasColumnName( "ModuleID" )
					 .ValueGeneratedNever();

				 entity.Property( e => e.Id )
					 .HasColumnName( "ID" )
					 .ValueGeneratedOnAdd();

				 entity.Property( e => e.Name )
					 .IsRequired()
					 .HasMaxLength( 30 );

				 entity.Property( e => e.Version ).HasMaxLength( 30 );

				 entity.ToTable( nameof( N4MobileApp ) );
			 } );
		}

		public static void ConfigureN4MobileAppPermission( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<N4MobileAppPermission>( entity =>
			 {
				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.ModuleId ).HasColumnName( "ModuleID" );

				 entity.Property( e => e.PermissionId ).HasColumnName( "PermissionID" );

				 entity.ToTable( nameof( N4MobileAppPermission ) );
			 } );
		}

		public static void ConfigureN4PatientIdentification( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<N4PatientIdentification>( entity =>
			 {
				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.IdentificationData ).HasMaxLength( 64 );

				 entity.Property( e => e.ManufacturerId ).HasColumnName( "ManufactureID" );

				 entity.Property( e => e.PatientId ).HasColumnName( "PatientID" );

				 entity.ToTable( nameof( N4PatientIdentification ) );
			 } );
		}

		public static void ConfigureN4PatientSetup( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<N4PatientSetup>( entity =>
			 {
				 entity.HasKey( e => new { e.PatientId, e.ModuleId } );

				 entity.ToTable( "N4PatientSetup" );

				 entity.Property( e => e.PatientId ).HasColumnName( "PatientID" );

				 entity.Property( e => e.ModuleId ).HasColumnName( "ModuleID" );

				 entity.Property( e => e.SetupData ).HasColumnType( "image" );

				 entity.HasOne( d => d.Patient )
					 .WithMany()
					 .HasForeignKey( d => d.PatientId )
					 .HasConstraintName( "PatientPatientSetupConstraint" );

				 entity.ToTable( nameof( N4PatientSetup ) );
			 } );
		}

		public static void ConfigureN4Preference( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<N4Preference>( entity =>
			 {
				 entity.Property( e => e.Id )
					 .HasColumnName( "ID" )
					 .ValueGeneratedNever();

				 entity.Property( e => e.Preference ).HasColumnType( "image" );

				 entity.ToTable( "N4Preferences" );
			 } );
		}

		public static void ConfigureN4Session( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<N4Session>( entity =>
			 {
				 entity.ToTable( nameof( N4Session ) );

				 entity.HasIndex( e => new { e.PatientId, e.CreateDate } )
					 .HasDatabaseName( "IX_N4Session_PatientID" );

				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.CreateDate ).HasColumnType( "datetime" );

				 entity.Property( e => e.PatientId ).HasColumnName( "PatientID" );

				 entity.HasOne( d => d.Patient )
					 .WithMany()
					 .HasForeignKey( d => d.PatientId )
					 .HasConstraintName( "PatientSessionConstraint" );

				 entity.HasMany( e => e.Actions )
					 .WithOne( p => p.Session )
					 .HasForeignKey( e => e.SessionId );

				 entity.ToTable( nameof( N4Session ) );
			 } );
		}

		public static void ConfigureN4UnboundAction( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<N4UnboundAction>( entity =>
			 {
				 entity.HasIndex( e => e.CreatedDate )
                     .HasDatabaseName( "idxUnBoundActionCreateDate" );

				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.ActionGroup ).HasColumnType( "datetime" );

				 entity.Property( e => e.ActionGuid )
					 .HasColumnName( "ActionGUID" )
					 .HasDefaultValueSql( "(newid())" );

				 entity.Property( e => e.CreatedDate ).HasColumnName( "CreateDate" ).HasColumnType( "datetime" );

				 entity.Property( e => e.Description )
					 .HasColumnName( "Description_" )
					 .HasMaxLength( 64 );

				 entity.Property( e => e.Hidden ).HasColumnName( "Hidden_" );

				 entity.Property( e => e.UpdatedDate ).HasColumnName( "LastModifiedDate" ).HasColumnType( "datetime" );

				 entity.Property( e => e.ModuleId ).HasColumnName( "ModuleID" );

				 entity.Property( e => e.PrivateData ).HasColumnType( "image" );

				 entity.Property( e => e.PublicData ).HasColumnType( "image" );

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UserID" );

				 entity.ToTable( nameof( N4UnboundAction ) );
			 } );
		}

		public static void ConfigureN4UnboundActionArchive( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<N4UnboundActionArchive>( entity =>
			 {
				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.ToTable( nameof( N4UnboundActionArchive ) );
			 } );
		}

		public static void ConfigureN4UnboundActionReference( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<N4UnboundActionReference>( entity =>
			 {
				 entity.HasIndex( e => e.ActionId )
					 .HasDatabaseName( "idxUnBoundActionReferenceActionID" );

				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.ActionId ).HasColumnName( "ActionID" );

				 entity.Property( e => e.Reference ).HasColumnName( "Reference_" );

				 entity.HasOne( d => d.Action )
					 .WithMany( p => p.N4UnboundActionReferences )
					 .HasForeignKey( d => d.ActionId )
					 .HasConstraintName( "UnBoundActionReferenceConstraint1" );

				 entity.ToTable( nameof( N4UnboundActionReference ) );
			 } );
		}

		public static void ConfigureN4UserSetup( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<N4UserSetup>( entity =>
			 {
				 entity.HasKey( e => new { e.UserId, e.ModuleId } );

				 entity.Property( e => e.UserId ).HasColumnName( "UserID" );

				 entity.Property( e => e.ModuleId ).HasColumnName( "ModuleID" );

				 entity.Property( e => e.SetupData ).HasColumnType( "image" );

				 entity.ToTable( nameof( N4UserSetup ) );
			 } );
		}

		public static void ConfigureNdmAction( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<NdmAction>( entity =>
			 {
				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.ActionId ).HasColumnName( "ActionID" );

				 entity.Property( e => e.AudiogramDate ).HasColumnType( "datetime" );

				 entity.Property( e => e.CreatedDate ).HasColumnType( "datetime" );

				 entity.ToTable( "NDMAction" );
			 } );
		}

		public static void ConfigureNdmAudiogram( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<NdmAudiogram>( entity =>
			 {
				 entity.HasKey( e => e.Id )
					 .IsClustered( false );

				 entity.HasIndex( e => e.ActionId )
					 .HasDatabaseName( "IX_NDMAudiogram_ActionID" )
					 .IsClustered();

				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.ActionId ).HasColumnName( "NDMActionID" );

				 entity.Property( e => e.MeasurementConditionId ).HasColumnName( "NDMMeasurementConditionID" );

				 entity.Property( e => e.PatientId ).HasColumnName( "PatientID" );
				 entity.HasOne( d => d.MeasurementCondition )
					 .WithMany()
					 .HasForeignKey( d => d.MeasurementConditionId );

				 entity.HasOne( d => d.Action )
					 .WithMany( p => p.Audiograms )
					 .HasForeignKey( d => d.ActionId )
					 .HasConstraintName( "FK_NDMAudiogram_NDMAction" );
				 entity.ToTable( "NDMAudiogram" );
			 } );
		}

		public static void ConfigureNdmMeasurementCondition( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<NdmMeasurementCondition>( entity =>
			 {
				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.HearingInstrument1Condition ).HasColumnName( "HearingInstrument_1_Condition" );

				 entity.Property( e => e.HearingInstrument2Condition ).HasColumnName( "HearingInstrument_2_Condition" );

				 entity.ToTable( "NDMMeasurementCondition" );
			 } );
		}

		public static void ConfigureNdmSearchCriteria( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<NdmSearchCriteria>( entity =>
			 {
				 entity.HasKey( e => e.Id )
					 .IsClustered( false );

				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.CreatedByUserId ).HasColumnName( "CreatedByUserID" );

				 entity.Property( e => e.CreatedDate ).HasColumnType( "datetime" );

				 entity.Property( e => e.IsBaha ).HasColumnName( "IsBAHA" );

				 entity.Property( e => e.IsBc ).HasColumnName( "IsBC" );

				 entity.Property( e => e.IsMcl ).HasColumnName( "IsMCL" );

				 entity.Property( e => e.IsUcl ).HasColumnName( "IsUCL" );

				 entity.Property( e => e.Name )
					 .IsRequired()
					 .HasMaxLength( 50 );

				 entity.Property( e => e.SeverityId ).HasColumnName( "SeverityID" );

				 entity.Property( e => e.TypeofLossId ).HasColumnName( "TypeofLossID" );

				 entity.Property( e => e.UpdatedByUserId ).HasColumnName( "UpdatedByUserID" );

				 entity.Property( e => e.UpdatedDate ).HasColumnType( "datetime" );

				 entity.Property( e => e.UsedForOpportunityTracking ).HasColumnName( "UsedForOT" );

				 entity.ToTable( "NDMSearchCriteria" );
			 } );
		}

		public static void ConfigureNdmSearchPoint( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<NdmSearchPoint>( entity =>
			 {
				 entity.HasKey( e => e.Id )
					 .IsClustered( false );

				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.SearchCriteriaId ).HasColumnName( "NDMSearchCriteriaID" );

				 entity.HasOne( d => d.SearchCriteria )
					 .WithMany( p => p.SearchPoints )
					 .HasForeignKey( d => d.SearchCriteriaId )
					 .HasConstraintName( "FK_NDMSearchPoint_NDMSearchCriteria" );

				 entity.ToTable( "NDMSearchPoint" );
			 } );
		}

		public static void ConfigureNdmTonePoint( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<NdmTonePoint>( entity =>
			 {
				 entity.HasKey( e => e.Id )
					 .IsClustered( false );

				 entity.HasIndex( e => e.AudiogramId )
					 .IsClustered();

				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.AudiogramId ).HasColumnName( "NDMAudiogramID" );

				 entity.HasOne( d => d.Audiogram )
					 .WithMany( p => p.TonePoints )
					 .HasForeignKey( d => d.AudiogramId )
					 .HasConstraintName( "FK_NDMTonePoint_NDMAudiogram" );

				 entity.ToTable( "NDMTonePoint" );
			 } );
		}

		public static void ConfigurePatient( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<Patient>( entity =>
			 {
				 entity.HasIndex( e => e.Guid )
					 .HasDatabaseName( "uq_Patient_PatientGUID" )
					 .IsUnique();

				 entity.HasIndex( e => new { e.Id, e.Inactive } )
					 .HasDatabaseName( "Pat_Active" );

				 entity.HasIndex( e => new { e.LastName, e.FirstName, e.Initial, e.Id, e.Inactive, e.SiteId } )
					 .HasDatabaseName( "Full Name" )
					 .IsUnique();

                 entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
                 entity.HasKey(i => i.Id);
                 entity.Ignore(i => i.PendingDelete);
                 entity.Ignore(i => i.HasStateBeenSet);
                 entity.Ignore(i => i.HasBeenAudited);
                 entity.Ignore(i => i.PatientTypeIds);
                 entity.Ignore(i => i.RestrictionIds);
                 entity.Ignore(i => i.AuthorizationIds);
                 entity.Ignore(i => i.AuthorizationReferences);
                 entity.Ignore(i => i.PatientTypeReferences);

				 entity.Ignore(i => i.InsuredInsurancePayerId);

                 entity.Property( e => e.AccountNo ).HasColumnName( "AccountNum" ).HasMaxLength( 35 );

				 entity.Property( e => e.Address1 ).HasColumnName( "Addr1" ).HasMaxLength( 30 );

				 entity.Property( e => e.Address2 ).HasColumnName( "Addr2" ).HasMaxLength( 30 );

				 entity.Property( e => e.City ).HasMaxLength( 20 );

				 entity.Property( e => e.ResponsibleParty ).HasMaxLength( 50 );
				 entity.Property( e => e.AlternateContact ).HasColumnName( "Contact" ).HasMaxLength( 50 );
				 entity.Property( e => e.MaritalStatusId ).HasColumnName( "MaritalStatusID" ).HasMaxLength( 2 );
				 entity.Property( e => e.EmplStatusId ).HasColumnName( "EmploymentStatusID" ).HasMaxLength( 2 );

                 entity.Property(e => e.SecondaryAddress1)
					.HasColumnName("SecondaryAddr1");

                 entity.Property(e => e.SecondaryAddress2)
                     .HasColumnName("SecondaryAddr2");

                 entity.Property( e => e.AlternateContactPhone ).HasColumnName( "ContactPhone" ).HasMaxLength( 15 );

				//entity.Property(e => e.CountyId).HasColumnName("CountyID");
				entity.Property( e => e.CreatedUserId ).HasColumnName( "CreatedByUserID" );

				 entity.Property( e => e.ProviderId ).HasColumnName( "DefaultProviderID" );

				//entity.Property(e => e.DtConsent).HasColumnType("datetime");
				entity.Property( e => e.CreatedDate )
					 .HasColumnName( "DtCreated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				//entity.Property(e => e.UpdatedDate).HasColumnName("DtLastUpdated").HasColumnType("datetime");
				entity.Property( e => e.UpdatedDate )
					 .HasColumnName( "DtUpdated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.HasIntakeData ).HasColumnName( "PCPForClaim" );

				 entity.Property( e => e.BirthDate ).HasColumnName( "DtOfBirth" ).HasColumnType( "datetime" );

				 entity.Property( e => e.DeathDate ).HasColumnName( "DtOfDeath" ).HasColumnType( "datetime" );

				 entity.Property( e => e.Email ).HasMaxLength( 99 );

				 entity.Property( e => e.FirstName ).HasColumnName( "FName" ).HasMaxLength( 25 );

				 entity.Property( e => e.HomePhone ).HasMaxLength( 15 );

				 entity.Property(e => e.QBID).HasMaxLength(50);

				//entity.Property(e => e.HomePhoneExt).HasMaxLength(15);
				entity.Property( e => e.Initial )
					 .HasMaxLength( 1 )
					 .IsUnicode( false );

				 entity.Property( e => e.LastName )
					 .IsRequired()
					 .HasColumnName( "LName" )
					 .HasMaxLength( 35 )
					 .HasDefaultValueSql( "('NoneGiven')" );

				 entity.Property( e => e.LegalRepFirstName ).HasColumnName( "LegalRepFName" ).HasMaxLength( 12 );
				 entity.Property( e => e.LegalRepLastName ).HasColumnName( "LegalRepLName" ).HasMaxLength( 20 );
				 entity.Property( e => e.LegalRepInitial ).HasMaxLength( 1 );
				 entity.Property( e => e.LegalRepAddress1 ).HasColumnName( "LegalRepAddr1" ).HasMaxLength( 30 );
				 entity.Property( e => e.LegalRepAddress2 ).HasColumnName( "LegalRepAddr2" ).HasMaxLength( 30 );
				 entity.Property( e => e.LegalRepCity ).HasMaxLength( 20 );
				 entity.Property( e => e.LegalRepState ).HasMaxLength( 3 );
				 entity.Property( e => e.LegalRepZipCode ).HasMaxLength( 15 );
				 entity.Property( e => e.LegalRepPhone ).HasMaxLength( 15 );

				 entity.Property( e => e.Language ).HasColumnName( "PreferredLanguage" );

				 entity.Property( e => e.MarketingId ).HasColumnName( "MarketingID" );

				 entity.Property( e => e.CustomDate1 ).HasColumnName( "MiscDt1" ).HasColumnType( "datetime" );

				 entity.Property( e => e.CustomDate2 ).HasColumnName( "MiscDt2" ).HasColumnType( "datetime" );

				 entity.Property( e => e.CustomText1 ).HasColumnName( "MiscText1" ).HasMaxLength( 100 );

				 entity.Property( e => e.CustomText2 ).HasColumnName( "MiscText2" ).HasMaxLength( 100 );

				 entity.Property( e => e.MobilePhone ).HasMaxLength( 15 );

				 entity.Property( e => e.Notes ).HasMaxLength( 2500 );

				 entity.Property( e => e.OtStatusId ).HasColumnName( "OTStatus" );

				 entity.Property( e => e.Guid )
					 .HasColumnName( "PatientGUID" )
					 .HasDefaultValueSql( "(newsequentialid())" );

				 entity.Property( e => e.PatientStatusId ).HasColumnName( "PatientStatusID" );

				 entity.Property( e => e.PatientTypeId ).HasColumnName( "PatientTypeID" );

				 entity.Property( e => e.PreferredName ).HasMaxLength( 25 );

				 entity.Property( e => e.PrimaryCareId ).HasColumnName( "PrimaryCareID" );

				 entity.Property( e => e.PrimaryPhone )
					 .HasConversion<int>()
					 .HasDefaultValueSql( "((1))" );

				 entity.Property( e => e.ReferringPhysicianId ).HasColumnName( "ReferralID" );

				//entity.Property(e => e.RowVersion)
				//    .IsRequired()
				//    .HasColumnName("rowVersion")
				//    .IsRowVersion();
				entity.Property( e => e.SalutationId ).HasColumnName( "SalutationID" );

				 entity.Property( e => e.Sex ).HasMaxLength( 2 );

				 entity.Property( e => e.SiteId ).HasColumnName( "SiteID" );

				 entity.Property( e => e.State ).HasMaxLength( 3 );

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UID" );

				 entity.Property( e => e.UpdatedSiteId ).HasColumnName( "UpdatedSiteID" );

				 entity.Property( e => e.WorkPhone ).HasMaxLength( 15 );
				 entity.Property( e => e.OtherPhone ).HasMaxLength( 15 );

				 entity.Property( e => e.ZipCode ).HasMaxLength( 15 );

				 entity.Property(e => e.ReleaseSignature).HasColumnName("PatientReleaseSignature");

				 entity.Property(e => e.ReleaseSignatureDate)
					.HasColumnName("DtPatientSignature")		
					.HasColumnType("datetime");

				 entity.Property(e => e.AssignBenefitsDate)
					.HasColumnName("DtInsuredSignature")
					.HasColumnType("datetime");

				 entity.Property(e => e.AssignBenefits).HasColumnName("InsuredAssignmentSig");

				 entity.Property( e => e.Ssn )
					 .HasColumnName( "SSN" )
					 .HasMaxLength( 12 );

				 entity.HasOne( e => e.Salutation )
					 .WithMany()
					 .HasForeignKey( e => e.SalutationId );

				 entity.HasOne( e => e.Provider )
					 .WithMany()
					 .HasForeignKey( e => e.ProviderId );

				 entity.HasOne( e => e.PrimaryCarePhysician )
					 .WithMany()
					 .HasForeignKey( e => e.PrimaryCareId );

                 entity.HasOne(e => e.Marketing)
                     .WithMany()
                     .HasForeignKey(e => e.MarketingId);

				 entity.HasOne( e => e.ReferringPhysician )
					 .WithMany()
					 .HasForeignKey( e => e.ReferringPhysicianId );

				 entity.HasOne( e => e.UpdatedByUser )
					 .WithMany()
					 .HasForeignKey( e => e.UpdatedUserId );

				 entity.HasMany( e => e.Restrictions )
					 .WithOne()
					 .HasForeignKey( e => e.PatientId );

                 entity.HasMany( e => e.MedicalConditions )
					 .WithOne()
					 .HasForeignKey( e => e.PatientId );

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

				 entity.ToTable( nameof( Patient ) );
			 } );
		}

		public static void ConfigurePatientImage( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<PatientImage>( entity =>
			 {
				 entity.HasIndex( e => new { e.PatientGuid, e.DocumentTypeId } )
					 .HasDatabaseName( "IX_PatientImage_PatientGUID" );

				 entity.Property( e => e.Id )
					 .HasColumnName( "ID" )
					 .ValueGeneratedNever();

				 entity.Property( e => e.CreatedDate ).HasColumnType( "datetime" ).HasColumnName( "DateCreated" );

				 entity.Property( e => e.Description )
					 .IsRequired()
					 .HasMaxLength( 50 )
					 .IsUnicode( false );

				 entity.Property( e => e.DocumentTypeId ).HasColumnName( "DocumentTypeID" );

				 entity.Property( e => e.DtExpires ).HasColumnType( "datetime" );

				 entity.Property( e => e.ImageId ).HasColumnName( "ImageID" );

				 entity.Property( e => e.ImageServerId ).HasColumnName( "ImageServerID" );

				 entity.Property( e => e.Notes )
					 .IsRequired()
					 .HasMaxLength( 255 )
					 .IsUnicode( false );

				 entity.Property( e => e.Password ).HasMaxLength( 100 );

				 entity.Property( e => e.PatientGuid ).HasColumnName( "PatientGUID" );

				 entity.Property( e => e.RowVersion )
					 .IsRequired()
					 .HasColumnName( "rowVersion" )
					 .IsRowVersion();

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UID" );

				 entity.Ignore( e => e.Image );
				 entity.ToTable( "PatientImage" );
			 } );
		}

		public static void ConfigurePatientInsurance( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<PatientInsurance>( entity =>
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

				 entity.ToTable( nameof( PatientInsurance ) );
			 } );
		}

		public static void ConfigurePatientLetterArchive( ModelBuilder modelBuilder )
		{

			modelBuilder.Entity<PatientLetterArchive>( entity =>
			 {
				 entity.HasKey( e => e.Id )
					 .IsClustered( false );

				 entity.HasIndex( e => e.PatientId )
					 .IsClustered();

				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.ArchiveId ).HasColumnName( "ArchiveID" );

				 entity.Property( e => e.ArchiveServerId ).HasColumnName( "ArchiveServerID" );

				 entity.Property( e => e.CreatedDate ).HasColumnType( "datetime" );

				 entity.Property( e => e.DeletedDate ).HasColumnType( "datetime" );

				 entity.Property( e => e.EmailSubject ).HasMaxLength( 200 );

				 entity.Property( e => e.Name )
					 .IsRequired()
					 .HasMaxLength( 50 );

				 entity.Property( e => e.PatientId ).HasColumnName( "PatientID" );

				 entity.Property( e => e.PrintedDate ).HasColumnType( "datetime" );

				 entity.Property( e => e.TemplateId ).HasColumnName( "TemplateID" );

				 entity.Property( e => e.UpdatedByUserId ).HasColumnName( "UpdatedByUserID" );

				 entity.Property( e => e.Password )
					 .HasMaxLength( 100 );

				 entity.Property( e => e.ExpirationDate ).HasColumnName( "DtExpires" ).HasColumnType( "datetime" );

				 entity.ToTable( nameof( PatientLetterArchive ) );
			 } );

		}

		public static void ConfigurePatientReportTemplate( ModelBuilder modelBuilder )
		{

			modelBuilder.Entity<PatientReportTemplate>( entity =>
			 {
				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.CreatedDate ).HasColumnType( "datetime" );

				 entity.Property( e => e.Description ).HasMaxLength( 100 );

				 entity.Property( e => e.Name )
					 .IsRequired()
					 .HasMaxLength( 50 );

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UpdatedByUserID" );

				 entity.Property( e => e.UpdatedDate ).HasColumnType( "datetime" );

				 entity.ToTable( nameof( PatientReportTemplate ) );
			 } );

		}

		public static void ConfigurePatientRestriction( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<PatientRestriction>( entity =>
			 {
				 entity.HasIndex( e => e.PatientId )
					 .HasDatabaseName( "IX_PatientRestriction_PatID" );

				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.PatientId ).HasColumnName( "PatID" );

				 entity.Property( e => e.RestrictionId ).HasColumnName( "RestrictionID" );

                 entity.Ignore(i => i.PendingDelete);
                 entity.Ignore(i => i.HasStateBeenSet);

				 entity.HasOne( e => e.Patient )
					 .WithMany( p => p.Restrictions )
					 .HasForeignKey( e => e.PatientId );

				 entity.HasOne( e => e.CommunicationRestriction )
					 .WithMany()
					 .HasForeignKey( e => e.RestrictionId );

				 entity.ToTable( nameof( PatientRestriction ) );
			 } );
		}

		public static void ConfigurePosDocument( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<PosDocument>( entity =>
			 {

				 entity.HasIndex( e => e.ApplyToId );

				 entity.HasIndex( e => e.AppointmentId )
					 .HasDatabaseName( "IX_POSDocument_AppointmentId" );

				 entity.HasIndex( e => e.PatientId );

				 entity.HasIndex( e => new { e.Id, e.Final, e.Void, e.QbInvoice, e.QbTransactionId, e.QbUpdateDate, e.ProviderId, e.SiteId } )
					 .HasDatabaseName( "POSDoc_QBSync" );

				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.PatientId ).HasColumnName( "PatID" );

				 entity.Property( e => e.ApplyToId ).HasColumnName( "ApplyToID" );

				 entity.Property( e => e.AppointmentId ).HasColumnName( "AppointmentID" );

				 entity.Property( e => e.ArVoid ).HasColumnName( "ARVoid" );

				 entity.Property( e => e.BillTo )
					 .IsRequired()
					 .HasMaxLength( 500 )
					 .IsUnicode( false )
					 .HasDefaultValueSql( "('Not Provided')" );

				 entity.Property( e => e.BillToAddr1 )
					 .IsRequired()
					 .HasMaxLength( 41 )
					 .HasDefaultValueSql( "('')" );

				 entity.Property( e => e.BillToAddr2 )
					 .IsRequired()
					 .HasMaxLength( 41 )
					 .HasDefaultValueSql( "('')" );

				 entity.Property( e => e.BillToAddr3 )
					 .IsRequired()
					 .HasMaxLength( 41 )
					 .HasDefaultValueSql( "('')" );

				 entity.Property( e => e.BillToAddr4 )
					 .IsRequired()
					 .HasMaxLength( 41 )
					 .HasDefaultValueSql( "('')" );

				 entity.Property( e => e.BillToCity )
					 .IsRequired()
					 .HasMaxLength( 31 )
					 .HasDefaultValueSql( "('')" );

				 entity.Property( e => e.BillToCountry )
					 .IsRequired()
					 .HasMaxLength( 31 )
					 .HasDefaultValueSql( "('')" );

				 entity.Property( e => e.BillToPostalCode )
					 .IsRequired()
					 .HasMaxLength( 13 )
					 .HasDefaultValueSql( "('')" );

				 entity.Property( e => e.BillToState )
					 .IsRequired()
					 .HasMaxLength( 21 )
					 .HasDefaultValueSql( "('')" );

				 entity.Property( e => e.CopayAmount ).HasColumnType( "money" );

				 entity.Property( e => e.CreatedByUserId ).HasColumnName( "CreatedByUserID" );

				 entity.Property( e => e.CustomerMessageId ).HasColumnName( "CustomerMessageID" );

				 entity.Property( e => e.DateCreated )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.ArUpdateDate )
					 .HasColumnName( "DtARUpdate" )
					 .HasColumnType( "datetime" );

				 entity.Property( e => e.DocumentDate )
					 .HasColumnName( "DtDocument" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.UpdatedDate )
					 .HasColumnName( "DtUpdated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.FormClaimNumber ).HasMaxLength( 25 );

				 entity.Property( e => e.InsurancePosItem ).HasColumnName( "InsurancePOSItem" );

				 entity.Property( e => e.MarketingId ).HasColumnName( "MarketingID" );

				 entity.Property( e => e.Memo ).HasMaxLength( 1000 );

				 entity.Property( e => e.Notes ).HasMaxLength( 250 );

				 entity.Property( e => e.PdfClaimData ).HasColumnName( "PDFClaimData" );

				 entity.Property( e => e.PaymentAmount )
					 .HasColumnName( "PmtAmount" )
					 .HasColumnType( "money" );

				 entity.Property( e => e.PaymentMethodId ).HasColumnName( "PmtMethodID" );

				 entity.Property( e => e.PaymentReference )
					 .HasColumnName( "PmtRef" )
					 .HasMaxLength( 50 )
					 .IsUnicode( false );

				 entity.Property( e => e.PoNumber )
					 .HasColumnName( "PONumber" )
					 .HasMaxLength( 25 );

				 entity.Property( e => e.DocumentType )
					 .HasColumnName( "DocType" );

				 entity.Property( e => e.PosDepositId ).HasColumnName( "POSDepositID" );

				 entity.Property( e => e.ProviderId ).HasColumnName( "ProviderID" );

				 entity.Property( e => e.QbInvoice )
					 .HasColumnName( "QBInvoice" )
					 .HasMaxLength( 50 );

				 entity.Property( e => e.QbTransactionId )
					 .HasColumnName( "QBTxnID" )
					 .HasMaxLength( 50 );

				 entity.Property( e => e.QbUpdateDate )
					 .HasColumnName( "QBUpdate" )
					 .HasColumnType( "datetime" );

				 entity.Property( e => e.RowVersion )
					 .IsRequired()
					 .HasColumnName( "rowVersion" )
					 .IsRowVersion();

				 entity.Property( e => e.SiteId ).HasColumnName( "SiteID" );

				 entity.Property( e => e.TaxGroupId ).HasColumnName( "TaxGroupID" );

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UID" );

				 entity.Property( e => e.UpdatedBySiteId ).HasColumnName( "UpdatedBySiteID" );

				 entity.ToTable( "POSDocument" );
			 } );
		}

		public static void ConfigurePractice( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<Practice>( entity =>
			 {
                 entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
                 entity.HasKey(i => i.Id);
                 entity.Ignore(i => i.PendingDelete);
                 entity.Ignore(i => i.HasStateBeenSet);

                 entity.Property( e => e.BusinessRules ).IsUnicode( false );

				 entity.Property( e => e.UpdatedDate )
					 .HasColumnName( "DtUpdated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.UpdatedUserId )
					 .HasColumnName( "UID" );

				 entity.Property( e => e.Email ).HasMaxLength( 50 );

				 entity.Property( e => e.EmailDisclaimer );

                 entity.Property( e => e.Fax ).HasMaxLength( 50 );

				 entity.Property( e => e.InactivityTimeout ).HasDefaultValueSql( "((60))" );

				 entity.Property( e => e.Logo ).HasColumnType( "image" );

				 entity.Property( e => e.UseSiteAddressForReports ).HasColumnName( "ReportAddr" );

				 entity.Property( e => e.Name )
					 .HasMaxLength( 100 )
					 .HasDefaultValueSql( "('Not Provided')" );

				 entity.Property( e => e.Npi )
					 .HasColumnName( "NPI" )
					 .HasMaxLength( 15 );

				 entity.Property( e => e.TaxId )
					 .HasColumnName( "TaxID" )
					 .HasMaxLength( 15 );

				 entity.Property( e => e.LastName )
					 .HasColumnName( "LName" )
					 .HasMaxLength( 20 );
				 entity.Property( e => e.FirstName )
					 .HasColumnName( "FName" )
					 .HasMaxLength( 12 );

				 entity.Property( e => e.BillingAddress1 ).HasColumnName( "PayToAddr1" ).HasMaxLength( 30 );

				 entity.Property( e => e.BillingAddress2 ).HasColumnName( "PayToAddr2" ).HasMaxLength( 30 );

				 entity.Property( e => e.BillingCity ).HasColumnName( "PayToCity" ).HasMaxLength( 20 );

				 entity.Property( e => e.BillingPhoneNumber ).HasColumnName( "PayToPhone" ).HasMaxLength( 50 );

				 entity.Property( e => e.BillingState ).HasColumnName( "PayToState" ).HasMaxLength( 3 );

				 entity.Property( e => e.BillingZipCode ).HasColumnName( "PayToZipCode" ).HasMaxLength( 10 );

				 entity.Property( e => e.LinkAppointmentHistory ).HasColumnName( "LinkApptHist" );

				 entity.Property( e => e.UsesAdAuthentication ).HasColumnName( "UseADAuthentication" );

				 entity.Property( e => e.QbLocale )
					 .IsRequired()
					 .HasColumnName( "QBLocale" )
					 .HasMaxLength( 4 )
					 .IsUnicode( false )
					 .HasDefaultValueSql( "('US')" );

				 entity.Property( e => e.OfficeCode )
					 .HasColumnName( "WebOfficeCode" )
					 .HasMaxLength( 128 )
					 .IsUnicode( false );

				 entity.Property( e => e.WebServer )
					 .HasColumnName( "WebApptServer" )
					 .HasMaxLength( 128 )
					 .IsUnicode( false );

				 entity.Property( e => e.TimsServer )
					 .HasMaxLength( 255 )
					 .IsUnicode( false );

				 entity.ToTable( "Practice" );
			 } );
		}

		public static void ConfigurePreviousHistory( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<PreviousHistory>( entity =>
			 {
				 entity.HasIndex( e => e.PatientId );

				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.ConditionId ).HasColumnName( "ConditionID" );

				 entity.Property( e => e.PatientId ).HasColumnName( "PatID" );

				 entity.HasOne( e => e.Patient )
					 .WithMany( p => p.MedicalConditions )
					 .HasForeignKey( e => e.PatientId );

				 entity.HasOne( e => e.Condition )
					 .WithMany()
					 .HasForeignKey( e => e.ConditionId );

				 entity.ToTable( nameof( PreviousHistory ) );
			 } );
		}

		public static void ConfigureProvider( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<Provider>( entity =>
			 {
                 entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
                 entity.HasKey(i => i.Id);
                 entity.Ignore(i => i.PendingDelete);
                 entity.Ignore(i => i.HasStateBeenSet);

                 entity.Ignore(i => i.Hours);
                 entity.Ignore(i => i.SiteHours);
                 entity.Ignore(i => i.WebColor);
                 entity.Ignore(i => i.UseForPatientScheduling);

                 entity.Property( e => e.AnesthesiaLicNum ).HasMaxLength( 15 );

				 entity.Property( e => e.BillToId ).HasColumnName( "BillToID" );

				 entity.Property( e => e.BlueShieldNum ).HasMaxLength( 15 );

				 entity.Property( e => e.Degree ).HasMaxLength( 15 );

				 entity.Property( e => e.QbModifiedDate )
					 .HasColumnName( "DtQBModified" )
					 .HasColumnType( "datetime" );

				 entity.Property( e => e.SignatureDate ).HasColumnName( "DtSignature" ).HasColumnType( "datetime" );

				 entity.Property( e => e.UpdatedDate )
					 .HasColumnName( "DtUpdated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.FirstName )
					 .IsRequired()
					 .HasColumnName( "FName" )
					 .HasMaxLength( 12 )
					 .HasDefaultValueSql( "(N'NA')" );

				 entity.Property( e => e.GroupNum ).HasMaxLength( 15 );

				 entity.Property( e => e.Initial ).HasMaxLength( 1 );

				 entity.Property( e => e.LastName )
					 .IsRequired()
					 .HasColumnName( "LName" )
					 .HasMaxLength( 20 )
					 .HasDefaultValueSql( "('NoneGiven')" );

				 entity.Property( e => e.MedicaidNum ).HasMaxLength( 15 );

				 entity.Property( e => e.MedicareNum ).HasMaxLength( 15 );

				 entity.Property(e => e.Deleted).HasColumnName("deleted");

				 entity.Property( e => e.Npi )
					 .HasColumnName( "NPI" )
					 .HasMaxLength( 15 );

				 entity.Property( e => e.Qbid )
					 .HasColumnName( "QBID" )
					 .HasMaxLength( 35 );

				 entity.Property( e => e.Qbid2 )
					 .HasColumnName( "QBID2" )
					 .HasMaxLength( 35 );

				//entity.Property(e => e.RowVersion)
				//    .IsRequired()
				//    .HasColumnName("rowVersion")
				//    .IsRowVersion();
				entity.Property( e => e.SecondaryIdNum )
					 .HasColumnName( "SecondaryIDNum" )
					 .HasMaxLength( 15 );

				 entity.Property( e => e.SecondaryIdQualifier )
					 .HasColumnName( "SecondaryIDQualifier" )
					 .HasMaxLength( 2 );

				 //entity.Property( e => e.Signature ).HasColumnType( "image" );

				 entity.Property( e => e.SpecialtyCode ).HasMaxLength( 50 );

				 entity.Property( e => e.SpecialtyLicNum ).HasMaxLength( 15 );

				 entity.Property( e => e.StateLicNum ).HasMaxLength( 15 );

				 entity.Property( e => e.TaxId )
					 .HasColumnName( "TaxID" )
					 .HasMaxLength( 15 );

				 entity.Property( e => e.TaxIdType )
					 .HasColumnName( "TaxIDType" )
					 .HasMaxLength( 10 );

				 entity.Property( e => e.Taxonomy ).HasMaxLength( 10 );

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UID" );

				 entity.Property( e => e.Upin )
					 .HasColumnName( "UPINNum" )
					 .HasMaxLength( 6 );

				 entity.Property( e => e.UsePracticeIds ).HasColumnName( "UsePracticeIDs" );

				 entity.Property( e => e.UserId ).HasColumnName( "UserIDIs" );

				 entity.HasOne( e => e.User )
					 .WithMany()
					 .HasForeignKey( e => e.UserId );

				 entity.Property(e => e.UseForPatientScheduling).HasColumnName("InUse").HasConversion<int>(); ;

				 entity.ToTable( nameof( Provider ) );
			 } );
		}

		public static void ConfigureProviderBlockSchedule( ModelBuilder modelBuilder )
		{
            modelBuilder.Entity<ProviderBlockSchedule>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .IsClustered(false);

                entity.HasIndex(e => new { e.ProviderId, e.ScheduleBlockId, e.ScheduleTimeSlotId })
                    .IsClustered();

                entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
                entity.HasKey(i => i.Id);
                entity.Ignore(i => i.PendingDelete);
                entity.Ignore(i => i.HasStateBeenSet);

                entity.Property(e => e.ScheduleBlockId).HasColumnName("BlockID");

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("DateCreated")
                    .HasColumnType("datetime");

                entity.Property(e => e.ProviderId).HasColumnName("ProviderID");

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .HasColumnName("rowVersion")
                    .IsRowVersion();

                entity.Property(e => e.ScheduleTimeSlotId).HasColumnName("TimeSlotID");

                entity.HasOne(e => e.ScheduleBlock)
                    .WithMany()
                    .HasForeignKey(e => e.ScheduleBlockId);

                entity.HasOne(e => e.ScheduleTimeSlot)
                    .WithMany()
                    .HasForeignKey(e => e.ScheduleTimeSlotId);

                entity.ToTable("ProviderBlockReference");
            });

            // Always return the time slot
            modelBuilder.Entity<ProviderBlockSchedule>().Navigation(e => e.ScheduleTimeSlot).AutoInclude();
        }

		public static void ConfigureRecurringDayQualifier( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<RecurringDayQualifier>( entity =>
			 {
				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.Name )
					 .IsRequired()
					 .HasMaxLength( 15 )
					 .HasDefaultValueSql( "('')" );

				 entity.ToTable( nameof( RecurringDayQualifier ) );
			 } );
		}

		public static void ConfigureRecurringDayType( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<RecurringDayType>( entity =>
			 {
				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.Name )
					 .IsRequired()
					 .HasMaxLength( 15 )
					 .HasDefaultValueSql( "('')" );

				 entity.ToTable( nameof( RecurringDayType ) );
			 } );
		}

		public static void ConfigureRecurringInterval( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<RecurringInterval>( entity =>
			 {
				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.EndDate )
					 .HasColumnName( "DtEnd" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.StartDate )
					 .HasColumnName( "DtStart" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.DayInterval ).HasColumnName( "T1Days" );

				 entity.Property( e => e.IsFridaySet ).HasColumnName( "T2Friday" );

				 entity.Property( e => e.IsMondaySet ).HasColumnName( "T2Monday" );

				 entity.Property( e => e.IsSaturdaySet ).HasColumnName( "T2Saturday" );

				 entity.Property( e => e.IsSundaySet ).HasColumnName( "T2Sunday" );

				 entity.Property( e => e.IsThursdaySet ).HasColumnName( "T2Thursday" );

				 entity.Property( e => e.IsTuesdaySet ).HasColumnName( "T2Tuesday" );

				 entity.Property( e => e.IsWednesdaySet ).HasColumnName( "T2Wednesday" );

				 entity.Property( e => e.WeekInterval ).HasColumnName( "T2WeekCnt" );

				 entity.Property( e => e.DayOfMonth ).HasColumnName( "T34DayNum" );

				 entity.Property( e => e.DayQualifier ).HasColumnName( "T34DayQualID" );

				 entity.Property( e => e.DayOfWeek ).HasColumnName( "T34DayTypeID" );

				 entity.Property( e => e.MonthInterval ).HasColumnName( "T3MonthCnt" );

				 entity.Property( e => e.Month ).HasColumnName( "T4MonthID" );

				 entity.HasMany( e => e.DeletedOccurrences )
					 .WithOne()
					 .HasForeignKey( e => e.RecurringIntervalId );

				 entity.ToTable( nameof( RecurringInterval ) );
			 } );
		}

		public static void ConfigureRecurringIntervalRemoved( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<RecurringIntervalRemoved>( entity =>
			 {
				 entity.HasIndex( e => e.RecurringIntervalId );

				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.RecurringIntervalId ).HasColumnName( "RecurringInvervalID" );

				 entity.ToTable( nameof( RecurringIntervalRemoved ) );
			 } );
		}

		public static void ConfigureRecurringMonth( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<RecurringMonth>( entity =>
			 {
				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.Name )
					 .IsRequired()
					 .HasMaxLength( 15 )
					 .HasDefaultValueSql( "('')" );

				 entity.ToTable( nameof( RecurringMonth ) );
			 } );
		}

		public static void ConfigureReferralSource( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<ReferralSource>( entity =>
			 {
				 entity.HasIndex( e => e.MktReferenceId );

				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.Address1 ).HasColumnName( "Addr1" ).HasMaxLength( 30 );

				 entity.Property( e => e.Address2 ).HasColumnName( "Addr2" ).HasMaxLength( 30 );

				 entity.Property( e => e.City ).HasMaxLength( 20 );

				 entity.Property( e => e.Degree ).HasMaxLength( 25 );

				 entity.Property( e => e.AgreementDate ).HasColumnName( "DtAgreement" ).HasColumnType( "datetime" );

				 entity.Property( e => e.UpdatedDate )
					 .HasColumnName( "DtUpdated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.Email )
					 .HasColumnName( "EMail" )
					 .HasMaxLength( 50 );

				 entity.Property( e => e.Ext ).HasMaxLength( 10 );

				 entity.Property( e => e.Fax ).HasMaxLength( 15 );

				 entity.Property( e => e.FirstName )
					 .IsRequired()
					 .HasColumnName( "FName" )
					 .HasMaxLength( 12 );

				 entity.Property( e => e.IdType )
					 .HasColumnName( "IDType" )
					 .HasMaxLength( 50 );

				 entity.Property( e => e.Initial ).HasMaxLength( 1 );

				 entity.Property( e => e.LastName )
					 .IsRequired()
					 .HasColumnName( "LName" )
					 .HasMaxLength( 20 );

				 entity.Property( e => e.MktReferenceId ).HasColumnName( "MktReferenceID" );

				 entity.Property( e => e.NetworkId )
					 .HasColumnName( "NetworkID" )
					 .HasMaxLength( 15 );

				 entity.Property( e => e.Notes ).HasMaxLength( 255 );

				 entity.Property( e => e.Npi )
					 .HasColumnName( "NPI" )
					 .HasMaxLength( 15 );

				 entity.Property( e => e.Phone ).HasMaxLength( 15 );

				 entity.Property( e => e.PhysicianId )
					 .HasColumnName( "PhysicianID" )
					 .HasMaxLength( 30 );

				 entity.Property( e => e.Practice ).HasMaxLength( 50 );

				 entity.Property( e => e.SalutationId ).HasColumnName( "SalutationID" );

				 entity.Property( e => e.SecondaryIdNum )
					 .HasColumnName( "SecondaryIDNum" )
					 .HasMaxLength( 15 );

				 entity.Property( e => e.SecondaryIdQualifier )
					 .HasColumnName( "SecondaryIDQualifier" )
					 .HasMaxLength( 2 );

				 entity.Property( e => e.SecureEmail ).HasMaxLength( 255 );

				 entity.Property( e => e.State ).HasMaxLength( 3 );

				 entity.Property( e => e.TaxId )
					 .HasColumnName( "TaxID" )
					 .HasMaxLength( 15 );

				 entity.Property( e => e.TaxIdType )
					 .HasColumnName( "TaxIDType" )
					 .HasMaxLength( 1 );

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UID" );

				 entity.Property( e => e.ZipCode ).HasMaxLength( 10 );

				 entity.ToTable( nameof( ReferralSource ) );
			 } );
		}

		public static void ConfigureResource( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<Resource>( entity =>
			 {
                 entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
                 entity.HasKey(i => i.Id);
                 entity.Ignore(i => i.PendingDelete);
                 entity.Ignore(i => i.HasStateBeenSet);

                 entity.Property( e => e.Description ).HasMaxLength( 50 );

				 entity.Property( e => e.UpdatedDate )
					 .HasColumnName( "DtUpdated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.Name )
					 .IsRequired()
					 .HasMaxLength( 50 )
					 .HasDefaultValueSql( "('No Name')" );

				 entity.Property( e => e.SiteId ).HasColumnName( "SiteID" );

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UID" );

				 entity.ToTable( nameof( Resource ) );
			 } );
		}

		public static void ConfigureCommunicationRestriction( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<CommunicationRestriction>( entity =>
			 {
                 entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
                 entity.HasKey(i => i.Id);
                 entity.Ignore(i => i.PendingDelete);
                 entity.Ignore(i => i.HasStateBeenSet);

                 entity.Property( e => e.Description ).HasMaxLength( 50 );

				 entity.Property( e => e.UpdatedDate )
					 .HasColumnName( "DtUpdated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.Name )
					 .IsRequired()
					 .HasMaxLength( 50 );

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UID" );

				 entity.ToTable( "Restriction" );
			 } );
		}

		public static void ConfigureSalutation( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<Salutation>( entity =>
			 {
				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.Description ).HasMaxLength( 50 );

				 entity.Property( e => e.UpdatedDate )
					 .HasColumnName( "DtUpdated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.Name )
					 .IsRequired()
					 .HasMaxLength( 50 );

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UID" );
				 entity.ToTable( nameof( Salutation ) );
			 } );
		}

		public static void ConfigureSchedule( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<Schedule>( entity =>
			 {
				 entity.HasIndex( e => e.ProviderId );

				 entity.HasIndex( e => e.SiteId );

				 entity.Property( e => e.Id ).HasColumnName( "ID" );

                 entity.Ignore(e => e.PendingDelete);
                 entity.Ignore(e => e.HasStateBeenSet);
                 entity.Ignore(e => e.SiteIds);
                 entity.Ignore(e => e.ProviderIds);

                 entity.Property( e => e.EndsAt )
					 .HasColumnName( "ApptEnd" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.StartsAt )
					 .HasColumnName( "ApptStart" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				//entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
				entity.Property( e => e.CreatedUserId ).HasColumnName( "CreatedByUserID" );

				 entity.Property( e => e.UpdatedDate ).HasColumnName( "DateUpdated" ).HasColumnType( "datetime" );

				 entity.Property( e => e.CreatedDate )
					 .HasColumnName( "DtCreated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "('1/1/1900')" );

				 entity.Property( e => e.Location ).HasMaxLength( 35 );

				 entity.Property( e => e.Notes ).HasMaxLength( 1000 );

				 entity.Property( e => e.ProviderId ).HasColumnName( "ProviderID" );

				 entity.Property( e => e.RecurringIntervalId ).HasColumnName( "RecurringIntervalID" );

				 entity.Property( e => e.RowVersion )
					 .IsRequired()
					 .HasColumnName( "rowVersion" )
					 .IsRowVersion();

				 entity.Property( e => e.SiteId ).HasColumnName( "SiteID" );

				 entity.Property( e => e.Title )
					 .IsRequired()
					 .HasMaxLength( 35 )
					 .HasDefaultValueSql( "('No Title Given')" );

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UpdatedByUserID" );
				 entity.Property( e => e.UpdatedSiteId ).HasColumnName( "UpdatedSiteID" );

				 entity.HasOne( e => e.RecurringInterval )
					 .WithMany()
					 .HasForeignKey( e => e.RecurringIntervalId );

				 entity.ToTable( "Schedule" );
			 } );
		}

		public static void ConfigureScheduleBlock( ModelBuilder modelBuilder )
		{
            modelBuilder.Entity<ScheduleBlock>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
                entity.HasKey(i => i.Id);
                entity.Ignore(i => i.PendingDelete);
                entity.Ignore(i => i.HasStateBeenSet);
                entity.Ignore(i => i.color_web);
                entity.Ignore(i => i.AppointmentTypes);

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("DateCreated")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnName("DateModified")
                    .HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.RowVersion)
                    .IsRequired()
                    .HasColumnName("rowVersion")
                    .IsRowVersion();

                entity.Property(e => e.UpdatedUserId).HasColumnName("UpdatedByUserID");

                entity.HasMany(e => e.ProviderBlockSchedules)
                    .WithOne(e => e.ScheduleBlock)
                    .HasForeignKey(e => e.ScheduleBlockId);

                entity.ToTable(nameof(ScheduleBlock));
            });
        }

		public static void ConfigureScheduleTimeSlot( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<ScheduleTimeSlot>( entity =>
			 {
				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.CreatedDate )
					 .HasColumnName( "DateCreated" )
					 .HasColumnType( "datetime" );

				 entity.Property( e => e.EndTime ).HasColumnType( "datetime" );

				 entity.Property( e => e.RowVersion )
					 .IsRequired()
					 .HasColumnName( "rowVersion" )
					 .IsRowVersion();

				 entity.Property( e => e.StartTime ).HasColumnType( "datetime" );

                 entity.Ignore(i => i.PendingDelete);
                 entity.Ignore(i => i.HasStateBeenSet);

				 entity.ToTable( nameof( ScheduleTimeSlot ) );
			 } );
		}

		public static void ConfigureSex( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<Sex>( entity =>
			 {
                 entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
                 entity.HasKey(i => i.Id);
                 entity.Ignore(i => i.PendingDelete);
                 entity.Ignore(i => i.HasStateBeenSet);

                 entity.Property(e => e.Description).HasMaxLength(50);

                 entity.Property(e => e.UpdatedDate)
                     .HasColumnName("DtUpdated")
                     .HasColumnType("datetime")
                     .HasDefaultValueSql("(getdate())");

                 entity.Property(e => e.Name)
                     .IsRequired()
                     .HasMaxLength(2);

                 entity.Property(e => e.Protected).IsRequired();

                 entity.Property(e => e.Pronoun).IsRequired();

                 entity.Property(e => e.Inactive).IsRequired();

                 entity.Property(e => e.UpdatedUserId).HasColumnName("UID");

                 entity.ToTable("Sex");
             } );
		}

		public static void ConfigureSite( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<Site>( entity =>
			 {
                 entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
                 entity.HasKey(i => i.Id);
                 entity.Ignore(i => i.PendingDelete);
                 entity.Ignore(i => i.HasStateBeenSet);
                 entity.Ignore(i => i.WebColor);
                 entity.Ignore(i => i.UseForPatientScheduling);
                 entity.Property( e => e.Address1 ).HasColumnName( "Addr1" ).HasMaxLength( 30 );

				 entity.Property( e => e.Address2 ).HasColumnName( "Addr2" ).HasMaxLength( 30 );

				 entity.Property( e => e.AllWellId )
					 .HasColumnName( "AllWellID" )
					 .HasMaxLength( 128 );

				 entity.Property( e => e.CareCreditMerchantNumber ).HasMaxLength( 64 );

				 entity.Property( e => e.CareCreditPassword ).HasMaxLength( 64 );

				 entity.Property( e => e.CareCreditPracticeCode ).HasMaxLength( 64 );

				 entity.Property( e => e.CcpromoCode ).HasColumnName( "CCPromoCode" );

				 entity.Property( e => e.City ).HasMaxLength( 20 );

				 entity.Property( e => e.DefaultTaxGroupId ).HasColumnName( "DefaultTaxGroupID" );

				 entity.Property( e => e.Description ).HasMaxLength( 50 );

				 entity.Property( e => e.QbModifiedDate )
					 .HasColumnName( "DtQBModified" )
					 .HasColumnType( "datetime" );

				 entity.Property( e => e.UpdatedDate )
					 .HasColumnType( "datetime" )
					 .HasColumnName( "DtUpdated" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.EcareCreditPaymentId ).HasColumnName( "ECareCreditPaymentID" );

				 entity.Property( e => e.EcheckPaymentId ).HasColumnName( "ECheckPaymentID" );

				 entity.Property( e => e.EcreditCardPaymentId ).HasColumnName( "ECreditCardPaymentID" );

				 entity.Property( e => e.FaxNumber ).HasMaxLength( 50 );

				 entity.Property( e => e.FriEnd ).HasColumnType( "datetime" );

				 entity.Property( e => e.FriStart ).HasColumnType( "datetime" );

				 entity.Property( e => e.MonEnd ).HasColumnType( "datetime" );

				 entity.Property( e => e.MonStart ).HasColumnType( "datetime" );

				 entity.Property( e => e.Name )
					 .IsRequired()
					 .HasMaxLength( 50 );

				 entity.Property( e => e.Npi )
					 .HasColumnName( "NPI" )
					 .HasMaxLength( 15 )
					 .IsUnicode( false );

				 entity.Property( e => e.OutreachEducator ).HasMaxLength( 50 );

				 entity.Property( e => e.Phone ).HasMaxLength( 15 );

				 entity.Property( e => e.PracticeId ).HasColumnName( "PracticeID" );

				 entity.Property( e => e.Qbid )
					 .HasColumnName( "QBID" )
					 .HasMaxLength( 50 );

				 entity.Property( e => e.RegionId ).HasColumnName( "RegionID" );

				 entity.Property( e => e.SatEnd ).HasColumnType( "datetime" );

				 entity.Property( e => e.SatStart ).HasColumnType( "datetime" );

				 entity.Property( e => e.SecondaryIdnum )
					 .HasColumnName( "SecondaryIDNum" )
					 .HasMaxLength( 15 )
					 .IsUnicode( false );

				 entity.Property( e => e.SecondaryIdqualifier )
					 .HasColumnName( "SecondaryIDQualifier" )
					 .HasMaxLength( 2 )
					 .IsUnicode( false );

				 entity.Property( e => e.SiteSettingId ).HasColumnName( "SiteSettingID" );

				 entity.Property( e => e.State ).HasMaxLength( 3 );

				 entity.Property( e => e.SunEnd ).HasColumnType( "datetime" );

				 entity.Property( e => e.SunStart ).HasColumnType( "datetime" );

				 entity.Property( e => e.ThurEnd ).HasColumnType( "datetime" );

				 entity.Property( e => e.ThurStart ).HasColumnType( "datetime" );

				 entity.Property( e => e.TransnationalAuthKey )
					 .HasMaxLength( 50 )
					 .IsUnicode( false );

				 entity.Property( e => e.TransnationalPassword ).HasMaxLength( 50 );

				 entity.Property( e => e.TransnationalUsername ).HasMaxLength( 50 );

				 entity.Property( e => e.TuesEnd ).HasColumnType( "datetime" );

				 entity.Property( e => e.TuesStart ).HasColumnType( "datetime" );

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UID" );

				 entity.Property( e => e.WedEnd ).HasColumnType( "datetime" );

				 entity.Property( e => e.WedStart ).HasColumnType( "datetime" );

				 entity.Property( e => e.Zip ).HasMaxLength( 10 );

				 entity.HasMany( e => e.Resources )
					 .WithOne()
					 .HasForeignKey( e => e.SiteId );

				 entity.Property(e => e.UseForPatientScheduling).HasColumnName("InUse").HasConversion<int>(); ;

				 entity.ToTable( "Site" );
			 } );
		}

		public static void ConfigureSmsLog( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<SmsLog>( entity =>
			 {
				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.AppointmentId ).HasColumnName( "AppointmentID" );

				 entity.Property( e => e.Body )
					 .HasMaxLength( 160 );
				 entity.Property( e => e.PatientId ).HasColumnName( "PatientID" );
				 entity.Property( e => e.CreatedUserId ).HasColumnName( "UID" );

				 entity.Property( e => e.CreatedDate )
					 .HasColumnName( "DtCreated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.From )
					 .IsRequired()
					 .HasMaxLength( 20 );

				 entity.Property( e => e.Identifier ).HasMaxLength( 100 );

				 entity.Property( e => e.MessageCost ).HasColumnType( "decimal(18, 0)" );

				 entity.Property( e => e.MessageTemplateId ).HasColumnName( "MessageTemplateID" );

				 entity.Property( e => e.NumberOfMessages ).HasDefaultValueSql( "((1))" );

				 entity.Property( e => e.To )
					 .IsRequired()
					 .HasMaxLength( 20 );

				 entity.HasOne( e => e.MessageTemplate )
					 .WithMany()
					 .HasForeignKey( e => e.MessageTemplateId );

				 entity.ToTable( nameof( SmsLog ) );
			 } );
		}

		public static void ConfigureSmsReply( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<SmsReply>( entity =>
			 {
				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.Body )
					 .IsRequired()
					 .HasMaxLength( 160 );

				 entity.Property( e => e.SmsLogId ).HasColumnName( "SmsLogID" );

				 entity.Property( e => e.CreatedDate )
					 .HasColumnName( "DtCreated" )
					 .HasColumnType( "datetime" );

				 entity.Property( e => e.From )
					 .IsRequired()
					 .HasMaxLength( 20 );

				 entity.Property( e => e.Identifier ).HasMaxLength( 100 );

				 entity.ToTable( nameof( SmsReply ) );
			 } );
		}

		public static void ConfigureSmsTracking( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<SmsTracking>( entity =>
			 {
				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.CreatedDate )
					 .HasColumnName( "DtCreated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.SmsLogId ).HasColumnName( "SmsLogID" );

				 entity.Property( e => e.Status )
					 .IsRequired()
					 .HasMaxLength( 20 );

				 entity.ToTable( nameof( SmsTracking ) );
			 } );
		}

		public static void ConfigureTimsArchive( ModelBuilder modelBuilder )
		{

			modelBuilder.Entity<TimsArchive>( entity =>
			 {
				 entity.Property( e => e.Id ).HasColumnName( "ID" );
				 entity.Property( e => e.ArchiveHtml ).HasColumnName( "ArchiveHTML" );
				 entity.Property( e => e.ArchiveTemplateId ).HasColumnName( "ArchiveTemplateID" );
				 entity.Property( e => e.CreatedDate ).HasColumnType( "datetime" );
				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UpdatedByUserID" );
				 entity.ToTable( "TIMSArchive" );
			 } );

		}

		public static void ConfigureTimsImage( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<TimsImage>( entity =>
			 {

				 entity.Property( e => e.Id )
					 .HasColumnName( "ID" )
					 .ValueGeneratedNever();

				 entity.Property( e => e.DateCreated ).HasColumnType( "datetime" );

				 entity.Property( e => e.DocumentTypeId ).HasColumnName( "DocumentTypeID" );

				 entity.Property( e => e.Image ).IsRequired();

				 entity.Property( e => e.RowVersion )
					 .IsRequired()
					 .HasColumnName( "rowVersion" )
					 .IsRowVersion();

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UID" );
				 entity.ToTable( "TIMSImage" );
			 } );
		}

		public static void ConfigureUser( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<User>( entity =>
			 {
                 entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
                 entity.HasKey(i => i.Id);
                 entity.Ignore(i => i.PendingDelete);
                 entity.Ignore(i => i.HasStateBeenSet);

                 /*

                                 entity.Property(e => e.AllProviders)
                                     .IsRequired()
                                     .HasDefaultValueSql("((1))");

                                 entity.Property(e => e.AllResources)
                                     .IsRequired()
                                     .HasDefaultValueSql("((1))");

                                 entity.Property(e => e.AllSites)
                                     .IsRequired()
                                     .HasDefaultValueSql("((1))");

                                 entity.Property(e => e.ApptPriorDays).HasDefaultValueSql("((60))");*/
                 entity.Property( e => e.ScheduleProviderFilter )
					 .HasColumnName( "ApptProvider" )
					 .IsRequired()
					 .HasMaxLength( 256 )
					 .IsUnicode( false )
					 .HasDefaultValueSql( "((0))" );

				 entity.Property( e => e.ScheduleResourceFilter )
					 .HasColumnName( "ApptResource" )
					 .IsRequired()
					 .HasMaxLength( 256 )
					 .IsUnicode( false )
					 .HasDefaultValueSql( "((0))" );

				 entity.Property( e => e.ScheduleSiteFilter )
					 .HasColumnName( "ApptSite" )
					 .IsRequired()
					 .HasMaxLength( 256 )
					 .IsUnicode( false )
					 .HasDefaultValueSql( "((0))" );

				 entity.Property( e => e.ScheduleSpecialtyFilter )
					 .HasColumnName( "ApptSpecialty" )
					 .IsRequired()
					 .HasMaxLength( 256 )
					 .IsUnicode( false )
					 .HasDefaultValueSql( "('0')" );
				/*

								entity.Property(e => e.CalEndTime).HasDefaultValueSql("((1020))");

								entity.Property(e => e.CalInterval).HasDefaultValueSql("((30))");

								entity.Property(e => e.CalStartTime).HasDefaultValueSql("((480))");

								entity.Property(e => e.CreatePos).HasColumnName("CreatePOS");
				*/
				 entity.Property( e => e.PasswordChangedDate ).HasColumnName( "DatePasswordLastChanged" ).HasColumnType( "datetime" );
				/*

								entity.Property(e => e.Description).HasMaxLength(50);
				*/
				 entity.Property( e => e.UpdatedDate )
					 .HasColumnName( "DtUpdated" )
					 .HasColumnType( "datetime" )
					 .HasDefaultValueSql( "(getdate())" );

				 entity.Property( e => e.Initials )
					 .IsRequired()
					 .HasMaxLength( 50 )
					 .HasDefaultValueSql( "('UNK')" );

				 entity.Property(e => e.MobilePhone)
					 .HasMaxLength(15);

				 entity.Property(e => e.Email)
					 .HasMaxLength(99);

				 entity.Property(e => e.RequireMFA);

				 /*entity.Property(e => e.InstanceId).HasColumnName("InstanceID");

				 entity.Property(e => e.LastLogin).HasColumnType("datetime");

				 entity.Property(e => e.LastLogout).HasColumnType("datetime");

				 entity.Property(e => e.LoginHeartbeat).HasColumnType("datetime");*/
				 entity.Property( e => e.Name )
					 .IsRequired()
					 .HasMaxLength( 50 )
					 .HasDefaultValueSql( "('Unknown')" );

				 entity.Property( e => e.Password )
					 .HasMaxLength( 100 )
					 .HasDefaultValueSql( "('password')" );

				 entity.Property( e => e.SiteId ).HasColumnName( "SiteID" );
				/*

								entity.Property(e => e.StartTab)
									.IsRequired()
									.HasMaxLength(20)
									.IsUnicode(false)
									.HasDefaultValueSql("('0-10')");*/
				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UID" );

				 entity.Property( e => e.UserSettings ).IsUnicode( false );

				 entity.Property( e => e.AdDomain ).HasColumnName( "ADDomain" );

				 entity.Property( e => e.AdUsername ).HasColumnName( "ADUsername" );
				 entity.Property( e => e.Deleted ).HasColumnName( "deleted" );

				 entity.Ignore( e => e.Jwt );

				/*
                                entity.HasOne(d => d.Session)
                                    .WithMany(p => p.Actions)
                                    .HasForeignKey(d => d.SessionId)
                                    .HasConstraintName("ActionSessionConstraint");
                                entity.Property(e => e.WorkDays)
                                    .IsRequired()
                                    .HasMaxLength(20)
                                    .IsUnicode(false)
                                    .HasDefaultValueSql("('2-3-4-5-6')");*/
				 entity.HasMany( e => e.SiteHours )
					 .WithOne()
					 .HasForeignKey( e => e.UserId );

                 entity.Property(e => e.CalendarInterval).HasColumnName("CalInterval");

                 entity.ToTable( "TIMSUser" );
			 } );
		}

		public static void ConfigureUserGroup( ModelBuilder modelBuilder )
		{

			modelBuilder.Entity<UserGroup>( entity =>
			 {
				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.Description ).HasMaxLength( 50 );

				 entity.Property( e => e.Name ).HasMaxLength( 128 );

				 entity.Property( e => e.UpdatedUserId ).HasColumnName( "UpdatedByUserID" );

				 entity.Property( e => e.UpdatedDate ).HasColumnType( "datetime" );

				 entity.HasMany( e => e.Settings )
					 .WithOne()
					 .HasForeignKey( e => e.GroupId );

				 entity.HasMany( e => e.UserReferences )
					 .WithOne()
					 .HasForeignKey( e => e.GroupId );

				 entity.ToTable( nameof( UserGroup ) );
			 } );
		}

		public static void ConfigureUserGroupAppSetting( ModelBuilder modelBuilder )
		{

			modelBuilder.Entity<UserGroupAppSetting>( entity =>
			 {
				 entity.HasKey( e => e.Id )
					 .IsClustered( false );

				 entity.HasIndex( e => e.GroupId )
					 .IsClustered();

				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.GroupId ).HasColumnName( "GroupID" );

				 entity.ToTable( nameof( UserGroupAppSetting ) );
			 } );

		}

		public static void ConfigureUserGroupReference( ModelBuilder modelBuilder )
		{

			modelBuilder.Entity<UserGroupReference>( entity =>
			 {
				 entity.HasKey( e => e.Id ).IsClustered( false );

				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.GroupId ).HasColumnName( "GroupID" );

				 entity.ToTable( nameof( UserGroupReference ) );
			 } );

		}

		public static void ConfigureUserSiteHours( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<UserSiteHours>( entity =>
			 {
                 entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedOnAdd();
                 entity.HasKey(i => i.Id);
                 entity.Ignore(i => i.PendingDelete);
                 entity.Ignore(i => i.HasStateBeenSet);

                 entity.Property( e => e.UserId ).HasColumnName( "UID" );
				 entity.Property( e => e.SiteId ).HasColumnName( "SiteID" );

				 entity.Property( e => e.EndTime ).HasColumnType( "datetime" );
				 entity.Property( e => e.StartTime ).HasColumnType( "datetime" );

				 entity.HasOne( e => e.Site )
					 .WithMany()
					 .HasForeignKey( e => e.SiteId );

				 entity.ToTable( "TIMSUserSite" );
			 } );
		}

        public static void ConfigureUserTask(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserTask>(entity =>
            {
                entity.ToTable("TIMSUserTask");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Ignore(i => i.PendingDelete);
                entity.Ignore(i => i.HasStateBeenSet);

                entity.Property(e => e.UpdatedDate).HasColumnName("DateUpdated").HasColumnType("datetime");

                entity.Property(e => e.CreatedDate).HasColumnName("DtCreated").HasColumnType("datetime").HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DueDate).HasColumnName("DtDue").HasColumnType("datetime");

                entity.Property(e => e.PatientId).HasColumnName("PatientID");
                entity.Property(e => e.CompletedDate).HasColumnName("DtComplete").HasColumnType("datetime");
                entity.Property(e => e.RecurringTaskId).HasColumnName("RecurringTaskID");

                entity.Property(e => e.Task)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.UserTaskTypeId).HasColumnName("TIMSUserTaskTypeID");

                entity.Property(e => e.UpdatedUserId).HasColumnName("UID");
            });
        }

        public static void ConfigureUserTaskType(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserTaskType>(entity =>
            {
                entity.ToTable("TIMSUserTaskType");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Ignore(i => i.PendingDelete);
                entity.Ignore(i => i.HasStateBeenSet);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });
        }

        public static void ConfigureUserTaskUserReference(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserTaskUserReference>(entity =>
            {
                entity.HasKey(e => new { e.UserTaskId, e.UserId })
                    .HasName("pk_TIMSUserTaskUserReference");

                entity.ToTable("TIMSUserTaskUserReference");

                entity.Property(e => e.UserTaskId).HasColumnName("TIMSUserTaskID");

                entity.Property(e => e.UserId).HasColumnName("TIMSUserID");
            });

        }

        public static void ConfigureVersion( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<Version>( entity =>
			 {
				 entity.HasIndex( e => e.VersionGuid )
					 .HasDatabaseName( "uq_Version_VersionGUID" )
					 .IsUnique();

				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.DtApplied ).HasColumnType( "datetime" );

				 entity.Property( e => e.DtDownload ).HasColumnType( "datetime" );

				//entity.Property(e => e.FileImage).HasColumnType("image");
				entity.Ignore( e => e.FileImage );

				 entity.Property( e => e.RowVersion )
					 .IsRequired()
					 .HasColumnName( "rowVersion" )
					 .IsRowVersion();

				 entity.Property( e => e.VersionNumber )
					 .HasColumnName( "Version" )
					 .HasMaxLength( 15 )
					 .IsUnicode( false );

				 entity.Property( e => e.VersionGuid )
					 .HasColumnName( "VersionGUID" )
					 .HasDefaultValueSql( "(newsequentialid())" );

				 entity.ToTable( nameof( Version ) );

			 } );
		}

		public static void ConfigureVoiceCallLog( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<VoiceCallLog>( entity =>
			 {
				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.AppointmentId ).HasColumnName( "AppointmentID" );

				 entity.Property( e => e.CallScript ).IsRequired();

				 entity.Property( e => e.CreatedDate ).HasColumnName( "DtCreated" ).HasColumnType( "datetime" );

				 entity.Property( e => e.From )
					 .IsRequired()
					 .HasMaxLength( 20 );

				 entity.Property( e => e.Identifier ).HasMaxLength( 100 );

				 entity.Property( e => e.MessageTemplateId ).HasColumnName( "MessageTemplateID" );

				 entity.Property( e => e.To )
					 .IsRequired()
					 .HasMaxLength( 20 );

				 entity.HasOne( e => e.MessageTemplate )
					 .WithMany()
					 .HasForeignKey( e => e.MessageTemplateId );

				 entity.ToTable( nameof( VoiceCallLog ) );
			 } );
		}

		public static void ConfigureVoiceCallTracking( ModelBuilder modelBuilder )
		{
			modelBuilder.Entity<VoiceCallTracking>( entity =>
			 {
				 entity.Property( e => e.Id ).HasColumnName( "ID" );

				 entity.Property( e => e.DigitsPressed ).HasMaxLength( 5 );

				 entity.Property( e => e.CreatedDate )
					 .HasColumnName( "DtCreated" )
					 .HasColumnType( "datetime" );

				 entity.Property( e => e.Status )
					 .IsRequired()
					 .HasMaxLength( 20 );

				 entity.Property( e => e.VoiceCallLogId ).HasColumnName( "VoiceCallLogID" );

				 entity.ToTable( nameof( VoiceCallTracking ) )
					 ;
			 } );
		}

		#endregion ModelConfigurator Members

	}
}