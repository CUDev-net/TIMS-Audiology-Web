using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.DAL
{
    public static class StartUp
    {
        public static IServiceCollection AddTimsXDataAccess(this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddTransient<IAdjustmentTypeUnitOfWork, AdjustmentTypeUnitOfWork>();
            services
                .AddTransient<IAppointmentsUnitOfWork, AppointmentsUnitOfWork>();
            services
	            .AddTransient<IApptAuthorizationUnitOfWork, ApptAuthorizationUnitOfWork>();
			services
                .AddTransient<IAppointmentTypeUnitOfWork, AppointmentTypeUnitOfWork>();
            services
                .AddTransient<IAppointmentStatusUnitOfWork, AppointmentStatusUnitOfWork>();
            services
                .AddTransient<IAppointmentTypeUnitOfWork, AppointmentTypeUnitOfWork>();
            services
                .AddTransient<IAuthorizationUnitOfWork, AuthorizationUnitOfWork>();
            services
	            .AddTransient<IAuthorizationReferenceUnitOfWork, AuthorizationReferenceUnitOfWork>();
			services
                .AddTransient<IBatterySizeUnitOfWork, BatterySizeUnitOfWork>();
            services
                .AddTransient<ICommunicationRestrictionUnitOfWork, CommunicationRestrictionUnitOfWork>();
            services
                .AddTransient<ICountyUnitOfWork, CountyUnitOfWork>();
            services
                .AddTransient<ICptCodeCategoryUnitOfWork, CptCodeCategoryUnitOfWork>();
            services
                .AddTransient<ICustomerMessageUnitOfWork, CustomerMessageUnitOfWork>();
            services
	            .AddTransient<IDescriptionUnitOfWork, DescriptionUnitOfWork>();
			services
                .AddTransient<IDiagnosisCodeUnitOfWork, DiagnosisCodeUnitOfWork>();
            services
                .AddTransient<IDiagnosisCodeCategoryUnitOfWork, DiagnosisCodeCategoryUnitOfWork>();
            services
	            .AddTransient<IEmplStatusUnitOfWork, EmplStatusUnitOfWork>();
			services
                .AddTransient<IGenderUnitOfWork, GenderUnitOfWork>();
            services
                .AddTransient<IHaComponentUnitOfWork, HaComponentUnitOfWork>();
            services
                .AddTransient<IHaModelUnitOfWork, HaModelUnitOfWork>();
            services
                .AddTransient<IHaModelOptionUnitOfWork, HaModelOptionUnitOfWork>();
            services
                .AddTransient<IHaRepairComplaintUnitOfWork, HaRepairComplaintUnitOfWork>();
            services
                .AddTransient<IHaReturnUnitOfWork, HaReturnUnitOfWork>();
            services
                .AddTransient<IHaReturnReasonUnitOfWork, HaReturnReasonUnitOfWork>();
            services
                .AddTransient<IHaStatusUnitOfWork, HaStatusUnitOfWork>();
            services
                .AddTransient<IHaStockItemStatusUnitOfWork, HaStockItemStatusUnitOfWork>();
            services
                .AddTransient<IHaStyleUnitOfWork, HaStyleUnitOfWork>();
            services
                .AddTransient<IHaTypeUnitOfWork, HaTypeUnitOfWork>();
            services
                .AddTransient<IHaWarrantyTypeUnitOfWork, HaWarrantyTypeUnitOfWork>();
            services
                .AddTransient<IHistoryUnitOfWork, HistoryUnitOfWork>();
            services
                .AddTransient<IHistoryTypeUnitOfWork, HistoryTypeUnitOfWork>();
            services
                .AddTransient<IInsurancePayerUnitOfWork, InsurancePayerUnitOfWork>();
            services
                .AddTransient<IKpiSiteTargetUnitOfWork, KpiSiteTargetUnitOfWork>();
            services
                .AddTransient<IHaHistoryUnitOfWork, HaHistoryUnitOfWork>();
            services
                .AddTransient<ILastPatientUnitOfWork, LastPatientUnitOfWork>();
            services
                .AddTransient<IManufacturerUnitOfWork, ManufacturerUnitOfWork>();
            services
	            .AddTransient<IMaritalStatusUnitOfWork, MaritalStatusUnitOfWork>();
			services
                .AddTransient<IMarketingCategoryUnitOfWork, MarketingCategoryUnitOfWork>();
            services
                .AddTransient<IMarketingReferenceUnitOfWork, MarketingReferenceUnitOfWork>();
            services
                .AddTransient<IMedicationUnitOfWork, MedicationUnitOfWork>();
            services
                .AddTransient<IMedicalConditionUnitOfWork, MedicalConditionUnitOfWork>();
            services
	            .AddTransient<IMessageSettingsUnitOfWork, MessageSettingsUnitOfWork>();
			services
                .AddTransient<IModifierUnitOfWork, ModifierUnitOfWork>();
            services
                .AddTransient<INdmAudiogramUnitOfWork, NdmAudiogramUnitOfWork>();
            services
                .AddTransient<IOutsideFacilityUnitOfWork, OutsideFacilityUnitOfWork>();
            services
	            .AddTransient<IPatientInsuranceUnitOfWork, PatientInsuranceUnitOfWork>();
			services
                .AddTransient<IPatientsUnitOfWork, PatientsUnitOfWork>();
            services
                .AddTransient<IPatientStatusUnitOfWork, PatientStatusUnitOfWork>();
            services
                .AddTransient<IPatientTypeUnitOfWork, PatientTypeUnitOfWork>();
            services
	            .AddTransient<IPatientTypeReferenceUnitOfWork, PatientTypeReferenceUnitOfWork>();
			services
                .AddTransient<IPatientRequiredFieldUnitOfWork, PatientRequiredFieldUnitOfWork>();
            services
                .AddTransient<IPaymentMethodUnitOfWork, PaymentMethodUnitOfWork>();
            services
                .AddTransient<IPosDocumentUnitOfWork, PosDocumentUnitOfWork>();
            services
                .AddTransient<IPracticeUnitOfWork, PracticeUnitOfWork>();
            services
                .AddTransient<IPreviousHistoryUnitOfWork, PreviousHistoryUnitOfWork>();
            services
                .AddTransient<IProvidersUnitOfWork, ProvidersUnitOfWork>();
            services
                .AddTransient<IProviderBlockScheduleUnitOfWork, ProviderBlockScheduleUnitOfWork>();
            services
                .AddTransient<IRecurringIntervalRemovedUnitOfWork, RecurringIntervalRemovedUnitOfWork>();
            services
                .AddTransient<IResourceUnitOfWork, ResourceUnitOfWork>();
            services
                .AddTransient<IResultUnitOfWork, ResultUnitOfWork>();
            services
                .AddTransient<IResultTypeUnitOfWork, ResultTypeUnitOfWork>();
            services
                .AddTransient <ISalutationUnitOfWork, SalutationUnitOfWork>();
            services
                .AddTransient<IScheduleUnitOfWork, ScheduleUnitOfWork>();
            services
                .AddTransient<IScheduleBlockUnitOfWork, ScheduleBlockUnitOfWork>();
            services
                .AddTransient<IScriptedNoteUnitOfWork, ScriptedNoteUnitOfWork>();
            services
                .AddTransient<IScriptedNoteCategoryUnitOfWork, ScriptedNoteCategoryUnitOfWork>();
            services
                .AddTransient<ISexUnitOfWork, SexUnitOfWork>();
            services
                .AddTransient<ISiteUnitOfWork, SiteUnitOfWork>();
            services
	            .AddTransient<IStudentStatusUnitOfWork, StudentStatusUnitOfWork>();
			services
                .AddTransient<ISubmitterInfoUnitOfWork, SubmitterInfoUnitOfWork>();
            services
                .AddTransient<ITaxAgencyUnitOfWork, TaxAgencyUnitOfWork>();
            services
                .AddTransient<ITaxGroupUnitOfWork, TaxGroupUnitOfWork>();
            services
                .AddTransient<ITaxItemUnitOfWork, TaxItemUnitOfWork>();
            services
                .AddTransient<ITimsUserSiteUnitOfWork, TimsUserSiteUnitOfWork>();
            services
                .AddTransient<IUserUnitOfWork, UserUnitOfWork>();
            services
                .AddTransient<IUserTaskUnitOfWork, UserTaskUnitOfWork>();
            services
                .AddTransient<IUserTaskTypeUnitOfWork, UserTaskTypeUnitOfWork>();
            
            return services;
        }
    }
}