import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { EnvironmentUrlService } from '../services/environment-url.service';

import { Entities } from '@app/entities/entities';
import ProviderSummary = Entities.ProviderSummary;

@Injectable({
    providedIn: 'root'
})
export class ProviderService {

    constructor(private http: HttpClient, private envUrl: EnvironmentUrlService) { }

    public getSummaries() {
        let route = `/web/provider/get-summaries`
        return this.http.get<ProviderSummary[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public getSummary(id) {
        let route = `/web/provider/get-summary?id=${id}`
        return this.http.get<ProviderSummary>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public getWithHours() {
        let route = `/web/provider/get-with-hours`
        return this.http.get<ProviderSummary[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    private createCompleteRoute = (route: string, envAddress: string) => {
        return `${envAddress}${route}`;
    }
}