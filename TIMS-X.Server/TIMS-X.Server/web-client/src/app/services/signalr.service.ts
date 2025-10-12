import { Injectable } from '@angular/core';
import * as signalR from "@microsoft/signalr";
import { when } from "mobx";
import * as _ from 'underscore';
import { SchedulerStatic } from "@app/dhtmlx/dhtmlxscheduler";

import { EnvironmentUrlService } from '@app/services/environment-url.service';

import { TimsStore } from '@app/stores/tims.store';

import { Entities } from '@app/entities/entities';
import AppointmentDto = Entities.AppointmentDto;
import ScheduleDto = Entities.ScheduleDto;
import RecurringIntervalRemoved = Entities.RecurringIntervalRemoved;


@Injectable({
    providedIn: 'root'
})
export class SignalrService {

    constructor(private environmentService: EnvironmentUrlService,
        public ts: TimsStore,) { }

    private hubConnection: signalR.HubConnection;
    private scheduler: SchedulerStatic;

    public startConnection(scheduler: SchedulerStatic) {
        this.scheduler = scheduler;
        when(() => this.ts.practiceStore.practiceSummary != null,
            () => {
                this.hubConnection = new signalR.HubConnectionBuilder()
                    .withUrl(`${this.environmentService.urlAddress}/web/schedulerhub?office=${this.ts.practiceStore.practiceSummary.officeCode}`)
                    .withAutomaticReconnect()
                    .build();

                this.addAppointmentCreatedListener();
                this.addAppointmentUpdatedListener();
                this.addAppointmentDeletedListener();
                this.addScheduleCreatedListener();
                this.addScheduleUpdatedListener();
                this.addScheduleDeletedListener();

                this.hubConnection
                    .start()
                    .then(
                        () => {
                            console.log('Connection started');
                            //this.addAppointmentUpdatedListener();
                        }
                    )
                    .catch(err => console.log('Error while starting connection: ' + err));
            }
        );
    }

    private addAppointmentCreatedListener() {
        this.hubConnection.on('OnAppointmentCreated', (data: AppointmentDto) => {
            let a = this.ts.appointmentStore.createAppointmentSummary(data);
            let event = this.scheduler.getEvent(a.id);
            if (!event) {
                this.scheduler.addEvent(a);
                this.scheduler.updateView();
            }
            console.log("OnAppointmentCreated");
        });
    }

    private addAppointmentUpdatedListener() {
        this.hubConnection.on('OnAppointmentUpdated', (data: AppointmentDto) => {
            let event = this.scheduler.getEvent('A-' + data.appointment.id);
            this.ts.appointmentStore.updateAppointmentSummary(event, data);
            this.scheduler.updateEvent(event.id);
            this.scheduler.updateView();
            console.log("OnAppointmentUpdated");
        });
    }

    private addAppointmentDeletedListener() {
        this.hubConnection.on('OnAppointmentDeleted', (data: number) => {
            let id = 'A-' + data;
            let event = this.scheduler.getEvent(id);
            if (event) {
                this.scheduler.deleteEvent(id);
                this.scheduler.updateView();
            }
            console.log("OnAppointmentDeleted");
        });
    }

    private addScheduleCreatedListener() {
        this.hubConnection.on('OnScheduleCreated', (data: ScheduleDto) => {
            let a = this.ts.scheduleStore.createScheduleSummary(data);
            let event = this.scheduler.getEvent(a.id);
            if (!event) {
                if (data.recurringItemSummary != null)
                    this.scheduler.addEvent(data.recurringItemSummary);
                else
                    this.scheduler.addEvent(a);
                this.scheduler.updateView();
            }
            console.log("OnScheduleCreated");
        });
    }

    private addScheduleUpdatedListener() {
        this.hubConnection.on('OnScheduleUpdated', (data: ScheduleDto, rr: RecurringIntervalRemoved) => {
            let event = this.scheduler.getEvent('X-' + data.schedule.id);
            if (!data.recurringItemSummary) {
                let a = _.find(this.ts.schedulerStore.all_schedule_items, (x) => { return x.id == event.id; });
                this.ts.scheduleStore.updateScheduleSummary(event, data);
                a.color_web = data.schedule.web_Color;
            }
            else {
                let a = _.find(this.ts.schedulerStore.all_recurring_schedule_items, (x) => { return x.id == event.id; });
                a.color_web = data.schedule.web_Color;
                a.location = data.schedule.location;
                a.title = data.schedule.title;
                a.deletedOccurrences.push(rr.itemNumber);
            }
            this.scheduler.updateEvent(event.id);
            this.scheduler.updateView();
            console.log("OnScheduleUpdated");
        });
    }

    private addScheduleDeletedListener() {
        this.hubConnection.on('OnScheduleDeleted', (data: number) => {
            let id = 'X-' + data;
            let event = this.scheduler.getEvent(id);
            if (event) {
                this.scheduler.deleteEvent(id);
                this.scheduler.updateView();
            }
            console.log("OnScheduleDeleted");
        });
    }


}