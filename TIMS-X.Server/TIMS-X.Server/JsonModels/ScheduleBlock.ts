



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
        public inactive: boolean;
        public providerBlockSchedules: ProviderBlockSchedule[];
		
        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;
        
    }










