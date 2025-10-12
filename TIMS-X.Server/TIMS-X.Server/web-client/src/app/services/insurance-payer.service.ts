import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { EnvironmentUrlService } from '../services/environment-url.service';

import { Entities } from '@app/entities/entities';

@Injectable({
    providedIn: 'root'
})
export class InsurancePayerService {

    constructor(private http: HttpClient, private envUrl: EnvironmentUrlService) { }

    public getPayers() {
        let route = `/web/insurancepayer/get-all`
        return this.http.get<Entities.InsurancePayer[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public getPayer(id) {
        let route = `/web/insurancepayer/get?id=${id}`
        return this.http.get<Entities.InsurancePayer>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    private createCompleteRoute = (route: string, envAddress: string) => {
        return `${envAddress}${route}`;
    }
}