import { Injectable } from '@angular/core';
import { makeObservable, observable } from 'mobx';
import * as _ from 'underscore';
import { DateTime } from "luxon";

import { PatientScheduleService } from '../services/patient-schedule.service';

import { BaseStore } from './base.store';
import { Entities } from '@app/entities/entities';


@Injectable()
export class PatientScheduleStore extends BaseStore {
    @observable public provider_list: Entities.ProviderSummary[] = [];
    @observable public site_list: Entities.Site[] = [];
    @observable public timeSlots: Entities.TimeSlotDto[] = [];
    @observable public practice: Entities.PracticeDto = null;
    @observable public createdPatientAppointmentDto: Entities.CreatedPatientAppointmentDto;
    @observable public timeslotMessage: string;
    public officeCode: string;
    public createNewPatient: boolean = false;

    constructor(private patientScheduleService: PatientScheduleService) {
        super();
        makeObservable(this);
        this.timeslotMessage = 'choose';
    }

    public getSites() {
        this.site_list = null;
        this.patientScheduleService.getSites(this.officeCode).subscribe((p) => {
            this.site_list = _.sortBy(p, 'name');
        });
    }

    public getProviders() {
        this.site_list = null;
        this.patientScheduleService.getProviders(this.officeCode).subscribe((p) => {
            this.provider_list = _.sortBy(p, 'firstName');
        });
    }

    public getTimeSlots(providerId: number, siteId: number, date: DateTime) {
        this.timeSlots = null;
        this.patientScheduleService.getTimeSlots(this.officeCode, providerId, siteId, date).subscribe((p) => {
            this.timeSlots = p;
            if (p.length == 0) {
                this.timeslotMessage = 'no availability';
            }
            else {
                this.timeslotMessage = 'choose';
            }
        });
    }

    public create(appointment: Entities.PatientScheduledAppointmentDto) {
        this.inprogress = true;
        this.createdPatientAppointmentDto = null;
        this.patientScheduleService.create(this.officeCode, appointment).subscribe((a) => {
            this.createdPatientAppointmentDto = a;
            this.inprogress = false;
        })
    }

    public getPractice() {
        this.patientScheduleService.getPractice(this.officeCode).subscribe((p) => {
            this.practice = p;
        });
    }
}
