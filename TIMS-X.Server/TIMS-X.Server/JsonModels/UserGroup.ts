



    export class UserGroup { 
        public name: string;
        public inactive: boolean;
        public protected: boolean;
        public description: string;
        public settings: UserGroupAppSetting[];
        public updatedUserId: number;
        public updatedDate: Date;
        public userReferences: UserGroupReference[];
		
        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;
        
    }










