import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { EnvironmentUrlService } from '../services/environment-url.service';

import { Entities } from '@app/entities/entities';
import Resource = Entities.Resource;

@Injectable({
    providedIn: 'root'
})
export class ResourceService {

    constructor(private http: HttpClient, private envUrl: EnvironmentUrlService) { }

    public getSummaries() {
        let route = `/web/resource/get-all`
        return this.http.get<Resource[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    private createCompleteRoute = (route: string, envAddress: string) => {
        return `${envAddress}${route}`;
    }
}