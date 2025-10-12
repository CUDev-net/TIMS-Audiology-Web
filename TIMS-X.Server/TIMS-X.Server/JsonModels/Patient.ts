


    export class Patient { 
        public guid: string;
        public inactive: boolean;
        public salutationId: number | null;
        public providerId: number | null;
        public primaryCareId: number | null;
        public referringPhysicianId: number | null;
        public marketingId: number | null;
        public siteId: number | null;
        public maritalStatusId: string;
        public emplStatusId: string;
        public patientStatusId: number | null;
        public patientTypeId: number | null;
        public otStatusId: number;
        public otStatusDescriptionId: number;
        public birthDate: Date | null;
        public deathDate: Date | null;
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
        public customDate1: Date | null;
        public customDate2: Date | null;
        public ssn: string;
        public insuredInsurancePayerId: number | null;
        public updatedSiteId: number | null;
        public race: RaceEnum;
        public ethnicity: EthnicityEnum;
        public language: LanguageEnum;
        public legalRep: boolean;
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
        public releaseSignatureDate: Date | null;
        public assignBenefits: boolean;
        public assignBenefitsDate: Date | null;
        public responsibleParty: string;
        public hasIntakeData: boolean;
        public lastFirstMiddle: string;
        public firstLast: string;
        public sexFull: string;
        public salutation: Salutation;
        public provider: Provider;
        public primaryCarePhysician: MarketingReference;
        public referringPhysician: MarketingReference;
        public marketing: MarketingReference;
        public updatedByUser: User;
        public createdUserId: number | null;
        public createdDate: Date;
        public privacyDate: Date | null;
        public updatedUserId: number | null;
        public updatedDate: Date;
        public useSecondaryAddress: boolean;
        public secondaryAddress1: string;
        public secondaryAddress2: string;
        public secondaryCity: string;
        public secondaryState: string;
        public secondaryZipCode: string;
        public qbid: string;
        public releaseInformation: boolean;
        public releaseInformationDate: Date | null;
        public consentDate: Date | null;
        public marketingAuthorization: boolean;
        public marketingAuthorizationDate: Date | null;
        public authorizedParties: string;
        public studentStatusId: string;
        public restrictions: PatientRestriction[];
        public medicalConditions: PreviousHistory[];
        public patientTypeIds: number[];
        public restrictionIds: number[];
        public authorizationIds: number[];
        public patientTypeReferences: PatientTypeReference[];
        public authorizationReferences: AuthorizationReference[];
		
        public hasBeenAudited: boolean;
        
        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;
    }











