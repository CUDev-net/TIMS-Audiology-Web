using Microsoft.EntityFrameworkCore;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.ModelCreators;

namespace TIMS_X.DAL.DAL
{
    public interface IModelCreator
    {
        void CreateModel(ModelBuilder modelBuilder);
    }

    public class TimsContext : DbContext
    {
        public TimsContext(DbContextOptions<TimsContext> options)
            : base(options)
        {
        }

        public TimsContext()
        {
            // For unit tests
        }

        public virtual DbSet<Appointment> Appointments { get; set; }
        public virtual DbSet<AppointmentStatus> AppointmentStatuses { get; set; }
        public virtual DbSet<AppointmentSummary> AppointmentSummaries { get; set; }
        public virtual DbSet<LastPatientList> LastPatientLists { get; set; }
        public virtual DbSet<MarketingReferenceCategory> MarketingReferenceCategories { get; set; }
        public virtual DbSet<NdmAction> NdmActions { get; set; }
        public virtual DbSet<Patient> Patients { get; set; }
        public virtual DbSet<QBPatientBalance> QBPatientBalances { get; set; }
        public virtual DbSet<HaHistory> HaHistories { get; set; }
        public virtual DbSet<PosDocument> PosDocuments { get; set; }
        public virtual DbSet<Practice> Practice { get; set; }
        public virtual DbSet<Schedule> Schedules { get; set; }
        public virtual DbSet<ScheduleBlock> ScheduleBlocks { get; set; }
        public virtual DbSet<Site> Sites { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<HoursOfOperationModel>();
            // Fake Object
            new IntReturnModelCreator().CreateModel(modelBuilder);

            new AdjustmentTypeModelCreator().CreateModel(modelBuilder);
            new AppointmentModelCreator().CreateModel(modelBuilder);
            new ApptRecurringIntervalModelCreator().CreateModel(modelBuilder);
            new ApptAuthorizationModelCreator().CreateModel(modelBuilder);
			new AppointmentSummaryModelCreator().CreateModel(modelBuilder);
            new AppointmentItemSummaryModelCreator().CreateModel(modelBuilder);
            new AppointmentStatusModelCreator().CreateModel(modelBuilder);
            new AppointmentTypeModelCreator().CreateModel(modelBuilder);
            new AuthorizationModelCreator().CreateModel(modelBuilder);
            new AuthorizationReferenceModelCreator().CreateModel(modelBuilder);
			new BatterySizeModelCreator().CreateModel(modelBuilder);
            new CalendarSummaryItemModelCreator().CreateModel(modelBuilder);
            new CommunicationRestrictionModelCreator().CreateModel(modelBuilder);
            new CountyModelCreator().CreateModel(modelBuilder);
            new CptCodeCategoryModelCreator().CreateModel(modelBuilder);
            new CustomerMessageModelCreator().CreateModel(modelBuilder);
            new DescriptionModelCreator().CreateModel(modelBuilder);
			new DiagnosisCodeModelCreator().CreateModel(modelBuilder);
            new DiagnosisCodeCategoryModelCreator().CreateModel(modelBuilder);
            new EmplStatusModelCreator().CreateModel(modelBuilder);
			new InsurancePayerModelCreator().CreateModel(modelBuilder);
            new HaComponentModelCreator().CreateModel(modelBuilder);
            new HaHistoryModelCreator().CreateModel(modelBuilder);
            new HaModelModelCreator().CreateModel(modelBuilder);
            new HaModelOptionModelCreator().CreateModel(modelBuilder);
            new HaOrderModelCreator().CreateModel(modelBuilder);
            new HaRepairComplaintModelCreator().CreateModel(modelBuilder);
            new HaReturnModelCreator().CreateModel(modelBuilder);
            new HaReturnReasonModelCreator().CreateModel(modelBuilder);
            new HaStatusModelCreator().CreateModel(modelBuilder);
            new HaStockItemStatusModelCreator().CreateModel(modelBuilder);
            new HaStyleModelCreator().CreateModel(modelBuilder);
            new HaTypeModelCreator().CreateModel(modelBuilder);
            new HaWarrantyTypeModelCreator().CreateModel(modelBuilder);
            new HistoryModelCreator().CreateModel(modelBuilder);
            new HistoryTypeModelCreator().CreateModel(modelBuilder);
            new HoursOfOperationModelCreator().CreateModel(modelBuilder);
            new InsurancePayerModelCreator().CreateModel(modelBuilder);
            new KpiSiteTargetModelCreator().CreateModel(modelBuilder);
            new LastPatientListModelCreator().CreateModel(modelBuilder);
            new ManufacturerModelCreator().CreateModel(modelBuilder);
            new MaritalStatusModelCreator().CreateModel(modelBuilder);
			new MarketingReferenceCategoryCreator().CreateModel(modelBuilder);
            new MarketingReferenceModelCreator().CreateModel(modelBuilder);
            new MarketingReferenceSiteModelCreator().CreateModel(modelBuilder);
			new MedicationModelCreator().CreateModel(modelBuilder);
            new MedicalConditionModelCreator().CreateModel(modelBuilder);
            new MessageSettingsModelCreator().CreateModel(modelBuilder);
			new ModifierModelCreator().CreateModel(modelBuilder);
            new NdmActionModelCreator().CreateModel(modelBuilder);
            new NdmAudiogramModelCreator().CreateModel(modelBuilder);
            new NdmMeasurementConditionModelCreator().CreateModel(modelBuilder);
            new NdmSearchCriteriaModelCreator().CreateModel(modelBuilder);
            new NdmSearchPointModelCreator().CreateModel(modelBuilder);
            new NdmTonePointModelCreator().CreateModel(modelBuilder);
            new OtStatusDescriptionModelCreator().CreateModel(modelBuilder);
            new OutsideFacilityModelCreator().CreateModel(modelBuilder);
            new PatientInsuranceModelCreator().CreateModel(modelBuilder);
            new PatientModelCreator().CreateModel(modelBuilder);
            new QBPatientBalanceModelCreator().CreateModel(modelBuilder);
            new PatientSummaryModelCreator().CreateModel(modelBuilder);
            new PatientStatusModelCreator().CreateModel(modelBuilder);
            new PatientTypeModelCreator().CreateModel(modelBuilder);
            new PatientTypeReferenceModelCreator().CreateModel(modelBuilder);
			new PatientRestrictionModelCreator().CreateModel(modelBuilder);
            new PatientRequiredFieldModelCreator().CreateModel(modelBuilder);
            new PatientSearchResultModelCreator().CreateModel(modelBuilder);
            new PaymentMethodModelCreator().CreateModel(modelBuilder);
            new PosDocumentModelCreator().CreateModel(modelBuilder);
            new PosLineItemModelCreator().CreateModel(modelBuilder);
            new PracticeModelCreator().CreateModel(modelBuilder);
            new PreviousHistoryModelCreator().CreateModel(modelBuilder);
            new ProviderBlockOpeningModelCreator().CreateModel(modelBuilder);
            new ProviderBlockScheduleModelCreator().CreateModel(modelBuilder);
            new ProviderModelCreator().CreateModel(modelBuilder);
            new RecurringIntervalModelCreator().CreateModel(modelBuilder);
            new RecurringIntervalRemovedModelCreator().CreateModel(modelBuilder);
            new ResourceModelCreator().CreateModel(modelBuilder);
            new ResultModelCreator().CreateModel(modelBuilder);
            new ResultTypeModelCreator().CreateModel(modelBuilder);
            new SalutationModelCreator().CreateModel(modelBuilder);
            new ScheduleBlockModelCreator().CreateModel(modelBuilder);
            new ScheduleItemSummaryModelCreator().CreateModel(modelBuilder);
            new ScheduleRecurringItemSummaryModelCreator().CreateModel(modelBuilder);
            new ScheduleModelCreator().CreateModel(modelBuilder);
            new ScheduleTimeSlotModelCreator().CreateModel(modelBuilder);
            new ScriptedNoteModelCreator().CreateModel(modelBuilder);
            new ScriptedNoteCategoryModelCreator().CreateModel(modelBuilder);
            new SexModelCreator().CreateModel(modelBuilder);
            new SiteModelCreator().CreateModel(modelBuilder);
            new StudentStatusModelCreator().CreateModel(modelBuilder);
			new SubmitterInfoModelCreator().CreateModel(modelBuilder);
            new TaxAgencyModelCreator().CreateModel(modelBuilder);
            new TaxGroupModelCreator().CreateModel(modelBuilder);
            new TaxGroupAssignmentModelCreator().CreateModel(modelBuilder);
            new TaxItemModelCreator().CreateModel(modelBuilder);
            new UserGroupModelCreator().CreateModel(modelBuilder);
            new UserGroupReferenceModelCreator().CreateModel(modelBuilder);
            new UserGroupAppSettingModelCreator().CreateModel(modelBuilder);
            new UserSiteModelCreator().CreateModel(modelBuilder);
            new UserModelCreator().CreateModel(modelBuilder);
            new UserTaskModelCreator().CreateModel(modelBuilder);
            new UserTaskTypeModelCreator().CreateModel(modelBuilder);
        }
    }
}