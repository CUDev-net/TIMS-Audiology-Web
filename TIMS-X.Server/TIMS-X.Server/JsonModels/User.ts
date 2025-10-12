



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
        public lastPatientList: LastPatientList;
        public isSupport: boolean;
        public siteHours: UserSiteHours[];
        public userReferences: UserGroupReference[];
		
        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;
        
    }










