import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DateTime } from "luxon";

import { EnvironmentUrlService } from '../services/environment-url.service';

import { Entities } from '@app/entities/entities';


@Injectable({
    providedIn: 'root'
})
export class PatientScheduleService {
    constructor(private http: HttpClient, private envUrl: EnvironmentUrlService) { }

    public getSites = (officeCode: string) => {
        let route = `/web/patientscheduling/get-sites?officeCode=${officeCode}`;
        return this.http.get<Entities.Site[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public getProviders = (officeCode: string) => {
        let route = `/web/patientscheduling/get-providers?officeCode=${officeCode}`;
        return this.http.get<Entities.ProviderSummary[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public getTimeSlots = (officeCode: string, providerId: number, siteId: number, date: DateTime) => {
        let d = date;
        let route = `/web/patientscheduling/get-time-slots?officeCode=${officeCode}&providerId=${providerId}&siteId=${siteId}&date=${d}`;
        return this.http.get<DateTime[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public getPractice = (officeCode: string) => {
        let route = `/web/patientscheduling/get-practice?officeCode=${officeCode}`;
        return this.http.get<Entities.PracticeDto>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public create(officeCode: string, appointment: Entities.PatientScheduledAppointmentDto) {
        let route = `/web/patientscheduling?officeCode=${officeCode}`;
        return this.http.post<Entities.CreatedPatientAppointmentDto>(this.createCompleteRoute(route, this.envUrl.urlAddress), appointment);
    }

    private createCompleteRoute = (route: string, envAddress: string) => {
        return `${envAddress}${route}`;
    }

}
