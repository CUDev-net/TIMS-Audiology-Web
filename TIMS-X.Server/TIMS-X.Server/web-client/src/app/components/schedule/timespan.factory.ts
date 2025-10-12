import { Injectable } from "@angular/core";
import * as _ from 'underscore';
import { DateTime } from "luxon";
import { when } from "mobx";

import { TimsStore } from '@app/stores/tims.store';
import { LocalStorageService } from '@app/services/local-storage.service';

import { SchedulerStatic } from "@app/dhtmlx/dhtmlxscheduler";

import { Entities } from '@app/entities/entities';
import day_of_week = Entities.day_of_week;
import Site = Entities.Site;
import ProviderSummary = Entities.ProviderSummary;

@Injectable()
export class TimeSpanFactory {
    constructor(private ts: TimsStore, private localStorageService: LocalStorageService) {
    }
    public currentMode: string = null;
    public isInitialized: boolean = false;

    public InitializeHours(scheduler: SchedulerStatic) {
        this.ts.providerStore.getWithHours();
        this.ts.practiceStore.getHours();
        when(
            () => this.ts.providerStore.provider_w_Hours_list != null &&
                this.ts.practiceStore.hoursOfOperation.length > 0 &&
                this.ts.siteStore.site_list != null &&
                this.ts.lookupStore.scheduleBlock_list != null &&
                this.ts.userstore.currentUser != null,
            () => {
                this.AddProvidersToView(scheduler);
                this.AddSitesToView(scheduler);
                this._addPracticeHours(scheduler);
                let view = this.localStorageService.getItem('lastCalendarTimespan');
                if (view != null)
                    scheduler.setCurrentView(null, view);
                else {
                    if (this.ts.userstore.currentUser.user.lastCalendarTimespan == 2) {
                        scheduler.setCurrentView(null, 'week');
                    }
                    else if (this.ts.userstore.currentUser.user.lastCalendarTimespan == 3) {
                        scheduler.setCurrentView(null, 'month');
                    }
                    else {
                        scheduler.setCurrentView(null, 'day');
                    }
                }
                this.isInitialized = true;
            }
        );
        let that = this;
        scheduler.attachEvent("onViewChange", function (new_mode, new_date) {
            that.currentMode = new_mode;
            if (that.currentMode == 'provider') {
                let viewableProviders = that.GetViewableProviders();
                that._addProviderTimeSlots(scheduler, viewableProviders);
            }
            else if (that.currentMode == 'site') {
                let viewableSites = that.GetViewableSites();
                that._addSiteTimeSlots(scheduler, viewableSites);
            }
            else {
                if (that.ts.userstore.currentUser != null)
                    that._addPracticeHours(scheduler);
            }
        }, null);
    }

    private _addPracticeHours(scheduler: SchedulerStatic) {
        scheduler.deleteMarkedTimespan();
        _.each(this.ts.practiceStore.hoursOfOperation, (h) => {
            if (h.startTime && h.endTime) {
                let start = new Date(h.startTime);
                let startHour = start.getHours();
                let startMinutes = start.getMinutes();
                let end = new Date(h.endTime);
                let endHour = end.getHours();
                let endMinutes = end.getMinutes();
                scheduler.addMarkedTimespan(
                    {
                        days: [h.day],
                        zones: [0 * 60, (startHour * 60) + startMinutes, (endHour * 60) + endMinutes, 24 * 60],
                        css: 'closed-hour'
                    }
                );
            }
        })
        scheduler.updateView();
    }

    public AddProvidersToView(scheduler: SchedulerStatic) {
        let viewableProviders = this.GetViewableProviders();
        let pList = [];
        _.each(viewableProviders, (p) => {
            pList.push({ key: p.id, label: p.firstName + ' ' + p.lastName });
        });

        scheduler.createUnitsView({
            name: 'provider',
            property: 'provider_id', //the mapped data property
            list: pList,
            days: 1
        });

        scheduler.templates.provider_scale_text = function (key, label, unit, date) {
            let provider = _.find(viewableProviders, p => p.id == key);

            let backgroundR = parseInt(provider.webColor.substr(1, 2), 16), backgroundG = parseInt(provider.webColor.substr(3, 2), 16), backgroundB = parseInt(provider.webColor.substr(5, 2), 16);
            let backgroundDelta = (backgroundR * 0.03) + (backgroundG * 0.59) + (backgroundB * 0.11);
            let foreground = backgroundDelta > 128 ? '#333333' : '#ffffff';

            return `<div style="background: ${provider.webColor}90; margin-top:-2px; padding-top: 2px; height: 20px;"><b style="color: ${foreground}">${label}</b></div>`;
        }

        if (this.currentMode == 'provider') {
            this._addProviderTimeSlots(scheduler, viewableProviders);
        }
    }

    public AddSitesToView(scheduler: SchedulerStatic) {
        let viewableSites = this.GetViewableSites();
        let siteList = [];
        _.each(viewableSites, (p) => {
            siteList.push({ key: p.id, label: p.name });
        });

        scheduler.createUnitsView({
            name: 'site',
            property: 'site_id', //the mapped data property
            list: siteList,
            days: 1
        });

        scheduler.templates.site_scale_text = function (key, label, unit, date) {
            let site = _.find(viewableSites, p => p.id == key);

            let backgroundR = parseInt(site.webColor.substr(1, 2), 16), backgroundG = parseInt(site.webColor.substr(3, 2), 16), backgroundB = parseInt(site.webColor.substr(5, 2), 16);
            let backgroundDelta = (backgroundR * 0.03) + (backgroundG * 0.59) + (backgroundB * 0.11);
            let foreground = backgroundDelta > 128 ? '#333333' : '#ffffff';

            return `<div style="background: ${site.webColor}90; margin-top:-2px; padding-top: 2px; height: 20px;"><b style="color: ${foreground}">${label}</b></div>`;
        }
        if (this.currentMode == 'site') {
            this._addSiteTimeSlots(scheduler, viewableSites);
        }
    }

    public GetViewableProviders(): ProviderSummary[] {
        let viewableProviders: ProviderSummary[];
        if (!this.ts.userstore.currentUser.user.scheduleProviderFilter || this.ts.userstore.currentUser.user.scheduleProviderFilter == '0') {
            // Display all
            viewableProviders = this.ts.providerStore.provider_w_Hours_list;
        }
        else {
            let ids = this.ts.userstore.currentUser.user.scheduleProviderFilter.split('-');
            viewableProviders = _.filter(this.ts.providerStore.provider_w_Hours_list, (x) => _.contains(ids, x.id.toString()));
        }
        return viewableProviders;
    }

    public GetViewableSites(): Site[] {
        let viewableSites: Site[];
        if (!this.ts.userstore.currentUser.user.scheduleSiteFilter || this.ts.userstore.currentUser.user.scheduleSiteFilter == '0') {
            // Display all
            viewableSites = this.ts.siteStore.site_list;
        }
        else {
            let ids = this.ts.userstore.currentUser.user.scheduleSiteFilter.split('-');
            viewableSites = _.filter(this.ts.siteStore.site_list, (x) => _.contains(ids, x.id.toString()));
        }
        return viewableSites;
    }

    getSiteHours(day: number, site: Site) {
        switch (day) {
            case 0:
                return { startTime: site.sunStart, endTime: site.sunEnd };
            case 1:
                return { startTime: site.monStart, endTime: site.monEnd };
            case 2:
                return { startTime: site.tuesStart, endTime: site.tuesEnd };
            case 3:
                return { startTime: site.wedStart, endTime: site.wedEnd };
            case 4:
                return { startTime: site.thurStart, endTime: site.thurEnd };
            case 5:
                return { startTime: site.friStart, endTime: site.friEnd };
            case 6:
                return { startTime: site.satStart, endTime: site.satEnd };
        }
        throw new Error(`Unknown day ${day}`)
    }

    private _createMarkedTime(dateTime: Date) {
        let start = DateTime.fromISO(dateTime);
        let h = start.get('hour');
        let m = start.get('minute');
        return (h * 60) + m;
    }

    private _addProviderTimeSlots(scheduler: SchedulerStatic, viewableProviders: ProviderSummary[]) {
        scheduler.deleteMarkedTimespan();
        _.each(viewableProviders, (p) => {
            let dayKeys = Object.keys(day_of_week).filter((v) => !isNaN(Number(v)));

            let blocks =
                _.filter(this.ts.lookupStore.scheduleBlock_list, function (s) {
                    let a = _.some(s.providerBlockSchedules, (pb) => {
                        return p.id == pb.providerId;
                    });
                    return a;
                });

            _.each(dayKeys, day => {
                let dayHours = _.find(p.hours, h => h.day.toString() == day);
                if (dayHours) {
                    let start = new Date(dayHours.startTime);
                    let startHour = start.getHours();
                    let startMinutes = start.getMinutes();
                    let end = new Date(dayHours.endTime);
                    let endHour = end.getHours();
                    let endMinutes = end.getMinutes();
                    scheduler.addMarkedTimespan({
                        days: [dayHours.day],
                        zones: [0 * 60, (startHour * 60) + startMinutes, (endHour * 60) + endMinutes, 24 * 60],
                        css: 'closed-hour',
                        sections: { provider: p.id }

                    });
                }
                else {
                    scheduler.addMarkedTimespan({
                        days: [day],
                        zones: ['fullday'],
                        css: 'closed-hour',
                        sections: { provider: p.id }
                    });
                }
            });

            _.each(blocks, b => {
                _.each(b.providerBlockSchedules, pb => {
                    if (pb.providerId == p.id && pb.scheduleTimeSlot != null) {
                        let d = pb.scheduleTimeSlot.dayOfWeek == 7 ? 0 : pb.scheduleTimeSlot.dayOfWeek;
                        let startTime = pb.scheduleTimeSlot.startTime == null ? 0 : this._createMarkedTime(pb.scheduleTimeSlot.startTime);
                        let endTime = pb.scheduleTimeSlot.startTime == null ? 24 * 60 : this._createMarkedTime(pb.scheduleTimeSlot.endTime);
                        scheduler.addMarkedTimespan({
                            days: [d],
                            zones: [startTime, endTime],
                            css: 'block_hour',
                            html: `<div class="schedule-block" style="width: 100%; height: 100%; opacity: 0.7; background-color: ${b.color_web};"></div>`,
                            sections: { provider: p.id }
                        });

                    }
                });
            });
        });
        scheduler.updateView();
    }

    private _addSiteTimeSlots(scheduler: SchedulerStatic, viewableSites: Site[]) {
        scheduler.deleteMarkedTimespan();
        _.each(viewableSites, (site) => {
            let dayKeys = Object.keys(day_of_week).filter((v) => !isNaN(Number(v)));
            _.each(dayKeys, day => {
                let dayHours = this.getSiteHours(Number(day), site)
                if (dayHours) {
                    let start = new Date(dayHours.startTime);
                    let startHour = start.getHours();
                    let startMinutes = start.getMinutes();
                    let end = new Date(dayHours.endTime);
                    let endHour = end.getHours();
                    let endMinutes = end.getMinutes();
                    scheduler.addMarkedTimespan({
                        days: [Number(day)],
                        zones: [0 * 60, (startHour * 60) + startMinutes, (endHour * 60) + endMinutes, 24 * 60],
                        css: 'closed-hour',
                        sections: { site: site.id }

                    });
                }
                else {
                    scheduler.addMarkedTimespan({
                        days: [day],
                        zones: ['fullday'],
                        css: 'closed-hour',
                        sections: { site: site.id }
                    });
                }
            });
        });
        scheduler.updateView();
    }

}