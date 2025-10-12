import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { EnvironmentUrlService } from '../services/environment-url.service';

import { Entities } from '@app/entities/entities';
import Schedule = Entities.Schedule;
import ScheduleDto = Entities.ScheduleDto;
import ValidationResult = Entities.ValidationResult;
import RecurringIntervalRemoved = Entities.RecurringIntervalRemoved;

@Injectable({
    providedIn: 'root'
})
export class ScheduleService {
    constructor(private http: HttpClient, private envUrl: EnvironmentUrlService) { }

    private createCompleteRoute = (route: string, envAddress: string) => {
        return `${envAddress}${route}`;
    }

    public getById = (id: number) => {
        let route = `/web/schedule/get?id=${id}`
        return this.http.get<Schedule>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public create(schedule: Schedule) {
        schedule.id = 0;
        let route = `/web/schedule/create`;
        return this.http.post<ScheduleDto[]>(this.createCompleteRoute(route, this.envUrl.urlAddress), schedule);
    }

    public update(schedule: Schedule) {
        let route = `/web/schedule/${schedule.id}`;
        return this.http.put<ScheduleDto>(this.createCompleteRoute(route, this.envUrl.urlAddress), schedule);
    }

    public delete(id: number) {
        let occurrenceNumber = -1;
        let route = `/web/schedule/${id}`;
        return this.http.delete(this.createCompleteRoute(route, this.envUrl.urlAddress), { body: { id, occurrenceNumber } });
    }

    public deleteOccurrence(recurringIntervalRemoved: RecurringIntervalRemoved) {
        let route = `/web/schedule/delete-recurring-instance`;
        return this.http.post(this.createCompleteRoute(route, this.envUrl.urlAddress), recurringIntervalRemoved);
    }

    public validate(schedule: Schedule) {
        let route = `/web/schedule/validate`;
        return this.http.post<ValidationResult[]>(this.createCompleteRoute(route, this.envUrl.urlAddress), schedule);
    }
}