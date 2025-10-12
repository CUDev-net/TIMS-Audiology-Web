import { Injectable } from '@angular/core';
import { makeObservable, observable } from 'mobx';
import * as _ from 'underscore';

import { ScheduleService } from '../services/schedule.service';
import { BaseStore } from './base.store';

import { SchedulerStore } from './scheduler.store';

import { Entities } from '@app/entities/entities';
import Schedule = Entities.Schedule;
import ScheduleDto = Entities.ScheduleDto;
import ValidationResult = Entities.ValidationResult;
import ScheduleItemSummary = Entities.ScheduleItemSummary;
import ScheduleRecurringItemSummary = Entities.ScheduleRecurringItemSummary;
import RecurringIntervalRemoved = Entities.RecurringIntervalRemoved;

@Injectable()
export class ScheduleStore extends BaseStore {
    public selected_schedule: Schedule = null;
    public new_schedules: ScheduleItemSummary[] = null;
    public new_recurring_schedules: ScheduleRecurringItemSummary[] = null;
    @observable public validation_results: ValidationResult[] = null;

    constructor(private scheduleService: ScheduleService, private schedulerStore: SchedulerStore) {
        super();
        makeObservable(this);
    }

    public getById(id: number) {
        this.selected_schedule = null;
        this.inprogress = true;
        this.scheduleService.getById(id).subscribe((p) => {
            this.selected_schedule = p;
            this.inprogress = false;
        });
    }

    public validate(schedule: Schedule) {
        this.validation_results = null;
        this.inprogress = true;
        this.scheduleService.validate(schedule).subscribe((p: ValidationResult[]) => {
            this.validation_results = p;
            this.inprogress = false;
        });
    }

    public createScheduleSummary(scheduleSummary: ScheduleDto) {
        let schedule = new ScheduleItemSummary();
        schedule.id = 'X-' + scheduleSummary.schedule.id;
        schedule.title = scheduleSummary.schedule.title;
        schedule.start_date = scheduleSummary.schedule.startsAt;
        schedule.end_date = scheduleSummary.schedule.endsAt;
        schedule.location = scheduleSummary.schedule.location;
        schedule.provider_web_color = scheduleSummary.provider_Color;
        schedule.provider_name = scheduleSummary.providerName;
        schedule.provider_id = scheduleSummary.schedule.providerId;
        schedule.site_web_color = scheduleSummary.site_Color;
        schedule.site_name = scheduleSummary.siteName;
        schedule.site_id = scheduleSummary.schedule.siteId;
        schedule.color_web = scheduleSummary.schedule.web_Color;
        this.schedulerStore.all_schedule_items.push(schedule);
        return schedule;
    }

    public updateScheduleSummary(scheduleSummary: ScheduleItemSummary, scheduleDto: ScheduleDto) {
        scheduleSummary.provider_web_color = scheduleDto.provider_Color;
        scheduleSummary.site_web_color = scheduleDto.site_Color;
        scheduleSummary.start_date = new Date(scheduleDto.schedule.startsAt);
        scheduleSummary.end_date = new Date(scheduleDto.schedule.endsAt);
        scheduleSummary.title = scheduleDto.schedule.title;
        scheduleSummary.notes = scheduleDto.schedule.notes;
        scheduleSummary.provider_id = scheduleDto.schedule.providerId;
        scheduleSummary.provider_name = scheduleDto.providerName;
        scheduleSummary.site_id = scheduleDto.schedule.siteId;
        scheduleSummary.site_name = scheduleDto.siteName;
        scheduleSummary.color_web = scheduleDto.schedule.web_Color;
    }

    public create(schedule: Schedule) {
        this.new_schedules = new Array<ScheduleItemSummary>();
        this.new_recurring_schedules = new Array<ScheduleRecurringItemSummary>();
        this.inprogress = true;
        this.scheduleService.create(schedule).subscribe((p: ScheduleDto[]) => {
            if (p[0].recurringItemSummary) {
                _.each(p, (x) => {
                    this.new_recurring_schedules.push(x.recurringItemSummary);
                });
            }
            else {
                _.each(p, (x) => {
                    this.new_schedules.push(this.createScheduleSummary(x));
                });
            }
            this.inprogress = false;
        });
    }

    public update(schedule: Schedule) {
        this.inprogress = true;
        this.scheduleService.update(schedule).subscribe((p: ScheduleDto) => {
            this.selected_schedule = p.schedule;
            let a_id = 'X-' + schedule.id;
            if (!p.recurringItemSummary) {
                let a = _.find(this.schedulerStore.all_schedule_items, (x) => { return x.id == a_id; });
                this.updateScheduleSummary(a, p);
                a.color_web = p.schedule.web_Color;
            }
            else {
                // Recurring
                let a = _.find(this.schedulerStore.all_recurring_schedule_items, (x) => { return x.id == a_id; });
                a.color_web = p.schedule.web_Color;
                a.location = p.schedule.location;
                a.title = p.schedule.title;
            }
            this.inprogress = false;
        });
    }

    public delete(id: number) {
        this.scheduleService.delete(id).subscribe((isDeleted) => {
        });
    }

    public deleteOccurrence(recurringIntervalRemoved: RecurringIntervalRemoved) {
        this.scheduleService.deleteOccurrence(recurringIntervalRemoved).subscribe((r: RecurringIntervalRemoved) => {
        });
    }
}