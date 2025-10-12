



    export class ProviderBlockSchedule { 
        public providerId: number;
        public scheduleBlockId: number;
        public scheduleTimeSlotId: number;
        public createdDate: Date;
        public rowVersion: number[];
        public scheduleBlock: ScheduleBlock;
        public scheduleTimeSlot: ScheduleTimeSlot;
		
        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;
        
    }










