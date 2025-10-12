import { Component, Injectable } from "@angular/core";
import { BsModalService, BsModalRef, ModalOptions } from 'ngx-bootstrap/modal';
import * as _ from 'underscore';
import { DateTime } from "luxon";
import { when } from "mobx";
import { isNumber } from "underscore";
import { scheduler, SchedulerStatic } from "@app/dhtmlx/dhtmlxscheduler";

import { TimsStore } from '@app/stores/tims.store';
import { TimeSpanFactory } from './timespan.factory';
import { DateUtils } from '@app/helpers/date-utils';

import { SignalrService } from '@app/services/signalr.service';
import { LocalStorageService } from '@app/services/local-storage.service';

import { AppointmentEditorComponent } from "../appointment-editor/appointment-editor.component";
import { ScheduleEditorComponent } from '../schedule-editor/schedule-editor.component';
import { SchedulerColorFactory } from './scheduler-color.factory';
import { StringBuilder } from '@app/utilities/stringbuilder';

import { Entities } from '@app/entities/entities';
import Schedule = Entities.Schedule;
import Appointment = Entities.Appointment;


@Injectable()
export class ScheduleStartUp {
    constructor(private ts: TimsStore,
        private modalService: BsModalService,
        private timeSpanFactory: TimeSpanFactory,
        private colorFactory: SchedulerColorFactory,
        private localStorageService: LocalStorageService,
        private signalrService: SignalrService) {
    }

    public calendarBrowserCalendars: any[] = [];
    public currentFrom: DateTime;
    public currentTo: DateTime;
    private timeFormat = 'h:mm a';
    private scheduler: SchedulerStatic;
    private bsModalRef: BsModalRef;
    private eventIDs: string[] = [];
    private dragged: any;

    public InitializeSchedule(scheduler: SchedulerStatic) {
        scheduler.templates.day_date = scheduler.date.date_to_str("%M %j, %Y");
        scheduler.templates.day_scale_date = scheduler.date.date_to_str("%D, %M %j");
        scheduler.templates.week_scale_date = scheduler.date.date_to_str("%D, %M %j");
        scheduler.templates.calendar_date = scheduler.date.date_to_str("%j");
        scheduler.templates.calendar_month = scheduler.date.date_to_str("%M %Y");

        // Config Scheduler
        if (this.ts.userstore.calendarInterval == 15) {
            //15 minutes
            scheduler.config.hour_size_px = 168;
            scheduler.xy.min_event_height = 84;
        }
        else {
            // 30 minutes
            scheduler.config.hour_size_px = 84;
            scheduler.xy.min_event_height = 42;
        }

        scheduler.config.hour_date = "%h:%i %a";
        //scheduler.config.time_step = 15;
        scheduler.config.mark_now = false;
        scheduler.config.start_on_monday = false;
        scheduler.config.scroll_hour = 8;
        scheduler.config.select = false;
        scheduler.config.drag_create = false;
        scheduler.config.dblclick_create = true;
        scheduler.config.details_on_dblclick = true;
        scheduler.config.details_on_create = true;
        this.scheduler = scheduler;

        this._wireCallbacks();

        scheduler.locale.labels.provider_tab = 'Provider';
        scheduler.locale.labels.site_tab = 'Site';
        this.timeSpanFactory.InitializeHours(scheduler);

        this.signalrService.startConnection(scheduler);

        /*************************************************************************************
        * Recurring overrides
        **************************************************************************************/
        this.scheduler.transpose_type = function (type) {
            var transposeFunctionName = "transpose_" + type;
            var occurencesFunctionName = "occurrences_" + type;
            if (!this.date[transposeFunctionName]) {
                var str = type.split("_");
                var day = 60 * 60 * 24 * 1000;
                var addFunctionName = "add_" + type;
                var step = this.transponse_size[str[0]] * str[1];

                if (str[0] == "day" || str[0] == "week") {
                    var days = null;
                    if (str[4]) {
                        days = str[4].split(",");
                        if (scheduler.config.start_on_monday) {
                            for (var i = 0; i < days.length; i++)
                                days[i] = (days[i] * 1) || 7;
                            days.sort();
                        }
                    }

                    this.date[transposeFunctionName] = function (nd, td) {
                        var delta = Math.floor((td.valueOf() - nd.valueOf()) / (day * step));
                        if (delta > 0)
                            nd.setDate(nd.getDate() + delta * step);
                        if (days)
                            scheduler.transpose_day_week(nd, days, 1, step);
                    };
                    this.date[occurencesFunctionName] = function (startDate, endDate) {
                        var delta = Math.floor((endDate.valueOf() - startDate.valueOf()) / (day * step));
                        if (delta < 0)
                            return 0;

                        return delta * (days ? days.length : 1);
                    };
                    this.date[addFunctionName] = function (sd, inc) {
                        var nd = new Date(sd.valueOf());
                        if (days) {
                            for (var count = 0; count < inc; count++)
                                scheduler.transpose_day_week(nd, days, 0, step);
                        } else
                            nd.setDate(nd.getDate() + inc * step);

                        return nd;
                    };
                }
                else if (str[0] == "month" || str[0] == "year") {
                    this.date[transposeFunctionName] = function (nd, td) {
                        var delta = Math.ceil(((td.getFullYear() * 12 + td.getMonth() * 1) - (nd.getFullYear() * 12 + nd.getMonth() * 1)) / (step));
                        if (delta >= 0)
                            nd.setMonth(nd.getMonth() + delta * step);
                        if (str[3])
                            scheduler.date.day_week(nd, str[2], str[3]);
                    };
                    this.date[occurencesFunctionName] = function (startDate, endDate) {
                        var endDateMonths = endDate.getFullYear() * 12 + endDate.getMonth() * 1,
                            startDateMonths = startDate.getFullYear() * 12 + startDate.getMonth() * 1,
                            delta = Math.ceil((endDateMonths - startDateMonths) / step);

                        if (delta < 0)
                            return 0;

                        return delta;
                    };
                    this.date[addFunctionName] = function (sd, inc) {
                        var nd = new Date(sd.valueOf());
                        nd.setMonth(nd.getMonth() + inc * step);
                        if (str[3])
                            scheduler.date.day_week(nd, str[2], str[3]);
                        return nd;
                    };
                }
            }
        };

        this.scheduler.repeat_date = function (ev, stack, non_render, from, to) {
            from = from || this._min_date;
            to = to || this._max_date;

            var td = new Date(ev.start_date.valueOf()),
                occurrenceIndex,
                deletedOccurrences = ev.deletedOccurrences,
                occurrenceNumber,
                skipOccurrence,
                i,
                length;

            if (!ev.rec_pattern && ev.rec_type)
                ev.rec_pattern = ev.rec_type.split("#")[0];

            this.transpose_type(ev.rec_pattern);
            occurrenceIndex = scheduler.date["occurrences_" + ev.rec_pattern](td, from);
            scheduler.date["transpose_" + ev.rec_pattern](td, from);

            while (td < ev.start_date || scheduler._fix_daylight_saving_date(td, from, ev, td, new Date(td.valueOf() + ev.event_length * 1000)).valueOf() <= from.valueOf() || td.valueOf() + ev.event_length * 1000 <= from.valueOf()) {
                td = scheduler.date.add(td, 1, ev.rec_pattern);
                occurrenceIndex += 1;
            }

            while (td < to && td < ev.end_date) {
                // We will simply skip deleted occurrences. Determine if we are skipping this one:
                skipOccurrence = false;
                occurrenceNumber = occurrenceIndex + 1;
                if ((length = deletedOccurrences.length) > 0) {
                    for (i = 0; i < length; i++) {
                        if (deletedOccurrences[i] === occurrenceNumber) {
                            skipOccurrence = true;
                            break;
                        }
                    }
                }

                // If we are not skipping this occurrence, then proceed.
                if (!skipOccurrence) {
                    var timestamp = (scheduler.config.occurrence_timestamp_in_utc) ? Date.UTC(td.getFullYear(), td.getMonth(), td.getDate(), td.getHours(), td.getMinutes(), td.getSeconds()) : td.valueOf();
                    var ch = this._get_rec_marker(timestamp, ev.id);
                    if (!ch) { // unmodified element of series
                        var ted = new Date(td.valueOf() + ev.event_length * 1000);
                        var copy = this._copy_event(ev);
                        //copy._timed = ev._timed;
                        copy.text = ev.text;
                        copy.start_date = td;
                        copy.event_pid = ev.id;
                        copy.id = ev.id + "#" + Math.ceil(timestamp / 1000);
                        copy.end_date = ted;
                        copy.occurrence_number = occurrenceNumber;
                        // needed for is_one_day_event
                        copy.rec_type = ev.rec_type;
                        copy.end_date = scheduler._fix_daylight_saving_date(copy.start_date, copy.end_date, ev, td, copy.end_date);
                        copy._timed = this.is_one_day_event(copy);

                        if (!copy._timed && !this._table_view && !this.config.multi_day) return;
                        stack.push(copy);

                        if (!non_render) {
                            this._events[copy.id] = copy;
                            this._rec_temp.push(copy);
                        }

                    } else if (non_render) {
                        stack.push(ch);
                    }
                }

                // Update the occurrence date and index.
                td = scheduler.date.add(td, 1, ev.rec_pattern);
                occurrenceIndex += 1;
            }
        };

        this.modalService.onHidden.subscribe(() => {
            this.ts.providerStore.removeInactiveProviders();
            this.ts.siteStore.removeInactiveSites();
        });
    }

    public RefreshProviders() {
        this.timeSpanFactory.AddProvidersToView(this.scheduler);
    }

    public RefreshSites() {
        this.timeSpanFactory.AddSitesToView(this.scheduler);
    }

    public setFilters() {
        this.scheduler.clearAll();
        let calendarBrowserMinDate = DateTime.fromJSDate(this.calendarBrowserCalendars[0]._min_date);
        let calendarBrowserMaxDate = DateTime.fromJSDate(this.calendarBrowserCalendars[2]._max_date).minus({ seconds: 1 });
        this.currentFrom = null;
        this.currentTo = null;
        this.updateItemStore(calendarBrowserMinDate, calendarBrowserMaxDate);
    }

    public updateItemStore(from: DateTime, to: DateTime) {
        // Initialize
        if (!from || !to) return;
        if (!this.currentFrom && !this.currentTo) {
            this.fetchCalendarItems(from, to.minus({ seconds: 1 }));
            this.fetchRecurringCalendarItems(from, to.minus({ seconds: 1 }));
            this.currentFrom = from;
            this.currentTo = to;
            return;
        }

        // Previous 
        if (from < this.currentFrom) {
            this.fetchCalendarItems(from, this.currentFrom.minus({ seconds: 1 }));
            this.currentFrom = from;
        }
        // Next
        if (to > this.currentTo) {
            this.fetchCalendarItems(this.currentTo, to);
            this.currentTo = to;
        }
    }

    // All dates are inclusive
    private fetchCalendarItems(from: DateTime, to: DateTime) {
        let f = from.toFormat(DateUtils.Constants.API_DATE_FORMAT);
        let t = to.toFormat(DateUtils.Constants.API_DATE_FORMAT);
        this.ts.schedulerStore.getScheduleItems(f, t);
        when(
            () => this.ts.schedulerStore.is_fetching_schedules == false && this.ts.userstore.currentUser != null,
            () => {
                let x = this.ts.schedulerStore.all_schedule_items;
                this.scheduler.parse(x);
            }
        );
        this.ts.schedulerStore.getAppointmentItems(f, t);
        when(
            () => this.ts.schedulerStore.is_fetching_appointments == false && this.ts.userstore.currentUser != null,
            () => {
                this.scheduler.parse(this.ts.schedulerStore.all_appointment_items)
            }
        );
    }

    private fetchRecurringCalendarItems(from: DateTime, to: DateTime) {
        let f = from.toFormat(DateUtils.Constants.API_DATE_FORMAT);
        let t = to.toFormat(DateUtils.Constants.API_DATE_FORMAT);
        this.ts.schedulerStore.getRecurrringScheduleItems(f, t);
        when(
            () => this.ts.schedulerStore.is_fetching_recurring_schedules == false && this.ts.userstore.currentUser != null,
            () => {
                let recurringSchedules = this.ts.schedulerStore.all_recurring_schedule_items;
                this.scheduler.parse(recurringSchedules);
            }
        );
    }

    public dispose() {
        this.eventIDs.forEach(e => scheduler.detachEvent(e));
        this.eventIDs = [];
    }

    public safeAddCalendarItem(a) {
        if (!a) return;
        let event = this.scheduler.getEvent(a.id);
        if (!event) {
            this.scheduler.addEvent(a);
        }
    }

    private _wireCallbacks() {
        let that = this;

        const onViewChange = this.scheduler.attachEvent("onViewChange", function (mode, date) {
            if (!that.calendarBrowserCalendars || that.calendarBrowserCalendars.length == 0) return;
            var state = that.scheduler.getState();
            let calendarBrowserMinDate = DateTime.fromJSDate(that.calendarBrowserCalendars[0]._min_date);
            let calendarBrowserMaxDate = DateTime.fromJSDate(that.calendarBrowserCalendars[2]._max_date).minus({ seconds: 1 });
            // console.log('calendarBrowserMinDate ' + calendarBrowserMinDate.toFormat(DateUtils.Constants.API_DATE_FORMAT));
            // console.log('calendarBrowserMaxDate ' + calendarBrowserMaxDate.toFormat(DateUtils.Constants.API_DATE_FORMAT));
            let schedulerMinDate = DateTime.fromJSDate(state.min_date);
            let schedulerMaxDate = DateTime.fromJSDate(state.max_date);
            // console.log('schedulerMinDate ' + schedulerMinDate.toFormat(DateUtils.Constants.API_DATE_FORMAT));
            // console.log('schedulerMaxDate ' + schedulerMaxDate.toFormat(DateUtils.Constants.API_DATE_FORMAT));

            if (schedulerMinDate < calendarBrowserMinDate) {
                that.updateCalendarBrowser(-1);
            }
            if (schedulerMaxDate > calendarBrowserMaxDate) {
                that.updateCalendarBrowser(1);
            }

            // Get new dates based on calendars
            calendarBrowserMinDate = DateTime.fromJSDate(that.calendarBrowserCalendars[0]._min_date);
            calendarBrowserMaxDate = DateTime.fromJSDate(that.calendarBrowserCalendars[2]._max_date).minus({ seconds: 1 });
            that.updateItemStore(calendarBrowserMinDate, calendarBrowserMaxDate);
            if (that.ts.userstore.currentUser != null && that.timeSpanFactory.isInitialized) {
                that.localStorageService.setItem('lastCalendarTimespan', mode);
                if (mode == 'week') {
                    that.ts.userstore.currentUser.user.lastCalendarTimespan = 2;
                }
                else if (mode == 'month') {
                    that.ts.userstore.currentUser.user.lastCalendarTimespan = 3;
                }
                else {
                    that.ts.userstore.currentUser.user.lastCalendarTimespan = 1;
                }
                that.ts.userstore.update(that.ts.userstore.currentUser);
            }
        });
        this.eventIDs.push(onViewChange);

        const onBeforeChanged = this.scheduler.attachEvent("onBeforeEventChanged", function (event, e, is_new, originalEvent) {
            console.log('onBeforeEventChanged');
            if (is_new) return true; // New Schedule event

            let sourceType = event.id.substring(0, 1);
            let newStartDate = event.start_date;
            let newEndDate = event.end_date;
            let newProviderId = event.provider_id;
            let newSiteId = event.site_id;
            if (sourceType === 'A') {
                let appointmentId = event.id.slice(2);
                that.ts.appointmentStore.getById(Number(appointmentId));
                when(() => that.ts.appointmentStore.inprogress == false,
                    () => {
                        that.ts.appointmentStore.selected_appointment.startsAt = newStartDate;
                        that.ts.appointmentStore.selected_appointment.endsAt = newEndDate;
                        that.ts.appointmentStore.selected_appointment.providerId = newProviderId;
                        that.ts.appointmentStore.selected_appointment.siteId = newSiteId;
                        let scheduler = that.scheduler;
                        const initialState: ModalOptions = { initialState: { scheduler }, class: 'modal-lg', ignoreBackdropClick: true };
                        that.bsModalRef = that.modalService.show(AppointmentEditorComponent, initialState);
                        that.bsModalRef.content.onClose.subscribe(result => {
                            if (!result) {
                                event.start_date = originalEvent.start_date;
                                event.end_date = originalEvent.end_date;
                                event.provider_id = originalEvent.provider_id;
                                event.site_id = originalEvent.site_id;
                                that.scheduler.updateEvent(event.id);
                            }
                        });
                    });
            }
            else if (sourceType === 'X') {
                let scheduleId = event.id.slice(2);
                let recurIndex = scheduleId.indexOf('#');
                if (recurIndex > -1) {
                    return false;
                }

                that.ts.scheduleStore.getById(Number(scheduleId));
                when(() => that.ts.scheduleStore.inprogress == false,
                    () => {
                        that.ts.scheduleStore.selected_schedule.startsAt = newStartDate;
                        that.ts.scheduleStore.selected_schedule.endsAt = newEndDate;
                        that.ts.scheduleStore.selected_schedule.providerId = newProviderId;
                        that.ts.scheduleStore.selected_schedule.siteId = newSiteId;
                        let scheduler = that.scheduler;
                        const initialState: ModalOptions = { initialState: { scheduler }, class: 'modal-lg', ignoreBackdropClick: true };
                        that.bsModalRef = that.modalService.show(ScheduleEditorComponent, initialState);
                        that.bsModalRef.content.onClose.subscribe(result => {
                            if (!result) {
                                event.start_date = originalEvent.start_date;
                                event.end_date = originalEvent.end_date;
                                event.provider_id = originalEvent.provider_id;
                                event.site_id = originalEvent.site_id;
                                that.scheduler.updateEvent(event.id);
                            }
                        });
                    });
            }
            return true;
        }, null);
        this.eventIDs.push(onBeforeChanged);

        document.addEventListener("dragstart", function (event) {
            // store a ref. on the dragged elem
            that.dragged = event.target;
            // make it half transparent
            let e: any = event;
            e.target.style.opacity = .5;
        }, false);

        document.addEventListener("dragend", function (event) {
            // reset the transparency
            let e: any = event;
            e.target.style.opacity = "";
        }, false);

        document.addEventListener("dragover", function (event) {
            // prevent default to allow drop
            let e: any = event;
            // console.log(e.target.parentElement.parentElement.classList);
            if (e.target.parentElement.classList.contains("dhx_cal_data") || e.target.parentElement.classList.contains("dhx_scale_holder_now") || e.target.classList.contains("schedule-block"))
                event.preventDefault();
        }, false);

        that.scheduler.templates.week_scale_date = function (date: Date) {
            let now = DateTime.now(); // 0-6
            var x = date.getDay();
            if (date.getDay() == now.weekday)
                return '<div style="background: #d0f2ff; margin-top:-2px; padding-top: 2px; height: 20px;"><b>' + now.toFormat('EEE MMM d') + '</b></div>';
            else
                return DateTime.fromJSDate(date).toFormat('EEE, MMM d');
        }

        document.addEventListener("drop", function (event) {
            var action_data = that.scheduler.getActionData(event);
            let canAddAppointment = that.ts.userstore.doesUserHaveSetting(Entities.SettingEnum.canCreateAppointments);
            if (!canAddAppointment) return;
            let e: any = event;
            if (e.target.parentElement.classList.contains("dhx_cal_data") || e.target.parentElement.classList.contains("dhx_scale_holder_now") || e.target.classList.contains("schedule-block")) {
                that.ts.appointmentStore.new_appointment_summary = null; // Clear
                that.ts.appointmentStore.selected_appointment = new Appointment();
                that.ts.appointmentStore.selected_appointment.patientId = that.ts.patientStore.new_appointment_patientId;
                that.ts.appointmentStore.selected_appointment.id = 0;
                let appointment_date = action_data.date;
                let ms = 1000 * 60 * that.ts.userstore.calendarInterval; // convert minutes to ms
                let roundedDate = new Date(Math.round(appointment_date.getTime() / ms) * ms);
                that.ts.appointmentStore.selected_appointment.startsAt = roundedDate;
                if (action_data.section) {
                    if (that.timeSpanFactory.currentMode == 'provider') {
                        that.ts.appointmentStore.selected_appointment.providerId = action_data.section;
                    }
                    else if (that.timeSpanFactory.currentMode == 'site')
                        that.ts.appointmentStore.selected_appointment.siteId = action_data.section;
                }
                that.bsModalRef = that.modalService.show(AppointmentEditorComponent, Object.assign({}, { class: 'modal-lg', ignoreBackdropClick: true }));
                that.bsModalRef.content.onClose.subscribe(result => {
                    if (result) {
                        when(() => that.ts.appointmentStore.inprogress == false,
                            () => {
                                that.safeAddCalendarItem(that.ts.appointmentStore.new_appointment_summary);
                            }
                        );
                    }
                });
            }
            //console.log('drop');
        });

        // Day/week events 
        that.scheduler.templates.event_header = function (start, end, event) {
            let id = String(event.id);
            let sourceType = id.substring(0, 1);
            if (sourceType === 'X' && event.id.includes("#")) {
                return DateTime.fromJSDate(start).toFormat(that.timeFormat) + ' &ndash; ' + DateTime.fromJSDate(end).toFormat(that.timeFormat) + '<i style="margin-left: 5px" class="dialog-button bi bi-arrow-repeat"></i>';
            }
            else {
                return DateTime.fromJSDate(start).toFormat(that.timeFormat) + ' &ndash; ' + DateTime.fromJSDate(end).toFormat(that.timeFormat);
            }
        };

        that.scheduler.templates.tooltip_text = function (start, end, event) {
            let sourceType = event.id.substring(0, 1);

            let time = DateTime.fromJSDate(start).toFormat(that.timeFormat) + ' &ndash; ' + DateTime.fromJSDate(end).toFormat(that.timeFormat);
            let html = '<table><tbody>';
            html += '<tr><th>Time:</th><td>' + time + '</td></tr>';
            if (sourceType === 'A') {
                html += event.patient_name ? '<tr><th>Patient:</th><td>' + event.patient_name + '</td></tr>' : '';
                html += event.appointment_type_name ? '<tr><th>Appt Type:</th><td>' + event.appointment_type_name + '</td></tr>' : '';
                html += event.status_name ? '<tr><th>Appt Status:</th><td>' + event.status_name + '</td></tr>' : '';
                html += event.site_name ? '<tr><th>Site:</th><td>' + event.site_name + '</td></tr>' : '';
            }
            else if (sourceType === 'X') {
                html += event.title ? '<tr><th>Title:</th><td>' + event.title + '</td></tr>' : '';
                html += event.location ? '<tr><th>Location:</th><td>' + event.location + '</td></tr>' : '';
                html += event.site_name ? '<tr><th>Site:</th><td>' + event.site_name + '</td></tr>' : '';
            }
            html += event.provider_name ? '<tr><th>Provider:</th><td>' + event.provider_name + '</td></tr>' : '';
            html += event.notes ? '<tr><th>Notes:</th><td>' + event.notes + '</td></tr>' : '';
            html += '</tbody></table>';
            return html;
        }

        that.scheduler.templates.event_text = function (start, end, event) {
            var builder = new StringBuilder();
            let s = that.scheduler as any;
            let viewMode = s._mode;
            let id = String(event.id);
            let sourceType = id.substring(0, 1);
            if (!event) return null;

            builder.append('<span class="swatch" style="background-color: ' + event.provider_web_color + '"></span>');
            event.textColor = '#FFFFFF';

            if (sourceType === 'A') {
                builder.append('<strong>');
                builder.append(event.patient_name);
                builder.append('</strong>');
                builder.append(': ');
                builder.append(event.appointment_type_name);
                that.colorFactory.setAppointmentBackground(event);
            }
            else if (sourceType === 'X') {
                builder.append('<strong>');
                builder.append(event.title);
                builder.append('</strong>');
                that.colorFactory.setScheduleBackground(event);
            }
            else {
                // new schedule

                return null;
            }
            if (viewMode !== 'site') builder.append(event.site_name, ' at ');
            if (viewMode !== 'provider') builder.append(event.provider_name, ' with ');

            return builder.toString();
        }

        that.scheduler.templates.hour_scale = function (date) {
            let s = that.scheduler as any;
            var interval = that.ts.userstore.calendarInterval;
            var dateHour = date.getHours(),
                top = (dateHour === 0) ? 'am' : (dateHour === 12) ? 'pm' : interval == 15 ? '15' : '30',
                bottom = interval == 15 ? '45' : '30',
                hour = ((dateHour + 11) % 12) + 1,
                sectionWidth = Math.floor(scheduler.xy.scale_width / 2),
                sectionHeight = s.config.hour_size_px,
                minuteHeight = Math.floor(s.config.hour_size_px / 2),
                html = '';

            html += "<div class='dhx_scale_hour_main' style='width: " + sectionWidth + "px; height:" + sectionHeight + "px; line-height: " + sectionHeight + "px;'>" + hour + "</div>";
            html += "<div class='dhx_scale_hour_minute_cont' style='width: " + sectionWidth + "px;'>";
            html += "<div class='dhx_scale_hour_minute_top' style='height:" + minuteHeight + "px; line-height:" + minuteHeight + "px;'>" + top + "</div>";
            html += "<div class='dhx_scale_hour_minute_bottom' style='height:" + minuteHeight + "px; line-height:" + minuteHeight + "px;'>" + bottom + "</div>";
            html += "<div class='dhx_scale_hour_sep' style='top: " + minuteHeight + "px;'></div></div>";

            return html;
        };

        that.scheduler.showLightbox = function (id) {
            let event = that.scheduler.getEvent(id);
            if (isNumber(id)) {
                let s = that.scheduler as any;
                let viewMode = s._mode;
                let canAdd = that.ts.userstore.doesUserHaveSetting(Entities.SettingEnum.canCreateNonPatientAppointments);
                if (!canAdd || viewMode === 'month') {
                    that.scheduler.deleteEvent(id);
                    return;
                }

                // New Schedule Event
                that.ts.scheduleStore.selected_schedule = new Schedule();
                // Round to nearest 30 min
                let schedule_date = event.start_date;
                let ms = 1000 * 60 * that.ts.userstore.calendarInterval; // convert minutes to ms
                let roundedDate = new Date(Math.round(schedule_date.getTime() / ms) * ms);
                that.ts.scheduleStore.selected_schedule.startsAt = roundedDate;
                that.ts.scheduleStore.selected_schedule.id = 0;
                if (event.provider_id)
                    that.ts.scheduleStore.selected_schedule.providerId = event.provider_id;
                if (event.site_id)
                    that.ts.scheduleStore.selected_schedule.siteId = event.site_id;
                let x = this.scheduler;
                const initialState: ModalOptions = { initialState: { x }, class: 'modal-lg', ignoreBackdropClick: true };
                that.bsModalRef = that.modalService.show(ScheduleEditorComponent, initialState);
                that.bsModalRef.content.onClose.subscribe(result => {
                    if (result) {
                        when(() => that.ts.scheduleStore.inprogress == false,
                            () => {
                                if (that.ts.scheduleStore.new_recurring_schedules != null) {
                                    _.each(that.ts.scheduleStore.new_recurring_schedules, (x) => {
                                        that.safeAddCalendarItem(x);
                                    });
                                }
                                else {
                                    _.each(that.ts.scheduleStore.new_schedules, (x) => {
                                        that.safeAddCalendarItem(x);
                                    });
                                }
                            }
                        );
                    }
                });
                that.scheduler.deleteEvent(id);
            }
            else {
                let sourceType = id.substring(0, 1);
                if (sourceType === 'A') {
                    let appointmentId = id.slice(2);
                    that.ts.appointmentStore.getById(Number(appointmentId));
                    when(() => that.ts.appointmentStore.inprogress == false,
                        () => {
                            let scheduler = that.scheduler;
                            const initialState: ModalOptions = { initialState: { scheduler }, class: 'modal-lg', ignoreBackdropClick: true };
                            that.bsModalRef = that.modalService.show(AppointmentEditorComponent, initialState);
                            that.bsModalRef.content.onClose.subscribe(result => {
                                if (result) {
                                    that.scheduler.updateEvent(event.id);
                                }
                            });
                        });
                }
                else if (sourceType === 'X') {
                    let occurrenceNumber = -1;
                    let occurrenceDate = null;
                    if (event.occurrence_number) {
                        occurrenceNumber = event.occurrence_number;
                        occurrenceDate = event.start_date;
                    }
                    let scheduleId = event.id;
                    if (event.event_pid) {
                        scheduleId = event.event_pid;
                    }
                    scheduleId = scheduleId.slice(2);
                    that.ts.scheduleStore.getById(Number(scheduleId));
                    when(() => that.ts.scheduleStore.inprogress == false,
                        () => {
                            let scheduler = that.scheduler;
                            const initialState: ModalOptions = { initialState: { scheduler, id, occurrenceNumber, occurrenceDate }, class: 'modal-lg', ignoreBackdropClick: true };
                            that.bsModalRef = that.modalService.show(ScheduleEditorComponent, initialState);
                            that.bsModalRef.content.onClose.subscribe(result => {
                                if (result) {
                                    when(() => that.ts.scheduleStore.inprogress == false,
                                        () => {
                                            let a = _.find(that.ts.schedulerStore.all_recurring_schedule_items, (x) => { return x.id == event.event_pid });
                                            that.scheduler.updateEvent(event.event_pid);
                                        });
                                }
                            });
                        });
                }
            }
        };
    }

    public createMiniCalendars() {
        let schedulerDate: Date = this.scheduler.getState().date;
        let plusOne = DateTime.local(schedulerDate).plus({ months: 1 }).toISODate();
        let plusTwo = DateTime.local(schedulerDate).plus({ months: 2 }).toISODate();
        this.calendarBrowserCalendars = [
            this.createMiniCal(schedulerDate),
            this.createMiniCal(plusOne),
            this.createMiniCal(plusTwo)
        ];
    }

    public getPrevMonthForMiniCal() {
        let c1 = this.calendarBrowserCalendars[0];
        let newDate = DateTime.fromJSDate(c1._date).minus({ months: 1 });
        if (newDate < this.currentFrom) {
            let to = this.currentFrom.minus({ days: 1 });
            this.updateItemStore(newDate, to);
        }
        this.updateCalendarBrowser(-1);
    }

    public getNextMonthForMiniCal() {
        let c1 = this.calendarBrowserCalendars[2];
        let newDate = DateTime.fromJSDate(c1._date).plus({ months: 1 }).endOf('month');
        if (newDate > this.currentTo) {
            let from = this.currentTo.plus({ days: 1 });
            this.updateItemStore(from, newDate);
        }
        this.updateCalendarBrowser(1);
    }

    private createMiniCal(date: Date) {
        let calendar = this.scheduler.renderCalendar({
            container: "calendar-browser",
            date: date,
            sync: false,
            handler: (date, calendar) => {
                let s = this.scheduler as any;
                let viewMode = s._mode;
                this.scheduler.setCurrentView(date, viewMode);
            }
        });
        this.wireCalendarBehaviors(calendar);
        return calendar;
    }

    private wireCalendarBehaviors(calendar) {
        let that = this;
        var action = function () {
            that.scheduler.updateCalendar(calendar, calendar._date);
            return true;
        };

        that.scheduler.attachEvent("onViewChange", action);
        that.scheduler.attachEvent("onXLE", action);
        that.scheduler.attachEvent("onEventAdded", action);
        that.scheduler.attachEvent("onEventChanged", action);
        that.scheduler.attachEvent("onEventDeleted", action);
        action();
    }

    private updateCalendarBrowser(months: number) {
        _.each(this.calendarBrowserCalendars, (c) => {
            let currentDate = c._date;
            let newDate = scheduler.date.add(currentDate, months, 'month');
            this.scheduler.updateCalendar(c, newDate);
        });
    }
}