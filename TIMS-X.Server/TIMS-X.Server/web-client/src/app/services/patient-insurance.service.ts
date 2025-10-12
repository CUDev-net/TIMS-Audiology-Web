import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { EnvironmentUrlService } from '../services/environment-url.service';

import { Entities } from '@app/entities/entities';
import PatientInsurance = Entities.PatientInsurance;

@Injectable({
    providedIn: 'root'
})
export class PatientInsuranceService {

    constructor(private http: HttpClient, private envUrl: EnvironmentUrlService) { }

    public getForPatient(patientId: number) {
        let route = `/web/patientinsurance/get-for-patient?patientId=${patientId}`
        return this.http.get<PatientInsurance[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public create(patientInsurance: PatientInsurance) {
        patientInsurance.id = 0;
        let route = `/web/patientinsurance`;
        return this.http.post<PatientInsurance>(this.createCompleteRoute(route, this.envUrl.urlAddress), patientInsurance);
    }

    public update(patientInsurance: PatientInsurance) {
        let route = `/web/patientinsurance/${patientInsurance.id}`;
        return this.http.put<PatientInsurance>(this.createCompleteRoute(route, this.envUrl.urlAddress), patientInsurance);
    }

    public delete(id: number) {
        let route = `/web/patientinsurance/${id}`;
        return this.http.delete(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    private createCompleteRoute = (route: string, envAddress: string) => {
        return `${envAddress}${route}`;
    }
}