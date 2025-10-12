import { Component, OnInit } from '@angular/core';
import { DateTime } from "luxon";
import { when } from "mobx";
import * as _ from 'underscore';

import { TimsStore } from '@app/stores/tims.store';
import { DateUtils } from '@app/helpers/date-utils';

import { Entities } from '@app/entities/entities';

@Component({
  selector: 'app-mobile-schedule',
  templateUrl: './mobile-schedule.component.html',
  styleUrl: './mobile-schedule.component.scss'
})
export class MobileScheduleComponent implements OnInit {
  constructor(public ts: TimsStore) {
    this.currentDate = DateTime.now();
    this.selectedDate = this.currentDate.toJSDate();
  }

  public selectedDate?: Date;
  public currentDate: DateTime;
  public selectedProvider: number = null;

  public get currentDisplayDate(): string {
    return this.currentDate.toLocaleString(DateTime.DATE_MED);
  }
  public scheduleItems: Entities.ScheduleItem[] = [];

  ngOnInit(): void {
    this.fetchCalendarItems();
  }

  onDateChanged(value: Date): void {
    this.scheduleItems = [];
    this.selectedDate = value;
    this.currentDate = DateTime.fromJSDate(this.selectedDate);
    this.fetchCalendarItems();
  }

  onSelectedProviderChanged() {
    this.scheduleItems = [];
    this.loadScheduleItems();
    this.loadAppointmentItems();
    this.loadRecurringScheduleItems();
  }

  onPreviousDay() {
    this.scheduleItems = [];
    this.currentDate = this.currentDate.minus({ days: 1 });
    this.selectedDate = this.currentDate.toJSDate();
    this.fetchCalendarItems();
  }

  onNextDay() {
    this.scheduleItems = [];
    this.currentDate = this.currentDate.plus({ days: 1 });
    this.selectedDate = this.currentDate.toJSDate();
    this.fetchCalendarItems();
  }

  loadScheduleItems() {
    for (let schedule of this.ts.schedulerStore.all_schedule_items) {
      if (this.selectedProvider != null && this.selectedProvider != schedule.provider_id)
        continue;
      var exists = _.find(this.scheduleItems, (x) => { return x.id == schedule.id });
      if (exists)
        continue;
      let item = new Entities.ScheduleItem();
      item.id = schedule.id;
      item.title = schedule.title;
      item.site_name = schedule.site_name;
      item.provider_name = schedule.provider_name;
      item.start_date = new Date(schedule.start_date);
      item.end_date = new Date(schedule.end_date);
      item.type = 'X';
      item.provider_web_color = schedule.provider_web_color;
      item.background_color = schedule.color_web;
      item.notes = schedule.notes;
      this.scheduleItems.push(item);
    }
    this.scheduleItems = _.sortBy(this.scheduleItems, 'start_date');
  }

  loadAppointmentItems() {
    for (let schedule of this.ts.schedulerStore.all_appointment_items) {
      if (this.selectedProvider != null && Number(this.selectedProvider) != schedule.provider_id)
        continue;
      var exists = _.find(this.scheduleItems, (x) => { return x.id == schedule.id });
      if (exists)
        continue;
      let item = new Entities.ScheduleItem();
      item.id = schedule.id;
      item.provider_web_color = schedule.provider_web_color;
      item.patientId = schedule.patient_id;
      item.patient_name = schedule.patient_name;
      item.site_name = schedule.site_name;
      item.site_web_color = schedule.site_web_color;
      item.provider_name = schedule.provider_name;
      item.appointment_type_name = schedule.appointment_type_name;
      item.appointment_type_web_color = schedule.appointment_type_web_color;
      item.appointment_status_name = schedule.status_name;
      item.start_date = new Date(schedule.start_date);
      item.end_date = new Date(schedule.end_date);
      item.type = 'A';
      item.notes = schedule.notes;
      this.scheduleItems.push(item);
    }
    this.scheduleItems = _.sortBy(this.scheduleItems, 'start_date');
  }

  loadRecurringScheduleItems() {
    for (let schedule of this.ts.schedulerStore.all_recurring_schedule_items) {
      var exists = _.find(this.scheduleItems, (x) => { return x.id == schedule.id });
      if (exists)
        continue;
      let item = new Entities.ScheduleItem();
      item.id = schedule.id;
      item.title = schedule.title;
      let s = new Date(schedule.start_date);
      let e = new Date(schedule.end_date);
      item.start_date = new Date(
        this.currentDate.get('year'),
        this.currentDate.get('month'),
        this.currentDate.get('day'),
        s.getHours(),
        s.getMinutes(),
        0);
      item.end_date = new Date(
        this.currentDate.get('year'),
        this.currentDate.get('month'),
        this.currentDate.get('day'),
        e.getHours(),
        e.getMinutes(),
        0);
      item.type = 'X';
      item.site_name = schedule.site_name;
      item.provider_name = schedule.provider_name;
      item.provider_web_color = schedule.provider_web_color;
      item.background_color = schedule.color_web;
      item.notes = schedule.notes;
      item.isRecurring = true;
      this.scheduleItems.push(item);
    }
    this.scheduleItems = _.sortBy(this.scheduleItems, 'start_date');
  }

  fetchCalendarItems() {
    let scheduleDate = this.currentDate.startOf('day').toFormat(DateUtils.Constants.API_DATE_FORMAT);

    this.ts.schedulerStore.getScheduleItems(scheduleDate, scheduleDate);
    when(
      () => this.ts.schedulerStore.is_fetching_schedules == false && this.ts.userstore.currentUser != null,
      () => {
        this.loadScheduleItems();
      });
    this.ts.schedulerStore.getAppointmentItems(scheduleDate, scheduleDate);
    when(
      () => this.ts.schedulerStore.is_fetching_appointments == false && this.ts.userstore.currentUser != null,
      () => {
        this.loadAppointmentItems();
      });
    this.ts.schedulerStore.getRecurrringScheduleItemsForDay(scheduleDate);
    when(
      () => this.ts.schedulerStore.is_fetching_recurring_schedules == false && this.ts.userstore.currentUser != null,
      () => {
        this.loadRecurringScheduleItems();
      }
    );
  }
}
