import { Injectable } from '@angular/core';
import { makeObservable, observable } from 'mobx';
import * as _ from 'underscore';

import { ApptAuthorizationService } from '../services/appt-authorization.service';
import { BaseStore } from './base.store';

import { Entities } from '@app/entities/entities';
import ApptAuthorization = Entities.ApptAuthorization;

@Injectable()
export class ApptAuthorizationStore extends BaseStore {
    @observable public apptAuthorization_list: ApptAuthorization[] = null;
    @observable public apptAuthorization: ApptAuthorization = null;

    constructor(private apptAuthorizationService: ApptAuthorizationService) {
        super();
        makeObservable(this);
    }

    public getForPatient(patientId: number, includeInactive: boolean, auhorizationId: number) {
        this.apptAuthorization_list = null;
        this.inprogress = true;
        this.apptAuthorizationService.getForPatient(patientId, includeInactive, auhorizationId).subscribe((p) => {
            this.apptAuthorization_list = p;
            this.inprogress = false;
        });
    }

    public create(apptAuthorization: ApptAuthorization) {
        this.inprogress = true;
        this.apptAuthorizationService.create(apptAuthorization).subscribe((newData: ApptAuthorization) => {
            this.inprogress = false;
        });
    }

    public update(apptAuthorization: ApptAuthorization) {
        this.inprogress = true;
        this.apptAuthorizationService.update(apptAuthorization).subscribe((newData: ApptAuthorization) => {
            this.inprogress = false;
        });
    }

    public delete(id: number) {
        this.inprogress = true;
        this.apptAuthorizationService.delete(id).subscribe((isDeleted) => {
            this.inprogress = false;
        });
    }
}