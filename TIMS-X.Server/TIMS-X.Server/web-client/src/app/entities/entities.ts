import { DateTime } from "luxon";

export module Entities {
    export enum day_of_week {
        sunday,
        monday,
        tuesday,
        wednesday,
        thursday,
        friday,
        saturday
    }

    export enum Severity {
        warning,
        error
    }

    export class EnumPair {
        display: string;
        value: number;
    }

    export enum SettingEnum {
        canDeleteAppointments = 10,
        canCreateAppointments = 18,
        canAccessAppointments = 48,
        canDeleteNonPatientAppointments = 95,
        canModifyAppointments = 114,
        canCreateNonPatientAppointments = 118,
        canModifyNonPatientAppointments = 119
    }

    export class CityStateLookup {
        public city: string;
        public state: string;
    }

    export enum OpportunityStatusEnum {
        NotSet = 0,
        TwoEarsAidable = 1,
        OneEarAidable = 2,
        ReSell2Ears = 3,
        ReSell1Ear = 4,
        NoMarketingOneEar = 5,
        NoOpportunity = 6,
        TestedNotSold = 7,
        TestedNotSold1Ear = 8,
        NoMarketing = 9,
        CurrentUser = 10
    }

    export class ScheduleItem {
        public id: string;
        public title: string;
        public patientId: number;
        public patient_name: string;
        public site_name: string;
        public site_web_color: string;
        public provider_name: string;
        public appointment_type_name: string;
        public appointment_type_web_color: string;
        public appointment_status_name: string;
        public start_date: Date;
        public end_date: Date;
        public type: string;
        public notes: string;
        public provider_web_color: string;
        public background_color: string;
        public isRecurring: boolean;
    }

    export class HoursOfOperationModel {
        public startTime: Date;
        public endTime: Date;
        public day: day_of_week
        public siteId: number
    }

    export class Ethnicity {
        public value: number;
        public name: string;
    }

    export class Race {
        public value: number;
        public name: string;
    }

    export class PreferredLanguage {
        public value: number;
        public name: string;
    }

    export class Sex {
        public name: string;
        public description: string;

        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;
    }

    export class ValidationResult {
        public message: string;
        public severity: Severity;
    }

    export class AppointmentStatus {
        public inactive: boolean;
        public name: string;
        public description: string;
        public protected: boolean;
        public show: boolean;
        public updatedUserId: number;
        public updatedDate: Date;

        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;
    }

    export class AppointmentType {
        public inactive: boolean;
        public name: string;
        public description: string;
        public protected: boolean;
        public duration: number;
        public color: number;
        public scheduleBlockId: number;
        public historyTypeId: number;
        public slp: boolean;
        public scheduleBlock: ScheduleBlock;
        public updatedUserId: number;
        public updatedDate: Date;

        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;
    }

    export class MarketingReferenceCategory {
        public inactive: boolean;
        public inUse: number;
        public name: string;
        public protected: boolean;
        public description: string;
        public updatedUserId: number;
        public updatedDate: Date;
        public marketingReferences: MarketingReference[];

        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;
    }

    export class MarketingReference {
        public inactive: boolean;
        public inUse: number;
        public name: string;
        public protected: boolean;
        public categoryId: number;
        public cost: number;
        public description: string;
        public startDate: Date;
        public endDate: Date;
        public notes: string;
        public reviewDate: Date;
        public sites: MarketingReferenceSite[];
        public updatedUserId: number;
        public updatedDate: Date;

        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;
    }

    export class MarketingReferenceSite {
        public marketingReferenceId: number;
        public siteId: number;

        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;

    }

    export class PracticeSummary {
        public inactive: boolean;
        public name: string;
        public fax: string;
        public email: string;
        public emailDisclaimer: string;
        public businessRules: string;
        public officeCode: string;
        public useBlockScheduling: boolean;
        public usesNoahDataMining: boolean;
        public usesAdAuthentication: boolean;
        public linkAppointmentHistory: boolean;
        public firstName: string;
        public lastName: string;
        public billingAddress1: string;
        public billingAddress2: string;
        public billingCity: string;
        public billingState: string;
        public billingZipCode: string;
        public billingPhoneNumber: string;
        public useSiteAddressForReports: boolean;
        public updatedDate: Date;
        public sites: Site[];
        public locale: string;

        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;
    }

    export class BusinessRuleDto {
        public onlineAppointmentPatientId: number;
        public onlinePatientAppointmentMessage: string;
        public requireAppointmentTypeForAppointment: boolean;
        public requireMarketingSourceForPatientAppointment: boolean;
        public useApptAuthorizations: boolean;
        public usesCalendarResources: boolean;
        public usesSlp: boolean;
    }

    export class Resource {
        public inactive: boolean;
        public inUse: number;
        public name: string;
        public siteId: number;
        public description: string;
        public color: number;
        public updatedUserId: number;
        public updatedDate: Date;

        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;
    }

    export class UserSiteHours {
        public userId: number;
        public siteId: number;
        public dayNum: number;
        public startTime: Date;
        public endTime: Date;
        public site: Site;

        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;
    }
    export class User {
        public inactive: boolean;
        public name: string;
        public initials: string;
        public adDomain: string;
        public adUsername: string;
        public isAdmin: boolean;
        public deleted: boolean;
        public isWebUser: boolean;
        public colorFrom: number;
        public calendarInterval: number;
        public password: string;
        public mobilePhone: string;
        public email: string;
        public requireMFA: boolean;
        public siteId: number;
        public scheduleProviderFilter: string;
        public scheduleSiteFilter: string;
        public scheduleResourceFilter: string;
        public scheduleSpecialtyFilter: string;
        public passwordChangedDate: Date;
        public userSettings: string;
        public jwt: string;
        public noahPermissions: number;
        public updatedUserId: number;
        public updatedDate: Date;
        public defaultMessageReceiver: boolean;
        public lastCalendarTimespan: number;
        //public lastPatientList: LastPatientList;
        public isSupport: boolean;
        public siteHours: UserSiteHours[];

        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;
    }

    export class CommunicationRestriction {
        public name: string;
        public description: string;
        public inUse: number;
        public inactive: boolean;
        public protected: boolean;
        public updatedDate: Date;
        public updatedUserId: number;

        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;
    }

    export class PatientSummary {
        public updatedDate: Date;
        public patientStatus: string;
        public patientType: string;
        public lastName: string;
        public deceased: boolean;
        public firstName: string;
        public initial: string;
        public address1: string;
        public address2: string;
        public city: string;
        public state: string;
        public zipCode: string;
        public primaryPhone: number;
        public homePhone: string;
        public workPhone: string;
        public otherPhone: string;
        public mobilePhone: string;
        public phoneToDisplay: string;
        public email: string;
        public contact: string;
        public siteId: number;
        public birthDate: Date;
        public gender: string;
        public maritalStatus: string;
        public otStatusId: OpportunityStatusEnum;
        public otStatusDescriptionId: number;
        public deathDate: Date;
        public createdDate: Date;
        // Don'y use restrictions
        public communicationRestrictions: CommunicationRestriction[];
        public lastAppointmentDate: Date;
        public lastAppointmentStatus: string;
        public nextAppointmentDate: Date;
        public nextAppointmentStatus: string;
        public currentLeftHearingAid: string;
        public currentRightHearingAid: string;
        public appointments: AppointmentSummary[];
        public updatedByUserName: string;
        public opportunity: string;
        public opportunityDescription: string;
        public inactive: boolean;

        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;
    }

    // For the patient summary
    export class AppointmentSummary {
        public patientId: number;
        public appointmentType: string;
        public appointmentStatus: string;
        public site: string;
        public providerFirstName: string;
        public providerLastName: string;
        public startsAt: Date;
        public endsAt: Date;
        public updatedDate: Date;

        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;
    }

    export class UserDto {
        public user: User;
        public lastPatientSummaries: PatientSummary[];
    }

    export class PatientSearchCriteriaDto {
        public dateOfBirth: Date | null;
        public email: string;
        public firstName: string;
        public includeInactive: boolean;
        public lastName: string;
        public phoneNumber: string;
    }

    export class Patient {
        //public guid: string;
        public inactive: boolean;
        public salutationId: number | null;
        public providerId: number | null;
        public primaryCareId: number | null;
        public referringPhysicianId: number | null;
        public marketingId: number | null;
        public siteId: number | null;
        public maritalStatusId: string;
        public emplStatusId: string;
        public patientStatusId: number | null;
        public patientTypeId: number | null;
        public otStatusId: OpportunityStatusEnum;
        public otStatusDescriptionId: number;
        public birthDate: Date | null;
        public deathDate: Date | null;
        public firstName: string;
        public lastName: string;
        public initial: string;
        public preferredName: string;
        public sex: string;
        public deceased: boolean;
        public email: string;
        public address1: string;
        public address2: string;
        public city: string;
        public state: string;
        public zipCode: string;
        public primaryPhone: number;
        public homePhone: string;
        public mobilePhone: string;
        public workPhone: string;
        public otherPhone: string;
        public accountNo: string;
        public notes: string;
        public customText1: string;
        public customText2: string;
        public customDate1: Date | null;
        public customDate2: Date | null;
        public ssn: string;
        public insuredInsurancePayerId: number | null;
        public updatedSiteId: number | null;
        public race: number;
        public ethnicity: number;
        public language: number;
        public legalRep: boolean;
        public legalRepFirstName: string;
        public legalRepLastName: string;
        public legalRepInitial: string;
        public legalRepAddress1: string;
        public legalRepAddress2: string;
        public legalRepCity: string;
        public legalRepState: string;
        public legalRepZipCode: string;
        public legalRepPhone: string;
        public alternateContact: string;
        public alternateContactPhone: string;
        public releaseSignature: boolean;
        public releaseSignatureDate: Date | null;
        public assignBenefits: boolean;
        public assignBenefitsDate: Date | null;
        public responsibleParty: string;
        public hasIntakeData: boolean;
        public lastFirstMiddle: string;
        public sexFull: string;
        //public salutation: Salutation;
        //public provider: Provider;
        //public primaryCarePhysician: MarketingReference;
        //public referringPhysician: MarketingReference;
        public updatedByUser: User;
        public createdUserId: number | null;
        public createdDate: Date;
        public privacyDate: Date | null;
        public updatedUserId: number | null;
        public updatedDate: Date;
        public useSecondaryAddress: boolean;
        public secondaryAddress1: string;
        public secondaryAddress2: string;
        public secondaryCity: string;
        public secondaryState: string;
        public secondaryZipCode: string;
        public qbid: string;
        public releaseInformation: boolean;
        public releaseInformationDate: Date | null;
        public consentDate: Date | null;
        public marketingAuthorization: boolean;
        public marketingAuthorizationDate: Date | null;
        public authorizedParties: string;
        public studentStatusId: string;
        public patientTypeIds: number[];
        public restrictionIds: number[];
        public authorizationIds: number[];
        public restrictions: PatientRestriction[];
        public patientTypeReferences: PatientTypeReference[];
        public authorizationReferences: AuthorizationReference[];
        //public medicalConditions: PreviousHistory[];

        public hasBeenAudited: boolean;

        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;
    }

    export class Site {
        public inactive: boolean;
        public practiceId: number;
        public name: string;
        public defaultTaxGroupId: number;
        public description: string;
        public address1: string;
        public address2: string;
        public city: string;
        public state: string;
        public zip: string;
        public cityStateZip: string;
        public phone: string;
        public qbid: string;
        public qbModifiedDate: Date;
        public monStart: Date;
        public monEnd: Date;
        public tuesStart: Date;
        public tuesEnd: Date;
        public wedStart: Date;
        public wedEnd: Date;
        public thurStart: Date;
        public thurEnd: Date;
        public friStart: Date;
        public friEnd: Date;
        public satStart: Date;
        public satEnd: Date;
        public sunStart: Date;
        public sunEnd: Date;
        public color: number;
        public siteSettingId: string;
        public echeckPaymentId: string;
        public ecreditCardPaymentId: string;
        public regionId: number;
        public npi: string;
        public secondaryIdqualifier: string;
        public secondaryIdnum: string;
        public ecareCreditPaymentId: string;
        public logo: number[];
        public careCreditPassword: string;
        public careCreditMerchantNumber: string;
        public careCreditPracticeCode: string;
        public allWellId: string;
        public faxNumber: string;
        public ccpromoCode: string;
        public customText1: string;
        public customText2: string;
        public customText3: string;
        public transnationalUsername: string;
        public transnationalPassword: string;
        public transnationalAuthKey: string;
        public outreachEducator: string;
        public updatedUserId: number;
        public updatedDate: Date;
        //public resources: Resource[];
        public webColor: string;

        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;
    }

    export class ProviderSummary {
        public inactive: boolean;
        public lastName: string;
        public usePracticeIds: boolean;
        public firstName: string;
        public initial: string;
        public degree: string;
        public deleted: boolean;
        public color: number;
        public npi: string;
        public displayOrder: number;
        public userId: number;
        public user: User;
        public simpleName: string;
        public fullName: string;
        public hours: HoursOfOperationModel[];
        public siteHours: UserSiteHours[];
        public webColor: string;

        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;
    }

    // For the scheduler
    export class AppointmentItemSummary {
        public id: string;
        public status_show: boolean;
        public appointment_type_id: number;
        public appointment_type_web_color: string;
        public patient_site_id: number;
        public provider_id: number;
        public provider_color_value: number;
        public provider_web_color: string;
        public status_id: number;
        public status_name: string;
        public notes: string;
        public patient_id: number;
        public patient_name: string;
        public provider_name: string;
        public appointment_type_name: string;
        public appointment_type_color_value: number;
        public site_id: number;
        public site_color_value: number;
        public site_web_color: string;
        public site_name: string;
        public start_date: Date;
        public end_date: Date;

    }

    // For the scheduler
    export class ScheduleItemSummary {
        public id: string;
        public location: string;
        public notes: string;
        public provider_id: number;
        public provider_color_value: number;
        public provider_name: string;
        public provider_web_color: string;
        public site_id: number;
        public site_color_value: number;
        public color: number;
        public color_web: string;
        public site_web_color: string;
        public site_name: string;
        public title: string;
        public start_date: Date;
        public end_date: Date;
    }

    export class ScheduleRecurringItemSummary {
        public id: string;
        public location: string;
        public notes: string;
        public provider_id: number;
        public provider_color_value: number;
        public provider_name: string;
        public provider_web_color: string;
        public site_id: number;
        public site_color_value: number;
        public color: number;
        public color_web: string;
        public site_web_color: string;
        public site_name: string;
        public title: string;
        public start_date: Date;
        public end_date: Date;
        public recurring_interval_id: number;
        public interval_type: number;
        public interval_sub_type: number;
        public day_interval: number;
        public week_interval: number;
        public is_monday_set: boolean;
        public is_tuesday_set: boolean;
        public is_wednesday_set: boolean;
        public is_thursday_set: boolean;
        public is_friday_set: boolean;
        public is_saturday_set: boolean;
        public is_sunday_set: boolean;
        public day_of_month: number;
        public month_interval: number;
        public day_of_week: number;
        public day_qualifier: number;
        public month: number;
        public end_type: number;
        public end_after: number;
        public recurrence_start_date: Date;
        public recurrence_end_date: Date;
        public rec_type: string;
        public event_length: string;
        public event_pid: string;
        public deletedOccurrences: number[];
    }

    export class AppointmentDto {
        public appointment: Appointment;
        public appointment_Type_Color: string;
        public provider_Color: string;
        public providerName: string;
        public site_Color: string;
        public siteName: string;
        public typeName: string;
        public patientName: string;
    }

    export class Appointment {
        public patientId: number;
        public updatedSiteId: number;
        public appointmentTypeId: number;
        public providerId: number;
        public appointmentStatusId: number;
        public siteId: number;
        public syncSiteId: number;
        public resourceId: number;
        public marketingId: number;
        public referringPhysicianId: number;
        public recurringIntervalId: number;
        public recurringParentId: number;
        public authorizationId: number;
        public billToProviderId: number;
        public startsAt: Date;
        public endsAt: Date;
        public nextContactDate: Date;
        //public notificationStatus: NotificationStatus;
        public addToCancellationList: boolean;
        public otStatus: OpportunityStatusEnum;
        public otStatusDescriptionId: number;
        //public otStatusDescription: OtStatusDescription;
        public notes: string;
        public custom1: string;
        public custom2: string;
        public slp: boolean;
        public createdDate: Date;
        public createdUserId: number;
        public updatedDate: Date;
        public updatedUserId: number;
        public updatedByUserName: string;
        //public appointmentType: AppointmentType;
        //public appointmentStatus: AppointmentStatus;
        //public patient: Patient;
        public recurringInterval: ApptRecurringInterval;
        //public provider: Provider;
        //public site: Site;
        public opportunity: string;
        public opportunityDescription: string;

        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;
    }

    export class ApptRecurringInterval {
        //public deletedOccurrences: ApptRecurringIntervalRemoved[];

        public intervalType: number;
        public subType: number;
        public dayInterval: number;
        public weekInterval: number;
        public isMondaySet: boolean;
        public isTuesdaySet: boolean;
        public isWednesdaySet: boolean;
        public isThursdaySet: boolean;
        public isFridaySet: boolean;
        public isSaturdaySet: boolean;
        public isSundaySet: boolean;
        public dayOfMonth: number;
        public monthInterval: number;
        public dayOfWeek: number;
        public dayQualifier: number;
        public month: number;
        public endType: number;
        public endOccurs: number;
        public startDate: Date;
        public endDate: Date;
        public createdDate: Date;
        public updatedDate: Date;
    }

    export class ScheduleDto {
        public provider_Color: string;
        public providerName: string;
        public schedule: Schedule;
        public recurringItemSummary: ScheduleRecurringItemSummary;
        public site_Color: string;
        public siteName: string;
    }

    export class Schedule {
        public startsAt: Date;
        public endsAt: Date;
        public title: string;
        public location: string;
        public notes: string;
        public recurringIntervalId: number;
        public siteId: number;
        public siteIds: number[];
        public providerId: number;
        public providerIds: number[];
        public color: number;
        public updatedUserId: number;
        public updatedSiteId: number;
        public updatedDate: Date;
        public createdUserId: number;
        public createdDate: Date;
        public web_Color: string;
        public updatedByUserName: string;
        public recurringInterval: RecurringInterval;

        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;
    }
    export class RecurringInterval {
        public deletedOccurrences: RecurringIntervalRemoved[];

        public intervalType: number;
        public subType: number;
        public dayInterval: number;
        public weekInterval: number;
        public isMondaySet: boolean;
        public isTuesdaySet: boolean;
        public isWednesdaySet: boolean;
        public isThursdaySet: boolean;
        public isFridaySet: boolean;
        public isSaturdaySet: boolean;
        public isSundaySet: boolean;
        public dayOfMonth: number;
        public monthInterval: number;
        public dayOfWeek: number;
        public dayQualifier: number;
        public month: number;
        public endType: number;
        public endOccurs: number;
        public startDate: Date;
        public endDate: Date;

        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;
    }

    export class RecurringIntervalRemoved {
        public scheduleId: number;
        public recurringIntervalId: number;
        public itemNumber: number;
        public itemDate: Date;

        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;
    }

    export class ScheduleBlock {
        public createdDate: Date;
        public name: string;
        public color: number;
        public color_web: string;
        public notes: string;
        public rowVersion: number[];
        public updatedUserId: number;
        public updatedDate: Date;
        public startDate: Date;
        public endDate: Date;
        public appointmentTypes: string[];
        public providerBlockSchedules: ProviderBlockSchedule[];

        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;
    }

    export class ProviderBlockSchedule {
        public providerId: number;
        public scheduleBlockId: number;
        public scheduleTimeSlotId: number;
        public createdDate: Date;
        public rowVersion: number[];
        //public scheduleBlock: ScheduleBlock;
        public scheduleTimeSlot: ScheduleTimeSlot;

        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;
    }

    export class ScheduleTimeSlot {
        public dayOfWeek: number;
        public startTime: Date;
        public endTime: Date;
        public createdDate: Date;
        public rowVersion: number[];

        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;
    }

    export class ScheduleOpeningsSearchModel {
        public appointmentTypeId: number;
        public durationHours: number;
        public durationMinutes: number;
        public durationTotalMinutes: number;
        public endDate: Date;
        public providerIds: number[];
        public resourceIds: number[];
        public siteIds: number[];
        public startDate: Date;
    }

    export class ScheduleOpeningModel {
        public endsAt: Date;
        public providerFirstName: string;
        public providerId: number;
        public providerLastName: string;
        public providerMiddleInitial: string;
        public resourceId: number;
        public resourceName: string;
        public siteId: number;
        public siteName: string;
        public startsAt: Date;
        public durationTotalMinutes: number;

        //Client side only
        public dayOfMonth: number;
        public dayOfWeek: string;
        public time: string;
    }

    export class MonthData {
        public label: string;
        public openings: ScheduleOpeningModel[];
    }

    export class PatientRequiredField {
        public fieldId: number;
        public fieldName: string;
        public required: boolean;

        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;
    }

    export enum PatientRequiredFieldEnum {
        seenBy = 3,
        salutation = 4,
        initial = 6,
        address1 = 8,
        address2 = 9,
        city = 10,
        state = 11,
        zip = 12,
        primaryPhone = 13,
        workPhone = 14,
        workPhoneExt = 15,
        mobilePhone = 16,
        homePhone = 17,
        homePhoneExt = 18,
        dob = 19,
        gender = 20,
        otherPhone = 21,
        otherPhoneExt = 22,
        email = 23,
        ssn = 24,
        alternateContact = 25,
        language = 26,
        alternatePhone = 27,
        race = 28,
        primaryCare = 29,
        ethnicity = 30,
        referringPhysician = 31,
        qbBillTo = 33,
        qbCustomerType = 34,
        county = 35,
        releaseSignature = 36,
        assignmentOfBenefits = 37,
        privacyAgreement = 38,
        consent = 39,
        customText1 = 40,
        customText2 = 41,
        customDate1 = 42,
        customDate2 = 43,
        patientType = 44,
    }

    export class PatientScheduledAppointmentDto {
        public appointmentTypeId: number;
        public birthDate: Date;
        public date: Date;
        public email: string;
        public firstName: string;
        public isNewPatient: boolean;
        public lastName: string;
        public middleInitial: string;
        public phone: string;
        public providerId: number;
        public reason: string;
        public siteId: number;
    }

    export class PracticeDto {
        public locale: string;
        public name: string;
        public onlinePatientAppointmentMessage: string;
    }

    export class CreatedPatientAppointmentDto {
        public appointment: Appointment;
        public emailMessage: string;
        public message: string;
        public pendingMessage: string;
        public practiceMessage: string;
    }

    export class TimeSlotDto {
        public timeSlot: Date;
        public providerId: number;
    }

    export class PatientAppointmentItemDto {
        public appointmentId: number;
        public birthDate: string;
        public date: string;
        public dateReceived: string;
        public email: string;
        public firstName: string;
        public initial: string;
        public lastName: string;
        public phone: string;
        public provider: string;
        public site: string;
    }

    export class PatientCandidateDto {
        public dateLastUpdated: string;
        public dateOfBirth: string;
        public email: string;
        public firstName: string;
        public initial: string;
        public lastName: string;
        public phone: string;
        public patientId: number;
    }

    export class PatientLinkDto {
        public appointmentId: number;
        public dateLastUpdated: Date | null;
        public dateOfBirth: Date | null;
        public email: string;
        public firstName: string;
        public initial: string;
        public lastName: string;
        public phone: string;
        public patientId: number;
    }

    export class InsurancePayer {
        public inactive: boolean;
        public inUse: number;
        public name: string;
        public protected: boolean;
        public organizationId: string;
        public claimOfficeNum: string;
        public insuranceType: string;
        public providerId: string;
        public secondaryId: string;
        public contact: string;
        public address1: string;
        public address2: string;
        public city: string;
        public state: string;
        public zipCode: string;
        public phone: string;
        public extension: string;
        public fax: string;
        public notes: string;
        public emcProviderId: string;
        public secondaryIdQualifier: string;
        public qbId: string;
        public dtQbModified: Date;
        public rowVersion: number[];
        public isInstitutional: boolean;
        public carrierCode: string;
        public npi: string;
        public priCarrierCodeReq: boolean;
        public isIcd10: boolean;
        public email: string;
        public acceptsFaxAttachments: boolean;
        public acceptsEmailAttachments: boolean;
        public acceptsElectronicAttachments: boolean;
        public mipsRequired: boolean;
        public updatedUserId: number;
        public updatedDate: Date;

        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;

    }

    export class MedicareSecondaryCodeDto {
        public name: string;
        public description: string;

    }

    export class PatientRelationDto {
        public id: number;
        public name: string;
        public description: string;

    }

    export class MaritalStatus {
        public description: string;
        public name: string;

        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;

    }

    export class EmplStatus {
        public description: string;
        public name: string;

        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;

    }

    export class StudentStatus {
        public description: string;
        public name: string;

        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;

    }

    export class PatientType {
        public inactive: boolean;
        public name: string;
        public description: string;
        public protected: boolean;
        public inUse: number;
        public parentID: number | null;
        public dateQuickBooksModified: Date | null;
        public quickBooksID: string;
        public updatedUserId: number | null;
        public updatedDate: Date;

        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;

    }

    export class PatientStatus {
        public inactive: boolean;
        public name: string;
        public description: string;
        public protected: boolean;
        public inUse: number;
        public updatedUserId: number | null;
        public updatedDate: Date;

        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;

    }

    export class Authorization {
        public name: string;
        public inactive: boolean;
        public description: string;
        public protected: boolean;
        public updatedUserId: number;
        public updatedDate: DateTime;

        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;

    }

    export class PatientRestriction {
        public patientId: number | null;
        public patient: Patient;
        public restrictionId: number;
        public communicationRestriction: CommunicationRestriction;

        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;

    }

    export class PatientTypeReference {
        public patientTypeId: number;
        public patientId: number;
        public patient: Patient;
        public createdDate: Date;
        public patientType: PatientType;

        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;

    }

    export class AuthorizationReference {
        public authorizationId: number;
        public patientId: number;
        public patient: Patient;
        public createdDate: Date;
        public authorization: Authorization;

        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;

    }

    export class Description {
        public updatedDate: Date;
        public updatedUserId: number | null;
        public customDate1Label: string;
        public customDate2Label: string;
        public customText1Label: string;
        public customText2Label: string;

        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;

    }

    export class PatientInsurance {
        public patientId: number;
        public payerLevel: number;
        public relationtoInsured: string;
        public patientSignatureDate: Date;
        public insurancePayerId: number;
        public idNumber: string;
        public policyGroupNum: string;
        public policyGroupName: string;
        public policyType: string;
        public firstName: string;
        public middleName: string;
        public lastName: string;
        public address1: string;
        public address2: string;
        public city: string;
        public state: string;
        public zipCode: string;
        public phone: string;
        public workPhone: string;
        public birthDate: Date;
        public sex: string;
        public employmentStatus: string;
        public employer: string;
        public retireDate: Date;
        public signatureDate: Date;
        public insurNotes: string;
        public createdDate: Date;
        public lastModifiedBy: number;
        public claimFilingIndicator: string;
        public insuredAssignmentSig: boolean;
        public carrierCode: string;
        public InsurancePayer: InsurancePayer;

        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;

    }

    export class ApptAuthorization {
        public authorizations: number | null;
        public expires: Date | null;
        public inactive: boolean;
        public isDeleted: boolean;
        public name: string;
        public notes: string;
        public numberUsed: number;
        public displayString: string;
        public patientId: number;
        public updatedUserId: number | null;
        public updatedDate: Date;

        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;

    }
}