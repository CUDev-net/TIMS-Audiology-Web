



    export class PosDocument { 
        public patientId: number;
        public documentDate: Date;
        public taxGroupId: number;
        public documentType: PosDocumentType;
        public siteId: number;
        public providerId: number;
        public customerMessageId: number;
        public void: boolean;
        public final: boolean;
        public billTo: string;
        public paymentAmount: number;
        public paymentMethodId: number;
        public memo: string;
        public qbUpdateDate: Date;
        public qbInvoice: string;
        public qbTransactionId: string;
        public paymentReference: string;
        public isCopay: boolean;
        public copayApplied: boolean;
        public copayAmount: number;
        public appointmentId: number;
        public notes: string;
        public poNumber: string;
        public applyToId: number;
        public marketingId: number;
        public paymentType: number;
        public billToAddr1: string;
        public billToAddr2: string;
        public billToAddr3: string;
        public billToAddr4: string;
        public billToCity: string;
        public billToState: string;
        public billToPostalCode: string;
        public billToCountry: string;
        public rowVersion: number[];
        public insurancePosItem: number;
        public insuranceSelected: boolean;
        public isTaxIncluded: boolean;
        public pdfClaimData: string;
        public formClaimNumber: string;
        public createdByUserId: number;
        public dateCreated: Date;
        public updatedBySiteId: number;
        public modifier1: number;
        public modifier2: number;
        public modifier3: number;
        public modifier4: number;
        public diagnosis1: number;
        public diagnosis2: number;
        public diagnosis3: number;
        public diagnosis4: number;
        public posDepositId: number;
        public arUpdateDate: Date;
        public arVoid: boolean;
        public updatedUserId: number;
        public updatedDate: Date;
        public posLines: PosLineItem[];
		
        public pendingDelete: boolean;
        public id: number;
        public hasStateBeenSet: boolean;
        
    }










