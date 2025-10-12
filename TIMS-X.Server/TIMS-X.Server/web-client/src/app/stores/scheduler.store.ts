import { Injectable } from '@angular/core';
import { makeObservable, observable } from 'mobx';
import * as _ from 'underscore';

import { SchedulerService } from '../services/scheduler.service';
import { BaseStore } from './base.store';

import { Entities } from '@app/entities/entities';
import AppointmentItemSummary = Entities.AppointmentItemSummary;
import ScheduleItemSummary = Entities.ScheduleItemSummary;
import ScheduleRecurringItemSummary = Entities.ScheduleRecurringItemSummary;

@Injectable()
export class SchedulerStore extends BaseStore {
    public all_appointment_items: AppointmentItemSummary[] = [];
    public all_schedule_items: ScheduleItemSummary[] = [];
    public all_recurring_schedule_items: ScheduleRecurringItemSummary[] = [];
    @observable patient_appointments: Entities.PatientAppointmentItemDto[] = [];
    public patient_appointment_candidates: Entities.PatientCandidateDto[] = [];
    public updated_appointment_link: Entities.Appointment = null;
    @observable public is_fetching_appointments: boolean = false;
    @observable public is_fetching_schedules: boolean = false;
    @observable public is_fetching_recurring_schedules: boolean = false;
    @observable public is_fetching_patient_appointments: boolean = false;

    constructor(private schedulerService: SchedulerService) {
        super();
        makeObservable(this);
    }

    getScheduleItems = (startdate: Date, enddate: Date) => {
        this.is_fetching_schedules = true;
        this.schedulerService.getScheduleItems(startdate, enddate).subscribe((p) => {
            this.all_schedule_items = p;
            this.is_fetching_schedules = false;
        });
    }

    getRecurrringScheduleItems = (startdate: Date, enddate: Date) => {
        this.is_fetching_recurring_schedules = true;
        this.schedulerService.getRecurringScheduleItems(startdate, enddate).subscribe((p) => {
            this.all_recurring_schedule_items = p;
            this.is_fetching_recurring_schedules = false;
        });
    }

    getRecurrringScheduleItemsForDay = (startdate: Date) => {
        this.is_fetching_recurring_schedules = true;
        this.schedulerService.getRecurringScheduleItemsForDay(startdate).subscribe((p) => {
            this.all_recurring_schedule_items = p;
            this.is_fetching_recurring_schedules = false;
        });
    }

    getAppointmentItems = (startdate: Date, enddate: Date) => {
        this.is_fetching_appointments = true;
        this.schedulerService.getAppointmentItems(startdate, enddate).subscribe((p) => {
            this.all_appointment_items = p;
            this.is_fetching_appointments = false;
        });
    }

    getPatientAppointments = () => {
        this.is_fetching_patient_appointments = true;
        this.schedulerService.getPatientAppointments().subscribe((p) => {
            this.patient_appointments = p;
            this.is_fetching_patient_appointments = false;
        });
    }

    getPatientAppointmentCandidates = (firstname: string, lastname: string, dateOfBirth, email: string, phone: string) => {
        this.patient_appointment_candidates = null;
        this.schedulerService.getPatientAppointmentCandidates(firstname, lastname, dateOfBirth, email, phone).subscribe((p) => {
            this.patient_appointment_candidates = p;
        });
    }

    getPatientAppointmentSearch = (name: string) => {
        this.patient_appointment_candidates = null;
        this.schedulerService.getPatientAppointmentSearch(name).subscribe((p) => {
            this.patient_appointment_candidates = p;
        });
    }

    linkPatientAppointment(link: Entities.PatientLinkDto) {
        this.inprogress = true;
        this.updated_appointment_link = null
        this.schedulerService.linkPatientAppointment(link).subscribe((p) => {
            this.updated_appointment_link = p;
            this.inprogress = false;
        });
    }
}