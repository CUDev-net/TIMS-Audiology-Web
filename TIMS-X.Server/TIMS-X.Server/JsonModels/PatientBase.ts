



    export class PatientBase { 
        public inactive: boolean;
        public salutationId: number;
        public providerId: number;
        public primaryCareId: number;
        public referringPhysicianId: number;
        public marketingId: number;
        public siteId: number;
        public maritalStatusId: string;
        public emplStatusId: string;
        public patientStatusId: number;
        public patientTypeId: number;
        public otStatusId: number;
        public birthDate: Date;
        public deathDate: Date;
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
        public primaryPhone: PrimaryPhoneEnum;
        public homePhone: string;
        public mobilePhone: string;
        public workPhone: string;
        public otherPhone: string;
        public accountNo: string;
        public notes: string;
        public customText1: string;
        public customText2: string;
        public customDate1: Date;
        public customDate2: Date;
        public ssn: string;
        public insuredInsurancePayerId: number;
        public updatedSiteId: number;
        public race: RaceEnum;
        public ethnicity: EthnicityEnum;
        public language: LanguageEnum;
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
        public releaseSignatureDate: Date;
        public assignBenefits: boolean;
        public assignBenefitsDate: Date;
        public responsibleParty: string;
        public hasIntakeData: boolean;
        public lastFirstMiddle: string;
        public sexFull: string;
        public salutation: Salutation;
        public provider: Provider;
        public primaryCarePhysician: MarketingReference;
        public referringPhysician: MarketingReference;
        public updatedByUser: User;
        public createdUserId: number;
        public createdDate: Date;
        public updatedUserId: number;
        public updatedDate: Date;
        public useSecondaryAddress: boolean;
        public secondaryAddress1: string;
        public secondaryAddress2: string;
        public secondaryCity: string;
        public secondaryState: string;
        public secondaryZipCode: string;
        public restrictions: PatientRestriction[];
        public medicalConditions: PreviousHistory[];
        public patientInsurances: PatientInsurance[];
		
        public hasBeenAudited: boolean;
        
        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;
    }





