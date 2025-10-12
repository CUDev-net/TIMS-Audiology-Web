import { Injectable } from '@angular/core';
import { makeObservable, observable } from 'mobx';
import * as _ from 'underscore';

import { PatientInsuranceService } from '../services/patient-insurance.service';
import { BaseStore } from './base.store';

import { Entities } from '@app/entities/entities';
import PatientInsurance = Entities.PatientInsurance;

@Injectable()
export class PatientInsuranceStore extends BaseStore {
    @observable public patientinsurance_list: Map<number, PatientInsurance> = null;
    public patientinsurnace: PatientInsurance = null;

    constructor(private patientInsuranceService: PatientInsuranceService) {
        super();
        makeObservable(this);
    }

    public getForPatient(patientId: number) {
        this.patientinsurance_list = null;
        this.inprogress = true;
        this.patientInsuranceService.getForPatient(patientId).subscribe((p) => {
            let list = new Map<number, PatientInsurance>();
            _.each(p, ((i: PatientInsurance) => {
                list.set(i.payerLevel, i);
            }));
            this.patientinsurance_list = list;
            this.inprogress = false;
        });
    }

    public create(patientInsurance: PatientInsurance) {
        this.inprogress = true;
        this.patientInsuranceService.create(patientInsurance).subscribe((newData: PatientInsurance) => {
            this.inprogress = false;
        });
    }

    public update(patientInsurance: PatientInsurance) {
        this.inprogress = true;
        this.patientInsuranceService.update(patientInsurance).subscribe((newData: PatientInsurance) => {
            this.inprogress = false;
        });
    }
}