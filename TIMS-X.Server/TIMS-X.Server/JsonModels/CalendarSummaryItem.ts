





    export class CalendarSummaryItem { 
        public itemId: string;
        public itemStart: Date;
        public itemEnd: Date;
        public addToCancellationList: boolean;
        public patientId: number;
        public patientLastname: string;
        public patientInitial: string;
        public patientFirstName: string;
        public patientSiteId: number;
        public providerId: number;
        public providerFirstName: string;
        public providerLastName: string;
        public providerColorValue: number;
        public statusId: number;
        public statusName: string;
        public statusShow: number;
        public statusIconId: number;
        public siteId: number;
        public siteName: string;
        public siteColorValue: number;
        public resourceId: number;
        public resourceName: string;
        public resourceColorValue: number;
        public appointmentTypeId: number;
        public appointmentTypeName: string;
        public appointmentTypeColorValue: number;
        public appointmentTypeIsSLP: boolean;
        public notes: string;
        public location: string;
        public categoryID: string;
        public categoryName: string;
        public scheduleColorValue: number;
        public dateUpdated: Date;
        public isRecurring: number;
        public recurranceID: number;
        public intervalType: number;
        public intervalSubType: number;
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
        public endAfter: number;
        public startDate: Date;
        public endDate: Date;
        public appointmentTypeWebColor: string;
        public siteWebColor: string;
        public providerWebColor: string;
        public removedInstances: RecurringIntervalRemoved[];
		
    }








