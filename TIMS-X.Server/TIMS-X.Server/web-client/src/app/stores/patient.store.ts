import { Injectable } from '@angular/core';
import { makeObservable, observable } from 'mobx';
import * as _ from 'underscore';

import { BaseStore } from './base.store';
import { PatientService } from '../services/patient.service';
import { UserStore } from './user.store';
import { PatientInsuranceStore } from './patient-insurance.store';
import { ApptAuthorizationStore } from './appt-authorization.store';

import { Entities } from '@app/entities/entities';
import Patient = Entities.Patient;
import PatientSummary = Entities.PatientSummary;
import PatientSearchCriteria = Entities.PatientSearchCriteriaDto;

@Injectable()
export class PatientStore extends BaseStore {
    @observable public selected_patient_summary: PatientSummary = null;
    @observable public census_submitted: boolean = false;
    @observable public censusPatients: PatientSummary[] = null;
    @observable public selected_patient: Patient = null;
    @observable public appointment_patient: PatientSummary = null;

    public new_appointment_patientId: number;

    constructor(private patientService: PatientService, private userStore: UserStore,
        private patientInsuranceStore: PatientInsuranceStore,
        private apptAuthorizationStore: ApptAuthorizationStore) {
        super();
        makeObservable(this);
    }

    public getAppointmentPatient(id: number) {
        this.appointment_patient = null;
        this.inprogress = true;
        this.patientService.getPatientSummary(id).subscribe((p) => {
            this.appointment_patient = p;
            this.inprogress = false;
        });
    }
    public getSummary(id: number) {
        this.inprogress = true;
        this.census_submitted = false;
        this.patientService.getPatientSummary(id).subscribe((p: PatientSummary) => {
            this.selected_patient_summary = p;
            this.userStore.getUserPatientList();
            this.inprogress = false;
        });
    }

    public searchPatients(criteria: PatientSearchCriteria) {
        this.inprogress = true;

        this.patientService.searchPatients(criteria).subscribe((p: PatientSummary[]) => {
            this.censusPatients = p;
            this.inprogress = false;
        });
    }

    public getById(id: number) {
        this.inprogress = true;
        this.census_submitted = false;
        this.patientService.getById(id).subscribe((p) => {
            this.userStore.getUserPatientList();
            this.selected_patient = p;
            this.inprogress = false;
        });
    }
    public create(patient: Patient, insurances: Entities.PatientInsurance[], authorizations: Entities.ApptAuthorization[]) {
        this.inprogress = true;
        this.selected_patient = null;
        this.patientService.create(patient).subscribe((p: Patient) => {
            this.userStore.getUserPatientList();
            this.selected_patient = p;
            if (insurances && insurances.length > 0) {
                _.each(insurances, (x: Entities.PatientInsurance) => {
                    x.patientId = p.id;
                    this.patientInsuranceStore.create(x);
                });
            }
            if (authorizations && authorizations.length > 0) {
                _.each(authorizations, (x: Entities.ApptAuthorization) => {
                    x.patientId = p.id;
                    this.apptAuthorizationStore.create(x);
                });
            }
            this.inprogress = false;
        });
    }

    public update(patient: Patient, insurances: Entities.PatientInsurance[], authorizations: Entities.ApptAuthorization[]) {
        this.inprogress = true;
        this.patientService.update(patient).subscribe((p: Patient) => {
            if (insurances && insurances.length > 0) {
                _.each(insurances, (x: Entities.PatientInsurance) => {
                    if (!x.id || x.id == 0) {
                        x.patientId = p.id;
                        this.patientInsuranceStore.create(x);
                    }
                    else {
                        this.patientInsuranceStore.update(x);
                    }
                });
            }
            if (authorizations && authorizations.length > 0) {
                _.each(authorizations, (x: Entities.ApptAuthorization) => {
                    if (x.isDeleted) {
                        if (x.id > 0)
                            this.apptAuthorizationStore.delete(x.id);
                    }
                    else if (x.id <= 0) {
                        let a = this.apptAuthorizationStore.create(x);
                    }
                    else {
                        let a = this.apptAuthorizationStore.update(x);
                    }
                });
            }
            this.inprogress = false;
            this.userStore.getUserPatientList();
            // Update summary
            this.getSummary(p.id);
        });
    }

}
