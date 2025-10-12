using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using TIMS_X.Core.Attributes;

namespace TIMS_X.Core.Enums
{
    public enum PermissionTypeEnum
    {
        /// <summary>
		/// Uses Questionnaire
		/// </summary>
		[Description("Uses Questionnaire"), Area(SettingAreaEnum.History)]
        UseQuestionnaire = 0,
        /// <summary>
        /// Uses Noah
        /// </summary>
        [Description("Uses Noah"), Area(SettingAreaEnum.History)]
        UseNoah = 1,
        /// <summary>
        /// Uses Noah
        /// </summary>
        [Description("Uses Audiogram"), Area(SettingAreaEnum.History)]
        UseAudiogram = 2,
        /// <summary>
        /// 
        /// </summary>
        [Description("Uses Results"), Area(SettingAreaEnum.History)]
        UseResults = 3,
        /// <summary>
        /// 
        /// </summary>
        [Description("Can Finalize Superbill"), Area(SettingAreaEnum.History)]
        CanLockSuperbill = 4,
        /// <summary>
        /// 
        /// </summary>
        [Description("Can Complete Questionnaire"), Area(SettingAreaEnum.History)]
        CanCompleteQuestionnaire = 5,
        /// <summary>
        /// Can access superbill
        /// </summary>
        [Description("Can Access Superbill"), Area(SettingAreaEnum.History)]
        CanAccessSuperbill = 6,
        /// <summary>
        /// 
        /// </summary>
        [Description("Can Delete Patients"), Area(SettingAreaEnum.Patient)]
        CanDeletePatients = 9,
        /// <summary>
        /// 
        /// </summary>
        [Description("Can Delete Appointments"), Area(SettingAreaEnum.Appointment)]
        CanDeleteAppointments = 10,
        /// <summary>
        /// 
        /// </summary>
        [Description("Can Delete Histories"), Area(SettingAreaEnum.History)]
        CanDeleteHistory = 11,
        /// <summary>
        /// 
        /// </summary>
        [Description("Can Delete HA Histories"), Area(SettingAreaEnum.HAHistory)]
        CanDeleteHAHistory = 12,

        /// <summary>
        /// 
        /// </summary>
        [Description("Can Access POS"), Area(SettingAreaEnum.POS)]
        CanAccessPOS = 14,
        /// <summary>
        /// 
        /// </summary>
        [Description("Can Delete Transactions"), Area(SettingAreaEnum.Claims)]
        CanDeleteTransactions = 15,
        /// <summary>
        /// 
        /// </summary>
        [Description(@"Can Create\Delete POS Items"), Area(SettingAreaEnum.POS)]
        CanCreateDeletePOS = 16,
        /// <summary>
        /// 
        /// </summary>
        [Description("Can Void POS Items"), Area(SettingAreaEnum.POS)]
        CanVoidPOS = 17,
        /// <summary>
        /// Can Add Modify Appointments
        /// </summary>
        [Description(@"Can Create\Modify Appointments"), Area(SettingAreaEnum.Appointment)]
        CanCreateModifyAppointments = 18,
        /// <summary>
        /// Can Add Modify Histories
        /// </summary>
        [Description(@"Can Create\Modify Histories"), Area(SettingAreaEnum.History)]
        CanCreateModifyHistory = 19,
        /// <summary>
        /// Can Add Modify Histories
        /// </summary>
        [Description(@"Can Modify Adjustment Accounts"), Area(SettingAreaEnum.QB)]
        CanModifyAdjustmentAccounts = 20,
        /// <summary>
        /// Can View Patient Balances
        /// </summary>
        [Description(@"Can View Patient Balances"), Area(SettingAreaEnum.POS)]
        CanViewPatientBalance = 21,
        /// <summary>
        /// Can Can Select Income Accounts in Claims Processing
        /// </summary>
        [Description(@"Can Select Income Accounts in Claims Processing"), Area(SettingAreaEnum.QB)]
        CanSelectIncomeAccountsInClaimsProcessing = 22,
        /// <summary>
        /// Can Edit Reports
        /// </summary>
        [Description(@"Can Add/Edit Reports"), Area(SettingAreaEnum.Reports)]
        CanEditReports = 23,
        /// <summary>
        /// Can Access Transactsion tab
        /// </summary>
        [Description(@"Uses Transactions"), Area(SettingAreaEnum.Claims)]
        CanAccessTransactsion = 24,
        /// <summary>
        /// Can Access Claims tab
        /// </summary>
        [Description(@"Uses Processing"), Area(SettingAreaEnum.Claims)]
        CanAccessClaimsProcessing = 25,
        /// <summary>
        /// Can Access Adjudication tab
        /// </summary>
        [Description(@"Uses Adjudication"), Area(SettingAreaEnum.Claims)]
        CanAccessAdjudication = 26,
        /// <summary>
        /// Can Access Adjudication tab
        /// </summary>
        [Description(@"Uses Appointment Comparison"), Area(SettingAreaEnum.Dashboard)]
        CanAccessAppointmentComparison = 27,
        /// <summary>
        /// Can Access Adjudication tab
        /// </summary>
        [Description(@"Uses Hearing Aid Summary"), Area(SettingAreaEnum.Dashboard)]
        CanAccessHASummary = 28,
        /// <summary>
        /// Can Access Adjudication tab
        /// </summary>
        [Description(@"Uses Current Opportunities"), Area(SettingAreaEnum.Dashboard)]
        CanAccessCurrentOpportunities = 29,
        /// <summary>
        /// Can Access Adjudication tab
        /// </summary>
        [Description(@"Uses Opportunity Tracking"), Area(SettingAreaEnum.Dashboard)]
        CanAccessOpporunityTracking = 30,
        /// <summary>
        /// Can Access Adjudication tab
        /// </summary>
        [Description(@"Uses Outstanding Claims"), Area(SettingAreaEnum.Dashboard)]
        CanAccessOutstandingClaims = 31,
        /// <summary>
        /// Can Access Adjudication tab
        /// </summary>
        [Description(@"Uses Sales By Marketing"), Area(SettingAreaEnum.Dashboard)]
        CanAccessSalesByMarketing = 32,
        /// <summary>
        /// Can Access Adjudication tab
        /// </summary>
        [Description(@"Uses Sales By source"), Area(SettingAreaEnum.Dashboard)]
        CanAccessSalesBySource = 33,
        /// <summary>
        /// Can Access Adjudication tab
        /// </summary>
        [Description(@"Uses Current Patient Communications"), Area(SettingAreaEnum.Marketing)]
        CanAccessTelemarketing = 34,
        /// <summary>
        /// Can Access Adjudication tab
        /// </summary>
        [Description(@"Uses Billing Center"), Area(SettingAreaEnum.SystemCenter)]
        CanAccessBillingCenter = 35,
        /// <summary>
        /// Can Access Adjudication tab
        /// </summary>
        [Description(@"Uses Imaging Center"), Area(SettingAreaEnum.SystemCenter)]
        CanAccessImagingCenter = 36,
        /// <summary>
        /// Can Access Adjudication tab
        /// </summary>
        [Description(@"Uses Notification Center"), Area(SettingAreaEnum.SystemCenter)]
        CanAccessNotificationCenter = 37,
        /// <summary>
        /// Can Access Adjudication tab
        /// </summary>
        [Description(@"Uses Stock Item"), Area(SettingAreaEnum.SystemCenter)]
        CanAccessStockItemCenter = 38,
        /// <summary>
        /// Can Access Adjudication tab
        /// </summary>
        [Description(@"Uses Hearing Aid History"), Area(SettingAreaEnum.HAHistory)]
        CanAccessHAHistory = 40,
        /// <summary>
        /// Can Access Adjudication tab
        /// </summary>
        [Description(@"Can Change Appointment on Invoice"), Area(SettingAreaEnum.POS)]
        CanChangeInvoiceAppointment = 41,
        /// <summary>
        /// Can Edit Reports
        /// </summary>
        [Description(@"Uses Standard Reports"), Area(SettingAreaEnum.Reports)]
        CanAccessStandardReports = 42,
        /// <summary>
        /// Can Edit Reports
        /// </summary>
        [Description(@"Can Process CareCredit Applications/Lookups"), Area(SettingAreaEnum.Financing)]
        CanProcessCareCreditApplications = 43,
        /// <summary>
        /// Can Edit Reports
        /// </summary>
        [Description(@"Can Process CareCredit Purchases"), Area(SettingAreaEnum.Financing)]
        CanProcessCareCreditPurchases = 44,
        /// <summary>
        /// Can Edit Reports
        /// </summary>
        [Description(@"Can Process CareCredit Returns"), Area(SettingAreaEnum.Financing)]
        CanProcessCareCreditReturns = 45,
        /// <summary>
        /// Can Access Battery Center link
        /// </summary>
        [Description(@"Uses Battery Center"), Area(SettingAreaEnum.SystemCenter)]
        CanAccessBatteryCenter = 46,
        /// <summary>
        /// Can Access Battery Center link
        /// </summary>
        [Description(@"Uses History"), Area(SettingAreaEnum.History)]
        CanAccessHistories = 47,
        /// <summary>
        /// Can Access Battery Center link
        /// </summary>
        [Description(@"Can View Appointments"), Area(SettingAreaEnum.Appointment)]
        CanAccessAppointments = 48,
        /// <summary>
        /// Can Merge Patients
        /// </summary>
        [Description(@"Merges Patients"), Area(SettingAreaEnum.Patient)]
        CanMergePatients = 49,
        /// <summary>
        /// Can Merge Patients
        /// </summary>
        [Description(@"Uses Communications Setup"), Area(SettingAreaEnum.Marketing)]
        CanAccessMarketingSetup = 50,
        /// <summary>
        /// Can Merge Patients
        /// </summary>
        [Description(@"Uses Patient Setup"), Area(SettingAreaEnum.Patient)]
        CanAccessPatientSetup = 51,
        /// <summary>
        /// Can Merge Patients
        /// </summary>
        [Description(@"Uses History Setup"), Area(SettingAreaEnum.History)]
        CanAccessHistorySetup = 52,
        /// <summary>
        /// Can Merge Patients
        /// </summary>
        [Description(@"Uses HA History Setup"), Area(SettingAreaEnum.HAHistory)]
        CanAccessHAHistorySetup = 53,
        /// <summary>
        /// Can Merge Patients
        /// </summary>
        [Description(@"Uses Appointment Setup"), Area(SettingAreaEnum.Appointment)]
        CanAccessAppointmentSetup = 54,
        /// <summary>
        /// 
        /// </summary>
        [Description("Can Lock History Record"), Area(SettingAreaEnum.History)]
        CanLockHistory = 55,
        /// <summary>
        /// Can Merge Patients
        /// </summary>
        [Description(@"Uses POS Setup"), Area(SettingAreaEnum.POS)]
        CanAccessPOSSetup = 56,
        /// <summary>
        /// Can Merge Patients
        /// </summary>
        [Description(@"Uses Site Pricing Setup"), Area(SettingAreaEnum.POS)]
        CanAccessSitePricingSetup = 57,
        /// <summary>
        /// Can Merge Patients
        /// </summary>
        [Description(@"Uses Claims Setup"), Area(SettingAreaEnum.Claims)]
        CanAccessClaimsSetup = 58,
        /// <summary>
        /// Can Delete Patient Communication Interactions
        /// </summary>
        [Description(@"Can Delete Patient Communication Interactions"), Area(SettingAreaEnum.Marketing)]
        CanDeletePatientInteraction = 59,
        /// <summary>
        /// Uses Questionnaire Setup
        /// </summary>
        [Description(@"Uses Questionnaire Setup"), Area(SettingAreaEnum.History)]
        CanAccessQuestionnaireSetup = 60,
        /// <summary>
        /// Uses User Group Setup
        /// </summary>
        [Description(@"Uses User Setup"), Area(SettingAreaEnum.Setup)]
        CanAccessUserSetup = 61,
        /// <summary>
        /// Uses Imaging Setup
        /// </summary>
        [Description(@"Uses Imaging Setup"), Area(SettingAreaEnum.Setup)]
        CanAccessImagingSetup = 62,
        /*        /// <summary>
				/// Can Edit POS Line Item Prices
				/// </summary>
				[Description(@"Can Edit POS Line Item Prices"), Area(SettingAreaEnum.POS)]
				CanEditPOSLineItemPrices = 63,*/
        /// <summary>
        /// Can Link/Unlink HAHistory records
        /// </summary>
        [Description(@"Can Link/Unlink HAHistories"), Area(SettingAreaEnum.HAHistory)]
        CanLinkHAHistories = 64,
        /// <summary>
        /// Can Edit Usernames
        /// </summary>
        [Description(@"Can Edit Usernames"), Area(SettingAreaEnum.Setup)]
        CanChangeUserNames = 65,
        /// <summary>
        /// Can Edit Usernames
        /// </summary>
        [Description(@"Uses Hearing Aid Pricing"), Area(SettingAreaEnum.HAHistory)]
        UsesHAPricing = 66,
        /// <summary>
        /// Can Edit Usernames
        /// </summary>
        [Description(@"Can Delete Patient Reports/Correspondence"), Area(SettingAreaEnum.Patient)]
        CanDeletePatientReports = 67,
        /// <summary>
        /// Can Edit Usernames
        /// </summary>
        [Description(@"Edit Service Line Items"), Area(SettingAreaEnum.POS)]
        CanEditServicePrice = 68,
        /// <summary>
        /// Can Edit Usernames
        /// </summary>
        [Description(@"Edit Inventory Line Items"), Area(SettingAreaEnum.POS)]
        CanEditInventoryPartPrice = 69,
        /// <summary>
        /// Can Edit Usernames
        /// </summary>
        [Description(@"Edit Non-Inventory Line Items"), Area(SettingAreaEnum.POS)]
        CanEditNonInventoryPartPrice = 70,
        /// <summary>
        /// Can Edit Usernames
        /// </summary>
        [Description(@"Edit Other Line Items"), Area(SettingAreaEnum.POS)]
        CanEditOtherPrice = 71,
        /// <summary>
        /// Can Edit Usernames
        /// </summary>
        [Description(@"Edit Discount Line Items"), Area(SettingAreaEnum.POS)]
        CanEditDiscountPrice = 72,
        /// <summary>
        /// Can Edit Usernames
        /// </summary>
        [Description(@"Edit Battery Line Items"), Area(SettingAreaEnum.POS)]
        CanEditBatteryPrice = 73,
        /// <summary>
        /// Can Edit Usernames
        /// </summary>
        [Description(@"Edit Hearing Aid Line Items"), Area(SettingAreaEnum.POS)]
        CanEditHearingAidPrice = 74,
        /// <summary>
        /// Can Edit Usernames
        /// </summary>
        [Description(@"Edit CPT Code Line Items"), Area(SettingAreaEnum.POS)]
        CanEditCPTCodPrice = 75,
        /// <summary>
        /// Can Access KPI Setup
        /// </summary>
        [Description(@"Uses KPI Setup"), Area(SettingAreaEnum.Setup)]
        CanAccessKPISetup = 76,
        /// <summary>
        /// Can Access KPI Setup
        /// </summary>
        [Description(@"Uses Patient Notification Setup"), Area(SettingAreaEnum.Setup)]
        CanAccessNotificationSetup = 77,
        /// <summary>
        /// Can Access Adjudication tab
        /// </summary>
        [Description(@"Uses KPI"), Area(SettingAreaEnum.Dashboard)]
        CanAccessKPIDashboard = 78,
        /// <summary>
        /// Can Access Adjudication tab
        /// </summary>
        [Description(@"Uses Payer Payments"), Area(SettingAreaEnum.Dashboard)]
        CanAccessPayerPaymentsDashboard = 79,
        /// <summary>
        /// Can Access Practice Setup
        /// </summary>
        [Description(@"Uses Practice Setup"), Area(SettingAreaEnum.Setup)]
        CanAccessPracticeSetup = 80,
        /// <summary>
        /// Can Access Practice Setup
        /// </summary>
        [Description(@"Can Create\Edit Patient Letters"), Area(SettingAreaEnum.Marketing)]
        CanCreateEditLetters = 81,
        /// <summary>
        /// Can Access Practice Setup
        /// </summary>
        [Description(@"Can Print Patient Letters"), Area(SettingAreaEnum.Marketing)]
        CanPrintLetters = 82,
        /// <summary>
        /// Can Access Practice Setup
        /// </summary>
        [Description(@"Can Delete Patient Letters"), Area(SettingAreaEnum.Marketing)]
        CanDeleteLetters = 83,
        /// <summary>
        /// Can Access Practice Setup
        /// </summary>
        [Description(@"Can Send Notifications"), Area(SettingAreaEnum.Appointment)]
        CanSendNotifications = 84,
        /// <summary>
        /// Can view claims
        /// </summary>
        [Description(@"Can View Claims"), Area(SettingAreaEnum.Claims)]
        CanViewClaims = 85,
        /// <summary>
        /// Can send patient emails
        /// </summary>
        [Description(@"Can Email Patient Letters"), Area(SettingAreaEnum.Marketing)]
        CanEmailPatientLetters = 86,
        /// <summary>
		/// Can send patient emails
		/// </summary>
		[Description(@"Uses User Group Setup"), Area(SettingAreaEnum.Setup)]
        CanAccessUserGroupSetup = 87,
        /// <summary>
        /// Can inactivate images
        /// </summary>
        [Description(@"Can Inactivate Images"), Area(SettingAreaEnum.Imaging)]
        CanInactivateImages = 88,
        /// <summary>
        /// Can inactivate images
        /// </summary>
        [Description(@"Can Request Patient Eligibility"), Area(SettingAreaEnum.Claims)]
        CanRequestPatientEligibility = 89,
        /// <summary>
        /// Can process all tasks
        /// </summary>
        [Description(@"Can View/Delete All Tasks"), Area(SettingAreaEnum.Tasks)]
        CanViewDeleteAllTasks = 90,
        /// <summary>
        /// Can process All Well
        /// </summary>
        [Description(@"Can Process AllWell Applications"), Area(SettingAreaEnum.Financing)]
        CanProcessAllWellApplication = 91,
        /// <summary>
        /// Can Edit Reports
        /// </summary>
        [Description(@"Can Fax Reports Electronically"), Area(SettingAreaEnum.Reports)]
        CanEFaxReports = 92,
        /// <summary>
        /// Can Create/Modify HA Histories
        /// </summary>
        [Description(@"Can Create\Modify HA Histories"), Area(SettingAreaEnum.HAHistory)]
        CanCreateModifyHAHistory = 93,
        /// <summary>
        /// 
        /// </summary>
        [Description(@"Uses Patient Reports Designer"), Area(SettingAreaEnum.Reports)]
        CanAccessPatientReportsDesigner = 94,

        /// <summary>
        /// 
        /// </summary>
        [Description("Can Delete Non-Patient Appointments"), Area(SettingAreaEnum.Appointment)]
        CanDeleteNonPatientAppointments = 95,

        /// <summary>
        /// 
        /// </summary>
        [Description("Can Access HL7 Center"), Area(SettingAreaEnum.HL7)]
        CanAccessHL7Center = 96,

        /// <summary>
        /// 
        /// </summary>
        [Description("Can Access HL7 Config"), Area(SettingAreaEnum.HL7)]
        CanAccessHL7Config = 97,

        /// <summary>
        /// 
        /// </summary>
        [Description("Can Access GL Export"), Area(SettingAreaEnum.Financing)]
        CanAccessGLExport = 98,

        /// <summary>
        /// 
        /// </summary>
        [Description("Can Access Statement Export"), Area(SettingAreaEnum.Financing)]
        CanAccessStatementExport = 99,

        /// <summary>
        /// Added enum option to determine if user is allowed to export full patient list per Mantis 4061 NEP
        /// </summary>
        [Description("Can Export Patient List"), Area(SettingAreaEnum.Marketing)]
        CanExportPatientList = 100,

        /// <summary>
        /// Added enum option to determine if user is allowed to export full patient list per Mantis 3991 NEP
        /// </summary>
        [Description("Can Update Accounts and Mapping"), Area(SettingAreaEnum.Financing)]
        CanUpdateMappingsandAccounts = 101,

        /// <summary>
        /// Added enum option to determine if user is allowed to delete completed task -  Mantis 3287 NEP
        /// </summary>
        [Description("Can Delete Completed Tasks"), Area(SettingAreaEnum.Tasks)]
        CanDeleteCompletedTasks = 102,

        /// <summary>
        /// Can view audits
        /// </summary>
        [Description("Can Access Audits"), Area(SettingAreaEnum.SystemCenter)]
        CanAccessAudits = 103,

        [Description("Can Access Setup"), Area(SettingAreaEnum.Setup)]
        CanAccessSetup = 104,
    }
}
