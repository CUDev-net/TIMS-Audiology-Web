import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { EnvironmentUrlService } from '../services/environment-url.service';

import { Entities } from '@app/entities/entities';
import PatientSummary = Entities.PatientSummary;
import PatientSearchCriteria = Entities.PatientSearchCriteriaDto;
import Patient = Entities.Patient;


@Injectable({
    providedIn: 'root'
})
export class PatientService {
    constructor(private http: HttpClient, private envUrl: EnvironmentUrlService) { }

    public getPatientSummary = (id: number) => {
        let route = `/web/patient/summary?id=${id}`;
        return this.http.get<PatientSummary>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public searchPatients = (criteria: PatientSearchCriteria) => {
        let route = `/web/patient/search`;
        return this.http.post<PatientSummary[]>(this.createCompleteRoute(route, this.envUrl.urlAddress), criteria);
    }

    private createCompleteRoute = (route: string, envAddress: string) => {
        return `${envAddress}${route}`;
    }

    public getById = (id: number) => {
        let route = `/web/patient/get?id=${id}`
        return this.http.get<Patient>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public create(patient: Patient) {
        patient.id = 0;
        let route = `/web/patient`;
        return this.http.post<Patient>(this.createCompleteRoute(route, this.envUrl.urlAddress), patient);
    }

    public update(patient: Patient) {
        let route = `/web/patient/${patient.id}`;
        return this.http.put<Patient>(this.createCompleteRoute(route, this.envUrl.urlAddress), patient);
    }
}