



    export class History { 
        public updatedSiteId: number;
        public patientId: number;
        public historyDate: Date;
        public historyType: HistoryType;
        public patient: Patient;
        public provider: Provider;
        public site: Site;
        public appointment: Appointment;
        public marketingReference: MarketingReference;
        public syncSiteId: number;
        public providerId: number;
        public referralSourceId: number;
        public severityLeft: number;
        public severityRight: number;
        public severityBilateral: number;
        public typeofLossLeft: number;
        public typeofLossRight: number;
        public typeofLossBilateral: number;
        public results1Right: number;
        public results1Left: number;
        public results2Right: number;
        public results2Left: number;
        public results3Right: number;
        public results3Left: number;
        public results4Right: number;
        public results4Left: number;
        public results5Right: number;
        public results5Left: number;
        public results6Right: number;
        public results6Left: number;
        public diagnosis: string;
        public recommendation: string;
        public notes: string;
        public historyTypeId: number;
        public appointmentId: number;
        public availableDate: Date | null;
        public rowVersion: number[];
        public historyGuid: string;
        public actionId: number;
        public lockDate: Date | null;
        public lockedByUser: number | null;
        public parentId: number | null;
        public patientInteractionId: number | null;
        public exportDate: Date | null;
        public officeNotes: string;
        public customText1: string;
        public configurationRight: number;
        public configurationLeft: number;
        public slpFluencyVoiceNotes: string;
        public slpFluency2: number;
        public slpVoice2: number;
        public slpPragmatics: number;
        public slpVoice: number;
        public slpFluency: number;
        public slpArticulation: number;
        public slpExpressiveLanguage: number;
        public slpReceptiveLanguage: number;
        public slpAttendingSkills: number;
        public slpResponseRate: number;
        public slpLevelOfActivity: number;
        public slpGoals: string;
        public slpCooperation: number;
        public slpSocialInteractions: number;
        public slpCommunicativeIntent: number;
        public slpAwarenessOfOthers: number;
        public slpReliabilityOfScores: number;
        public slpEnvironmentalAwareness: number;
        public slpPrognosis: number;
        public slpProgressNotes: string;
        public slpDiagnosis: string;
        public slpRecommendationNotes: string;
        public slpGoalsStatus: number;
        public createdDate: Date;
        public updatedUserId: number | null;
        public updatedDate: Date;
		
        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;
        
    }










