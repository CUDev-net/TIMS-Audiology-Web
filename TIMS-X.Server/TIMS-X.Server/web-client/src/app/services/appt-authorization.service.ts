import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { EnvironmentUrlService } from '../services/environment-url.service';

import { Entities } from '@app/entities/entities';
import ApptAuthorization = Entities.ApptAuthorization;

@Injectable({
    providedIn: 'root'
})
export class ApptAuthorizationService {

    constructor(private http: HttpClient, private envUrl: EnvironmentUrlService) { }

    public getForPatient(patientId: number, includeInactive: boolean, authorizationId: number) {
        if (authorizationId == null) authorizationId = 0;
        let route = `/web/apptauthorization/get-for-patient?patientId=${patientId}&includeInactive=${includeInactive}&authorizationId=${authorizationId}`
        return this.http.get<ApptAuthorization[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public create(patientInsurance: ApptAuthorization) {
        patientInsurance.id = 0;
        let route = `/web/apptauthorization`;
        return this.http.post<ApptAuthorization>(this.createCompleteRoute(route, this.envUrl.urlAddress), patientInsurance);
    }

    public update(patientInsurance: ApptAuthorization) {
        let route = `/web/apptauthorization/${patientInsurance.id}`;
        return this.http.put<ApptAuthorization>(this.createCompleteRoute(route, this.envUrl.urlAddress), patientInsurance);
    }

    public delete(id: number) {
        let route = `/web/apptauthorization/${id}`;
        return this.http.delete(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    private createCompleteRoute = (route: string, envAddress: string) => {
        return `${envAddress}${route}`;
    }
}