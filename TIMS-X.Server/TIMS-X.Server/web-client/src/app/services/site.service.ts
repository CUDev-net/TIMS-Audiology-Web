import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { EnvironmentUrlService } from '../services/environment-url.service';

import { Entities } from '@app/entities/entities';
import Site = Entities.Site;

@Injectable({
    providedIn: 'root'
})
export class SiteService {

    constructor(private http: HttpClient, private envUrl: EnvironmentUrlService) { }

    public getSummaries() {
        let route = `/web/site/get-summaries`
        return this.http.get<Site[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public getSummary(id) {
        let route = `/web/site/get-summary?id=${id}`
        return this.http.get<Site>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    private createCompleteRoute = (route: string, envAddress: string) => {
        return `${envAddress}${route}`;
    }
}