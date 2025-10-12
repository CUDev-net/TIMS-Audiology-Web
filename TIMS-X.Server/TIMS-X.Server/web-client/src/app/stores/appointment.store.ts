import { Injectable } from '@angular/core';
import { makeObservable, observable } from 'mobx';
import * as _ from 'underscore';
import { DateTime } from "luxon";

import { AppointmentService } from '@app/services/appointment.service';
import { BaseStore } from './base.store';
import { PatientStore } from './patient.store';
import { SchedulerStore } from './scheduler.store';

import { Entities } from '@app/entities/entities';
import Appointment = Entities.Appointment;
import AppointmentDto = Entities.AppointmentDto;
import ValidationResult = Entities.ValidationResult;
import AppointmentItemSummary = Entities.AppointmentItemSummary;
import ScheduleOpeningsSearchModel = Entities.ScheduleOpeningsSearchModel;
import ScheduleOpeningModel = Entities.ScheduleOpeningModel;
import MonthData = Entities.MonthData;

@Injectable()
export class AppointmentStore extends BaseStore {
    public selected_appointment: Appointment = null;
    public new_appointment_summary: AppointmentItemSummary = null;
    // Used for display purposes
    public openingsStartDate: Date;
    public openingsEndDate: Date;
    public openingsAppointmentTypeId: number;
    public openingsDuration: number

    @observable public validation_results: ValidationResult[] = null;
    @observable public delete_validation_results: ValidationResult[] = null;
    @observable public scheduleOpenings: ScheduleOpeningModel[] = null;
    @observable public months: MonthData[] = null;
    @observable public isSearchForOpenings: boolean = false;

    constructor(private appointmentService: AppointmentService,
        private schedulerStore: SchedulerStore,
        private patientStore: PatientStore) {
        super();
        makeObservable(this);
    }

    public getById(id: number) {
        this.selected_appointment = null;
        this.inprogress = true;
        this.appointmentService.getById(id).subscribe((p) => {
            this.selected_appointment = p;
            this.inprogress = false;
        });
    }

    public validate(appointment: Appointment) {
        this.validation_results = null;
        this.inprogress = true;
        this.appointmentService.validate(appointment).subscribe((p: ValidationResult[]) => {
            this.validation_results = p;
            this.inprogress = false;
        });
    }

    public validateDelete(id: number) {
        this.delete_validation_results = null;
        this.inprogress = true;
        this.appointmentService.validateDelete(id).subscribe((p: ValidationResult[]) => {
            this.delete_validation_results = p;
            this.inprogress = false;
        });
    }

    public createAppointmentSummary(appointmentDto: AppointmentDto) {
        let appointmentSummary = new AppointmentItemSummary();
        appointmentSummary.patient_name = appointmentDto.patientName;
        appointmentSummary.id = 'A-' + appointmentDto.appointment.id;
        appointmentSummary.start_date = appointmentDto.appointment.startsAt;
        appointmentSummary.end_date = appointmentDto.appointment.endsAt;
        appointmentSummary.provider_web_color = appointmentDto.provider_Color;
        appointmentSummary.provider_name = appointmentDto.providerName;
        appointmentSummary.provider_id = appointmentDto.appointment.providerId;
        appointmentSummary.site_web_color = appointmentDto.site_Color;
        appointmentSummary.site_name = appointmentDto.siteName;
        appointmentSummary.site_id = appointmentDto.appointment.siteId;
        appointmentSummary.notes = appointmentDto.appointment.notes;
        appointmentSummary.appointment_type_id = appointmentDto.appointment.appointmentTypeId;
        appointmentSummary.appointment_type_name = appointmentDto.typeName;
        appointmentSummary.appointment_type_web_color = appointmentDto.appointment_Type_Color;
        this.schedulerStore.all_appointment_items.push(appointmentSummary);
        return appointmentSummary;
    }

    public updateAppointmentSummary(appointmentSummary: AppointmentItemSummary, appointmentDto: AppointmentDto) {
        appointmentSummary.provider_web_color = appointmentDto.provider_Color;
        appointmentSummary.site_web_color = appointmentDto.site_Color;
        appointmentSummary.start_date = new Date(appointmentDto.appointment.startsAt);
        appointmentSummary.end_date = new Date(appointmentDto.appointment.endsAt);
        appointmentSummary.notes = appointmentDto.appointment.notes;
        appointmentSummary.provider_id = appointmentDto.appointment.providerId;
        appointmentSummary.provider_name = appointmentDto.providerName;
        appointmentSummary.site_id = appointmentDto.appointment.siteId;
        appointmentSummary.site_name = appointmentDto.siteName;
        appointmentSummary.appointment_type_id = appointmentDto.appointment.appointmentTypeId;
        appointmentSummary.appointment_type_name = appointmentDto.typeName;
        appointmentSummary.appointment_type_web_color = appointmentDto.appointment_Type_Color;
    }

    public create(appointment: Appointment) {
        this.selected_appointment = null;
        this.inprogress = true;
        this.appointmentService.create(appointment).subscribe((newData: AppointmentDto[]) => {
            _.each(newData, (p) => {
                this.patientStore.getSummary(p.appointment.patientId);
                let a = this.createAppointmentSummary(p);
            });
            //this.new_appointment_summary = a;
            this.inprogress = false;
        });
    }

    public updatePatientName(patientName: string, appointmentId: number) {
        let a_id = 'A-' + appointmentId;
        let a = _.find(this.schedulerStore.all_appointment_items, (x) => { return x.id == a_id; });
        if (a) {
            if (patientName)
                a.patient_name = patientName;
        }
    }

    public update(appointment: Appointment) {
        this.inprogress = true;
        this.appointmentService.update(appointment).subscribe((p: AppointmentDto) => {
            this.selected_appointment = p.appointment;
            let a_id = 'A-' + appointment.id;
            let a = _.find(this.schedulerStore.all_appointment_items, (x) => { return x.id == a_id; });
            if (a) {
                this.updateAppointmentSummary(a, p);
            }
            this.inprogress = false;
        });
    }

    public endRecurringSeries(id: number) {
        this.inprogress = true;
        this.appointmentService.endRecurringSeries(id).subscribe((isDeleted) => {
            // Let signalr handle the removal of the series
            this.inprogress = false;
        });
    }

    public delete(id: number) {
        this.inprogress = true;
        this.appointmentService.delete(id).subscribe((isDeleted) => {
            this.inprogress = false;
        });
    }

    public searchForOpenings(scheduleOpeningsSearchModel: ScheduleOpeningsSearchModel) {
        this.inprogress = true;
        this.appointmentService.searchForOpenings(scheduleOpeningsSearchModel).subscribe((p: ScheduleOpeningModel[]) => {
            this.months = [];
            let monthData: MonthData = null;
            let currentMonth = -1;
            _.each(p, (opening) => {
                let startsAt = DateTime.fromISO(opening.startsAt);
                opening.dayOfMonth = startsAt.day;
                opening.dayOfWeek = startsAt.weekdayLong;
                opening.time = startsAt.toFormat('h:mm a') + ' - ' + DateTime.fromISO(opening.endsAt).toFormat('h:mm a');
                let openingMonth = startsAt.month;
                if (currentMonth != openingMonth) {
                    currentMonth = openingMonth;
                    monthData = new MonthData();
                    monthData.label = DateTime.fromISO(opening.startsAt).toFormat('MMMM, yyyy');;
                    monthData.openings = [opening];
                    this.months.push(monthData);
                }
                else {
                    monthData.openings.push(opening);
                }
            });
            this.scheduleOpenings = p;
            this.isSearchForOpenings = true;
            this.inprogress = false;
        });
    }
}