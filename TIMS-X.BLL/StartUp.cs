using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TIMS_X.BLL.Repositories;
using TIMS_X.BLL.Services;
using TIMS_X.BLL.Validation;
using TIMS_X.BLL.VendorSync.Audigy;
using TIMS_X.BLL.VendorSync.Repositories;
using TIMS_X.DAL;

namespace TIMS_X.BLL
{
    public static class StartUp
    {
        public static IServiceCollection AddTimsXBusinessLogic(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddTimsXDataAccess(configuration);

            services
                .AddTransient<IAdjustmentTypeRepository, AdjustmentTypeRepository>();
            services
                .AddTransient<IAdjustmentTypeValidator, AdjustmentTypeValidator>();
            services
	            .AddTransient<IApptAuthorizationRepository, ApptAuthorizationRepository>();
			services
                .AddTransient<IAppointmentBusinessRuleService, AppointmentBusinessRuleService>();
            services
                .AddTransient<IAppointmentRepository, AppointmentRepository>();
            services
                .AddTransient<IAppointmentStatusRepository, AppointmentStatusRepository>();
            services
                .AddTransient<IAppointmentStatusValidator, AppointmentStatusValidator>();
            services
                .AddTransient<IAppointmentTypeRepository, AppointmentTypeRepository>();
            services
                .AddTransient<IAppointmentTypeValidator, AppointmentTypeValidator>();
            services
                .AddTransient<IAppointmentValidator, AppointmentValidator>();
            services
                .AddTransient<IAudiologistRepository, AudiologistRepository>();
            services
                .AddTransient<IAuthorizationValidator, AuthorizationValidator>();
            services
                .AddTransient<IAuthorizationRepository, AuthorizationRepository>();
            services
                .AddTransient<IBatterySizeValidator, BatterySizeValidator>();
            services
                .AddTransient<IBatterySizeRepository, BatterySizeRepository>();
            services
                .AddTransient<ICommunicationRestrictionValidator, CommunicationRestrictionValidator>();
            services
                .AddTransient<ICommunicationRestrictionRepository, CommunicationRestrictionRepository>();
            services
                .AddTransient<ICountyValidator, CountyValidator>();
            services
                .AddTransient<ICountyRepository, CountyRepository>();
            services
                .AddTransient<ICptCodeCategoryValidator, CptCodeCategoryValidator>();
            services
                .AddTransient<ICptCodeCategoryRepository, CptCodeCategoryRepository>();
            services
                .AddTransient<ICustomerMessageValidator, CustomerMessageValidator>();
            services
                .AddTransient<ICustomerMessageRepository, CustomerMessageRepository>();
            services
                .AddTransient<IDataMiningRepository, DataMiningRepository>();
            services
                .AddTransient<IDiagnosisCodeValidator, DiagnosisCodeValidator>();
            services
                .AddTransient<IDiagnosisCodeRepository, DiagnosisCodeRepository>();
            services
                .AddTransient<IDiagnosisCodeCategoryValidator, DiagnosisCodeCategoryValidator>();
            services
                .AddTransient<IDiagnosisCodeCategoryRepository, DiagnosisCodeCategoryRepository>();
            services
                .AddTransient<IEmployeeRepository, EmployeeRepository>();
            services
                .AddTransient<IHaComponentRepository, HaComponentRepository>();
            services
                .AddTransient<IHaComponentValidator, HaComponentValidator>();
            services
                .AddTransient<IHaHistoryRepository, HaHistoryRepository>();
            services
                .AddTransient<IHaHistoryValidator, HaHistoryValidator>();
            services
                .AddTransient<IHaModelRepository, HaModelRepository>();
            services
                .AddTransient<IHaModelValidator, HaModelValidator>();
            services
                .AddTransient<IHaModelOptionRepository, HaModelOptionRepository>();
            services
                .AddTransient<IHaModelOptionValidator, HaModelOptionValidator>();
            services
                .AddTransient<IHaRepairComplaintRepository, HaRepairComplaintRepository>();
            services
                .AddTransient<IHaRepairComplaintValidator, HaRepairComplaintValidator>();
            services
                .AddTransient<IHaReturnReasonRepository, HaReturnReasonRepository>();
            services
                .AddTransient<IHaReturnReasonValidator, HaReturnReasonValidator>();
            services
                .AddTransient<IHaStatusRepository, HaStatusRepository>();
            services
                .AddTransient<IHaStatusValidator, HaStatusValidator>();
            services
                .AddTransient<IHaStockItemStatusRepository, HaStockItemStatusRepository>();
            services
                .AddTransient<IHaStockItemStatusValidator, HaStockItemStatusValidator>();
            services
                .AddTransient<IHaStyleRepository, HaStyleRepository>();
            services
                .AddTransient<IHaStyleValidator, HaStyleValidator>();
            services
                .AddTransient<IHaTypeRepository, HaTypeRepository>();
            services
                .AddTransient<IHaTypeValidator, HaTypeValidator>();
            services
                .AddTransient<IHaWarrantyTypeRepository, HaWarrantyTypeRepository>();
            services
                .AddTransient<IHaWarrantyTypeValidator, HaWarrantyTypeValidator>();
            services
                .AddTransient<IHistoryRepository, HistoryRepository>();
            services
                .AddTransient<IHistoryValidator, HistoryValidator>();
            services
                .AddTransient<IHistoryTypeRepository, HistoryTypeRepository>();
            services
                .AddTransient<IHistoryTypeValidator, HistoryTypeValidator>();
            services
                .AddTransient<IInsurancePayerRepository, InsurancePayerRepository>();
            services
                .AddTransient<IInsurancePayerValidator, InsurancePayerValidator>();
            services
                .AddTransient<IKpiSiteTargetRepository, KpiSiteTargetRepository>();
            services
                .AddTransient<IKpiSiteTargetValidator, KpiSiteTargetValidator>();
            services
                .AddTransient<ILookUpRepository, LookUpRepository>();
            services
                .AddTransient<IManufacturerRepository, ManufacturerRepository>();
            services
                .AddTransient<IManufacturerValidator, ManufacturerValidator>();
            services
                .AddTransient<IMarketingCategoryRepository, MarketingCategoryRepository>();
            services
                .AddTransient<IMarketingCategoryValidator, MarketingCategoryValidator>();
            services
                .AddTransient<IMarketingReferenceRepository, MarketingReferenceRepository>();
            services
                .AddTransient<IMarketingReferenceValidator, MarketingReferenceValidator>();
            services
                .AddTransient<IMedicationRepository, MedicationRepository>();
            services
                .AddTransient<IMedicationValidator, MedicationValidator>();
            services
                .AddTransient<IMedicalConditionRepository, MedicalConditionRepository>();
            services
                .AddTransient<IMedicalConditionValidator, MedicalConditionValidator>();
            services
	            .AddTransient<IMessageSettingsRepository, MessageSettingsRepository>();
			services
                .AddTransient<IModifierRepository, ModifierRepository>();
            services
                .AddTransient<IModifierValidator, ModifierValidator>();
            services
                .AddTransient<IOutsideFacilityRepository, OutsideFacilityRepository>();
            services
                .AddTransient<IOutsideFacilityValidator, OutsideFacilityValidator>();
            services
                .AddTransient<IPatientAppointmentRepository, PatientAppointmentRepository>();
            services
	            .AddTransient<IPatientInsuranceRepository, PatientInsuranceRepository>();
            services
	            .AddTransient<IPatientRepository, PatientRepository>();
			services
	            .AddTransient<IPatientSchedulingRepository, PatientSchedulingRepository>();
			services
				.AddTransient<IPatientStatusRepository, PatientStatusRepository>();
            services
                .AddTransient<IPatientStatusValidator, PatientStatusValidator>();
            services
                .AddTransient<IPatientTypeRepository, PatientTypeRepository>();
            services
                .AddTransient<IPatientTypeValidator, PatientTypeValidator>();
            services
                .AddTransient<IPatientRequiredFieldRepository, PatientRequiredFieldRepository>();
            services
                .AddTransient<IPaymentMethodRepository, PaymentMethodRepository>();
            services
                .AddTransient<IPaymentMethodValidator, PaymentMethodValidator>();
            services
                .AddTransient<IPointOfSaleHelper, PointOfSaleHelper>();
            services
                .AddTransient<IPointOfSaleRepository, PointOfSaleRepository>();
            services
                .AddTransient<Repositories.IPracticeRepository, Repositories.PracticeRepository>();
            services
                .AddTransient<VendorSync.Repositories.IPracticeRepository, VendorSync.Repositories.PracticeRepository>();
            services
                .AddTransient<VendorSync.Repositories.ILocationRepository, VendorSync.Repositories.LocationRepository>();
            services
                .AddTransient<IPreviousHistoryRepository, PreviousHistoryRepository>();
            services
                .AddTransient<IPreviousHistoryValidator, PreviousHistoryValidator>();
            services
                .AddTransient<IProviderRepository, ProviderRepository>();
            services
                .AddTransient<IRepairComplaintRepository, RepairComplaintRepository>();
            services
                .AddTransient<IRepairComplaintValidator, RepairComplaintValidator>();
            services
                .AddTransient<IResourceRepository, ResourceRepository>();
            services
                .AddTransient<IResourceValidator, ResourceValidator>();
            services
                .AddTransient<IResultRepository, ResultRepository>();
            services
                .AddTransient<IResultValidator, ResultValidator>();
            services
                .AddTransient<IResultTypeRepository, ResultTypeRepository>();
            services
                .AddTransient<IResultTypeValidator, ResultTypeValidator>();
            services
                .AddTransient <ISalutationValidator, SalutationValidator>();
            services
                .AddTransient<ISalutationRepository, SalutationRepository>();
            services
                .AddTransient<IScheduleBlockRepository, ScheduleBlockRepository>();
            services
                .AddTransient<IScheduleBlockValidator, ScheduleBlockValidator>();
            services
                .AddTransient<IScheduleRepository, ScheduleRepository>();
            services
                .AddTransient<IScheduleValidator, ScheduleValidator>();
            services
                .AddTransient<IScheduleOpeningsRepository, ScheduleOpeningsRepository>();
            services
                .AddTransient<IScriptedNoteRepository, ScriptedNoteRepository>();
            services
                .AddTransient<IScriptedNoteValidator, ScriptedNoteValidator>();
            services
                .AddTransient<IScriptedNoteCategoryRepository, ScriptedNoteCategoryRepository>();
            services
                .AddTransient<IScriptedNoteCategoryValidator, ScriptedNoteCategoryValidator>();
            services
                .AddTransient<ISexValidator, SexValidator>();
            services
                .AddTransient<ISexRepository, SexRepository>();
            services
                .AddTransient<ISiteValidator, SiteValidator>();
            services
                .AddTransient<ISiteRepository, SiteRepository>();
            services
                .AddTransient<ISubmitterInfoValidator, SubmitterInfoValidator>();
            services
                .AddTransient<ISubmitterInfoRepository, SubmitterInfoRepository>();
            services
                .AddTransient<ITaxAgencyValidator, TaxAgencyValidator>();
            services
                .AddTransient<ITaxAgencyRepository, TaxAgencyRepository>();
            services
                .AddTransient<ITaxGroupRepository, TaxGroupRepository>();
            services
                .AddTransient<ITaxGroupValidator, TaxGroupValidator>();
            services
                .AddTransient<ITaxItemValidator, TaxItemValidator>();
            services
                .AddTransient<ITaxItemRepository, TaxItemRepository>();
            services
                .AddTransient<IUserRepository, UserRepository>();
            services
                .AddTransient<IUserTaskRepository, UserTaskRepository>();
            services
                .AddTransient<IUserTaskValidator, UserTaskValidator>();
            services
                .AddTransient<IUserTaskTypeRepository, UserTaskTypeRepository>();
            services
                .AddTransient<IUserTaskTypeValidator, UserTaskTypeValidator>();
            services
                .AddTransient<IVendorAppointmentRepository, VendorAppointmentRepository>();
            services
                .AddTransient<IVendorPatientRepository, VendorPatientRepository>();
            services
                .AddTransient<IVendorHAUnitRepository, VendorHAUnitRepository>();

            return services;
        }
    }
}