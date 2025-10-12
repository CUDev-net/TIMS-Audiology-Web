



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
        public recurringItemNumber: number;
        public authorizationId: number;
        public billToProviderId: number;
        public startsAt: Date;
        public endsAt: Date;
        public nextContactDate: Date;
        public notificationStatus: NotificationStatus;
        public addToCancellationList: boolean;
        public otStatus: OpportunityStatusEnum;
        public otStatusDescriptionId: number;
        public otStatusDescription: OtStatusDescription;
        public notes: string;
        public custom1: string;
        public custom2: string;
        public slp: boolean;
        public createdDate: Date;
        public createdUserId: number;
        public updatedDate: Date;
        public updatedUserId: number;
        public appointmentType: AppointmentType;
        public appointmentStatus: AppointmentStatus;
        public patient: Patient;
        public recurringInterval: ApptRecurringInterval;
        public provider: Provider;
        public updatedByUserName: string;
        public site: Site;
        public opportunity: string;
        public opportunityDescription: string;
		
        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;
        
    }










