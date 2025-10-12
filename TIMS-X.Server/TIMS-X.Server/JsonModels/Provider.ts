



    export class Provider { 
        public inactive: boolean;
        public lastName: string;
        public useForPatientScheduling: boolean;
        public signatureOnFile: boolean;
        public usePracticeIds: boolean;
        public firstName: string;
        public notBillable: boolean;
        public billToId: number;
        public initial: string;
        public degree: string;
        public signatureDate: Date | null;
        public upin: string;
        public taxId: string;
        public taxIdType: string;
        public groupNum: string;
        public deleted: boolean;
        public npi: string;
        public color: number | null;
        public secondaryIdNum: string;
        public secondaryIdQualifier: string;
        public specialtyCode: string;
        public specialtyLicNum: string;
        public medicareNum: string;
        public medicaidNum: string;
        public blueShieldNum: string;
        public stateLicNum: string;
        public anesthesiaLicNum: string;
        public qbid: string;
        public qbModifiedDate: Date | null;
        public qbid2: string;
        public taxonomy: string;
        public displayOrder: number;
        public updatedUserId: number | null;
        public updatedDate: Date;
        public fullName: string;
        public userId: number;
        public user: User;
        public blockSchedules: ProviderBlockSchedule[];
        public hours: HoursOfOperationModel[];
        public siteHours: UserSiteHours[];
        public webColor: string;
		
        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;
        
    }










