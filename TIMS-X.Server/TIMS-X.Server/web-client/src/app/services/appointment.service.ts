import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { EnvironmentUrlService } from '../services/environment-url.service';

import { Entities } from '@app/entities/entities';
import Appointment = Entities.Appointment;
import AppointmentDto = Entities.AppointmentDto;
import ValidationResult = Entities.ValidationResult;
import ScheduleOpeningsSearchModel = Entities.ScheduleOpeningsSearchModel;
import ScheduleOpeningModel = Entities.ScheduleOpeningModel;

@Injectable({
    providedIn: 'root'
})
export class AppointmentService {
    constructor(private http: HttpClient, private envUrl: EnvironmentUrlService) { }

    private createCompleteRoute = (route: string, envAddress: string) => {
        return `${envAddress}${route}`;
    }

    public getById = (id: number) => {
        let route = `/web/appointment/get?id=${id}`
        return this.http.get<Appointment>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public create(appointment: Appointment) {
        appointment.id = 0;
        let route = `/web/appointment`;
        return this.http.post<AppointmentDto[]>(this.createCompleteRoute(route, this.envUrl.urlAddress), appointment);
    }

    public update(appointment: Appointment) {
        let route = `/web/appointment/${appointment.id}`;
        return this.http.put<AppointmentDto>(this.createCompleteRoute(route, this.envUrl.urlAddress), appointment);
    }

    public validate(appointment: Appointment) {
        let route = `/web/appointment/validate`;
        return this.http.post<ValidationResult[]>(this.createCompleteRoute(route, this.envUrl.urlAddress), appointment);
    }

    public validateDelete(id: number) {
        let route = `/web/appointment/validate-delete?id=${id}`;
        return this.http.get<ValidationResult[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public endRecurringSeries(id) {
        let route = `/web/appointment/end-series-${id}`;
        return this.http.delete<number[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public delete(id: number) {
        let route = `/web/appointment/${id}`;
        return this.http.delete(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public searchForOpenings(scheduleOpeningsSearchModel: ScheduleOpeningsSearchModel) {
        let route = `/web/appointment/schedule-openings`;
        return this.http.post<ScheduleOpeningModel[]>(this.createCompleteRoute(route, this.envUrl.urlAddress), scheduleOpeningsSearchModel);
    }
}