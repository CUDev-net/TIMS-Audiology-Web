



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
        public siteId: number | null;
        public birthDate: Date | null;
        public gender: string;
        public maritalStatus: string;
        public otStatusId: OpportunityStatusEnum;
        public otStatusDescriptionId: number;
        public deathDate: Date | null;
        public createdDate: Date;
        public lastAppointmentDate: Date | null;
        public lastAppointmentStatus: string;
        public nextAppointmentDate: Date | null;
        public nextAppointmentStatus: string;
        public currentLeftHearingAid: string;
        public currentRightHearingAid: string;
        public appointments: AppointmentSummary[];
        public communicationRestrictions: CommunicationRestriction[];
        public updatedByUserName: string;
        public opportunity: string;
        public opportunityDescription: string;
        public inactive: boolean;
		
        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;
        
    }










