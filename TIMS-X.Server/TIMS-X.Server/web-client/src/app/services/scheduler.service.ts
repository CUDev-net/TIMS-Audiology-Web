import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { EnvironmentUrlService } from '../services/environment-url.service';
import { DateTime } from "luxon";

import { Entities } from '@app/entities/entities';
import AppointmentItemSummary = Entities.AppointmentItemSummary;
import ScheduleItemSummary = Entities.ScheduleItemSummary;
import ScheduleRecurringItemSummary = Entities.ScheduleRecurringItemSummary;

@Injectable({
    providedIn: 'root'
})
export class SchedulerService {
    constructor(private http: HttpClient, private envUrl: EnvironmentUrlService) { }

    public getAppointmentItems = (startdate: DateTime, enddate: DateTime) => {
        let sdate = startdate;
        let edate = enddate;
        let route = `/web/scheduler/get-appointment-items?startdate=${sdate}&enddate=${edate}`;
        return this.http.get<AppointmentItemSummary[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public getScheduleItems = (startdate: DateTime, enddate: DateTime) => {
        let sdate = startdate;
        let edate = enddate;
        let route = `/web/scheduler/get-schedule-items?startdate=${sdate}&enddate=${edate}`;
        return this.http.get<ScheduleItemSummary[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public getRecurringScheduleItems = (startdate: DateTime, enddate: DateTime) => {
        let sdate = startdate;
        let edate = enddate;
        let route = `/web/scheduler/get-recurring-schedule-items?startdate=${sdate}&enddate=${edate}`;
        return this.http.get<ScheduleRecurringItemSummary[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public getRecurringScheduleItemsForDay = (startdate: DateTime) => {
        let sdate = startdate;
        let route = `/web/scheduler/get-recurring-schedule-items-for-day?startdate=${sdate}`;
        return this.http.get<ScheduleRecurringItemSummary[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    private createCompleteRoute = (route: string, envAddress: string) => {
        return `${envAddress}${route}`;
    }

    public getPatientAppointments = () => {
        let route = `/web/scheduler/get-patient-appointments`;
        return this.http.get<Entities.PatientAppointmentItemDto[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public getPatientAppointmentCandidates = (firstname: string, lastname: string, dateOfBirth, email: string, phone: string) => {
        let route = `/web/scheduler/get-patient-appointment-candidates?firstname=${firstname}&lastname=${lastname}&dateOfBirth=${dateOfBirth}&email=${email}&phone=${phone}`;
        return this.http.get<Entities.PatientCandidateDto[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public getPatientAppointmentSearch = (name: string) => {
        let route = `/web/scheduler/get-patient-appointment-search?name=${name}`;
        return this.http.get<Entities.PatientCandidateDto[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public linkPatientAppointment(link: Entities.PatientLinkDto) {
        let route = `/web/scheduler/link-patient-appointment`;
        return this.http.post<Entities.Appointment>(this.createCompleteRoute(route, this.envUrl.urlAddress), link);
    }
}