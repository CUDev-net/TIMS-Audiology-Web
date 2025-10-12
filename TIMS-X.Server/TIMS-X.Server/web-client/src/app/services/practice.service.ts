import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { EnvironmentUrlService } from '../services/environment-url.service';

import { Entities } from '@app/entities/entities';
import PracticeSummary = Entities.PracticeSummary;
import HoursOfOperationModel = Entities.HoursOfOperationModel;

@Injectable({
    providedIn: 'root'
})
export class PracticeService {
    constructor(private http: HttpClient, private envUrl: EnvironmentUrlService) { }

    public get = () => {
        let route = `/web/practice/summary`;
        return this.http.get<PracticeSummary>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public getHours() {
        let route = `/web/practice/get-hours`
        return this.http.get<HoursOfOperationModel[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public getBusinessRules() {
        let route = `/web/practice/get-businessrules`
        return this.http.get<Entities.BusinessRuleDto>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    private createCompleteRoute = (route: string, envAddress: string) => {
        return `${envAddress}${route}`;
    }
}