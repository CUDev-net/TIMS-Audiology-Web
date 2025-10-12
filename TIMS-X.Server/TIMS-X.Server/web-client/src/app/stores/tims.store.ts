import { Injectable } from '@angular/core';

import { UserStore } from './user.store';
import { PatientStore } from './patient.store';
import { PracticeStore } from './practice.store';
import { ProviderStore } from './provider.store';
import { ResourceStore } from './resource.store';
import { SiteStore } from './site.store';
import { SchedulerStore } from './scheduler.store';
import { LookupStore } from './lookup.store';
import { AppointmentStore } from './appointment.store';
import { ScheduleStore } from './schedule.store';
import { PatientScheduleStore } from './patient-schedule.store';
import { InsurancePayerStore } from './insurance-payer.store';
import { PatientInsuranceStore } from './patient-insurance.store';
import { ApptAuthorizationStore } from './appt-authorization.store';

@Injectable()
export class TimsStore {
    constructor(public userstore: UserStore,
        public patientStore: PatientStore,
        public practiceStore: PracticeStore,
        public providerStore: ProviderStore,
        public schedulerStore: SchedulerStore,
        public resourceStore: ResourceStore,
        public siteStore: SiteStore,
        public lookupStore: LookupStore,
        public scheduleStore: ScheduleStore,
        public appointmentStore: AppointmentStore,
        public patientScheduleStore: PatientScheduleStore,
        public insurancePayerStore: InsurancePayerStore,
        public patientInsuranceStore: PatientInsuranceStore,
        public apptAuthorizationStore: ApptAuthorizationStore) {
    }

    initializePatientScheduleStores() {
        this.patientScheduleStore.getSites();
        this.patientScheduleStore.getProviders();
    }

    initializeWebScehdulerStores() {
        this.userstore.getCurrentUser();
        this.practiceStore.get();
        this.practiceStore.getBusinessRules();
        this.providerStore.getSummaries();
        this.siteStore.getSummaries();
        this.resourceStore.getSummaries();
        this.lookupStore.getAll();
        this.insurancePayerStore.getPayers();
    }

    inProgress() {
        return this.lookupStore.inprogress ||
            this.userstore.inprogress ||
            this.patientStore.inprogress ||
            this.practiceStore.inprogress ||
            this.providerStore.inprogress ||
            this.resourceStore.inprogress ||
            this.schedulerStore.inprogress ||
            this.siteStore.inprogress ||
            this.scheduleStore.inprogress ||
            this.appointmentStore.inprogress ||
            this.insurancePayerStore.inprogress ||
            this.patientInsuranceStore.inprogress ||
            this.apptAuthorizationStore.inprogress;
    }
}