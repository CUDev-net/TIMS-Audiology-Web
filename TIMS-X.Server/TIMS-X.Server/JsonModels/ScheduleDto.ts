




    export class ScheduleDto { 
        public provider_Color: string;
        public providerName: string;
        public schedule: Schedule;
        public recurringItemSummary: ScheduleRecurringItemSummary;
        public site_Color: string;
        public siteName: string;
        public providers: ScheduleProviderDto[];
		
    }
    export class ScheduleProviderDto { 
        public id: number;
        public color: string;
        public name: string;
		
    }









