import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Subject } from 'rxjs';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { SchedulerStatic } from "@app/dhtmlx/dhtmlxscheduler";
import { DateTime } from "luxon";
import * as _ from 'underscore';
import { when } from "mobx";
import { IDropdownSettings } from 'ng-multiselect-dropdown';

import { DateUtils } from '@app/helpers/date-utils';
import { TimeEntryComponent } from '@app/components/time-entry/time-entry.component';
import { RecurrenceEditorComponent } from '@app/components/recurrence-editor/recurrence-editor.component';
import { DurationEntryComponent } from '@app/components/duration-entry/duration-entry.component';

import { TimsStore } from '@app/stores/tims.store';
import { Entities } from '@app/entities/entities';
import Severity = Entities.Severity;
import RecurringInterval = Entities.RecurringInterval;
import RecurringIntervalRemoved = Entities.RecurringIntervalRemoved;

@Component({
  selector: 'app-schedule-editor',
  templateUrl: './schedule-editor.component.html',
  styleUrls: ['./schedule-editor.component.scss']
})
export class ScheduleEditorComponent implements OnInit {
  @ViewChild('startElement') startTimeEntryComponent: TimeEntryComponent;
  @ViewChild('endElement') endTimeEntryComponent: TimeEntryComponent;
  @ViewChild('recurranceEditor') recurranceEditor: RecurrenceEditorComponent;
  @ViewChild('duranceEditor') duranceEditor: DurationEntryComponent;
  public onClose: Subject<boolean>;
  public scheduleForm: FormGroup;
  public bsConfig?: Partial<BsDatepickerConfig>;
  public endTime: Date;
  public displayDeleteMessage: boolean = false;
  public displayDeleteInstanceMessage: boolean = false;
  public isNotesSelected: boolean = true;
  public errorMessage: string = null;
  public cpIsDisabled: boolean = false;
  public canSave: boolean = true;
  public deleteMessage: string;
  public showDeleteSingle: boolean = false;
  public providerDropdownSettings: IDropdownSettings = {};
  public siteDropdownSettings: IDropdownSettings = {};

  private _hasBeenValidated: boolean = false;
  private _duration: number;
  private scheduler: SchedulerStatic;
  private occurrenceDate: DateTime;
  private occurrenceNumber: number;
  private id: string;
  private _startTime: Date;
  private canAddSchedule: boolean;
  public canEditSchedule: boolean;
  private canDeleteSchedule: boolean;

  constructor(private formBuilder: FormBuilder,
    public timsStore: TimsStore,
    public bsModalRef: BsModalRef) {
  }

  public get ts() { return this.timsStore; }
  public get f() { return this.scheduleForm.controls; }

  public get backgroundColor(): string {
    return this.f['backgroundColor'].value;
  }

  public set backgroundColor(value: string) {
    this.f['backgroundColor'].setValue(value);
  }

  public get startTime(): Date {
    return this._startTime;
  }

  public set startTime(value: Date) {
    this._startTime = value;
    this.endTime = this.addMinutes(this.startTime, this._duration);
  }

  public get duration(): number {
    return this._duration;
  }

  public set duration(value: number) {
    this._duration = value;
    this.endTime = this.addMinutes(this.startTime, this._duration);
    this._hasBeenValidated = false;
  }
  public get LastUpdate() {
    if (this.ts.scheduleStore.selected_schedule == null)
      return '';
    return 'Last updated ' + DateTime.fromISO(this.ts.scheduleStore.selected_schedule.updatedDate).toLocaleString(DateTime.DATETIME_SHORT) + ' by ' + this.ts.scheduleStore.selected_schedule.updatedByUserName;
  }

  isNew() {
    return this.ts.scheduleStore.selected_schedule != null && this.ts.scheduleStore.selected_schedule.id <= 0;
  }

  isRecurring() {
    return this.ts.scheduleStore.selected_schedule != null && this.ts.scheduleStore.selected_schedule.recurringIntervalId > 0;
  }

  addMinutes(date, minutes) {
    let m = Number(minutes);
    let newDate = DateTime.fromJSDate(date).plus({ minutes: m })
    return newDate.toJSDate();
  }

  addDays(date, minutes) {
    let m = Number(minutes);
    let newDate = DateTime.fromJSDate(date).plus({ days: m })
    return newDate.toJSDate();
  }

  ngOnInit(): void {
    this.canAddSchedule = this.ts.userstore.doesUserHaveSetting(Entities.SettingEnum.canCreateNonPatientAppointments);
    this.canEditSchedule = this.ts.userstore.doesUserHaveSetting(Entities.SettingEnum.canModifyNonPatientAppointments);
    this.canDeleteSchedule = this.ts.userstore.doesUserHaveSetting(Entities.SettingEnum.canDeleteNonPatientAppointments);

    this.providerDropdownSettings = {
      singleSelection: false,
      idField: 'id',
      textField: 'simpleName',
      itemsShowLimit: 1,
      unSelectAllText: 'UnSelect All',
      allowSearchFilter: false
    };

    this.siteDropdownSettings = {
      singleSelection: false,
      idField: 'id',
      textField: 'name',
      itemsShowLimit: 1,
      unSelectAllText: 'UnSelect All',
      allowSearchFilter: false
    };

    this.scheduleForm = this.formBuilder.group({
      isnew: [true],
      providerid: [null],
      selectedProviders: [null],
      siteid: [null],
      selectedSites: [null],
      location: [""],
      subject: [null, Validators.required],
      date: [null, Validators.required],
      notes: [""],
      backgroundColor: '#ffff00',
      // Recurrance
      isrecurrence: [false],
      occurrences: [3],
      recurrenceType: [RecurrenceEditorComponent.RecurrenceOptions[0].id],
      dailySettings: new FormControl('singleDay'),
      endDateOption: new FormControl('endAfterOccurrences'),
      dailyValue: new FormControl('1'),
      endByDate: [null],
      // Weekly
      weeklyValue: new FormControl('1'),
      isWeeklyMonday: [false],
      isWeeklyTuesday: [false],
      isWeeklyWednesday: [false],
      isWeeklyThursday: [false],
      isWeeklyFriday: [false],
      isWeeklySaturday: [false],
      isWeeklySunday: [false],
      // Monthly
      monthlyValue: new FormControl('1'),
      monthlySettings: new FormControl('dayOfMonth'),
      dayOfMonthValue: new FormControl('1'),
      weekOfMonth: new FormControl('1'),
      dayOfWeekOfMonth: new FormControl('1'),
      // Yearly
      monthOfYear: new FormControl('1'),
      dayOfMonthYearValue: new FormControl('1')
    });
    this.scheduleForm.valueChanges.subscribe(f => {
      this._hasBeenValidated = false;
    });
    this.ts.scheduleStore.validation_results = null;
    this.bsConfig = Object.assign({}, { containerClass: 'theme-dark-blue', customTodayClass: 'tims-today-class', showWeekNumbers: false });
    if (this.ts.practiceStore.practiceSummary.locale == 'NZ' || this.ts.practiceStore.practiceSummary.locale == 'AU')
      this.bsConfig.dateInputFormat = 'DD/MM/YYYY';
    this.onClose = new Subject();
    this._startTime = new Date(this.ts.scheduleStore.selected_schedule.startsAt);
    if (this.isNew()) {
      this.scheduleForm.patchValue({ date: new Date(this.ts.scheduleStore.selected_schedule.startsAt) });
      this.endTime = this.addMinutes(this.ts.scheduleStore.selected_schedule.startsAt, 30);
      if (this.ts.scheduleStore.selected_schedule.providerId) {
        var provider = _.find(this.ts.providerStore.provider_summary_list, (x) => x.id == this.ts.scheduleStore.selected_schedule.providerId);
        this.scheduleForm.patchValue({ selectedProviders: [provider] });
      }
      if (this.ts.scheduleStore.selected_schedule.siteId) {
        var site = _.find(this.ts.siteStore.site_list, (x) => x.id == this.ts.scheduleStore.selected_schedule.siteId);
        this.scheduleForm.patchValue({ selectedSites: [site] });
      }
      this.duration = 30;
    }
    else {
      this.scheduleForm.patchValue({ isnew: false });
      this.endTime = new Date(this.ts.scheduleStore.selected_schedule.endsAt);
      this.duration = Math.round((this.endTime.getTime() - this.startTime.getTime()) / 60000);
      // In case of inactive site
      let site = _.find(this.ts.siteStore.site_list, (x) => { return x.id == this.ts.scheduleStore.selected_schedule.siteId });
      if (!site) {
        this.ts.siteStore.getSummary(this.ts.scheduleStore.selected_schedule.siteId);
        when(() => !this.ts.siteStore.inprogress,
          () => {
            this.scheduleForm.patchValue({ siteid: this.ts.scheduleStore.selected_schedule.siteId });
          });
      }
      else {
        this.scheduleForm.patchValue({ siteid: this.ts.scheduleStore.selected_schedule.siteId });
      }
      // In case of inactive provider
      let provider = _.find(this.ts.providerStore.provider_list_w_blank, (x) => { return x.id == this.ts.scheduleStore.selected_schedule.providerId });
      if (!provider) {
        this.ts.providerStore.getSummary(this.ts.scheduleStore.selected_schedule.providerId);
        when(() => !this.ts.providerStore.inprogress,
          () => {
            this.scheduleForm.patchValue({ providerid: this.ts.scheduleStore.selected_schedule.providerId });
          });
      }
      else
        this.scheduleForm.patchValue({ providerid: this.ts.scheduleStore.selected_schedule.providerId });
      this.scheduleForm.patchValue({ subject: this.ts.scheduleStore.selected_schedule.title });
      this.scheduleForm.patchValue({ location: this.ts.scheduleStore.selected_schedule.location });
      this.scheduleForm.patchValue({ notes: this.ts.scheduleStore.selected_schedule.notes });
      if (this.ts.scheduleStore.selected_schedule.web_Color) {
        this.scheduleForm.patchValue({ backgroundColor: this.ts.scheduleStore.selected_schedule.web_Color });
      }
      this.scheduleForm.patchValue({ date: new Date(this.ts.scheduleStore.selected_schedule.startsAt) });
      if (this.ts.scheduleStore.selected_schedule.recurringInterval != null) {
        this.scheduleForm.patchValue({ date: new Date(this.occurrenceDate) });
        this.canSave = false;
        this.scheduleForm.patchValue({ isrecurrence: true });
        this.scheduleForm.patchValue({ recurrenceType: this.ts.scheduleStore.selected_schedule.recurringInterval.intervalType });
        if (this.ts.scheduleStore.selected_schedule.recurringInterval.intervalType == 1) {
          switch (this.ts.scheduleStore.selected_schedule.recurringInterval.subType) {
            case 1:
              this.scheduleForm.patchValue({ dailyValue: this.ts.scheduleStore.selected_schedule.recurringInterval.dayInterval });
              this.scheduleForm.patchValue({ dailySettings: 'singleDay' });
              break
            case 2:
              this.scheduleForm.patchValue({ dailySettings: 'weekdays' });
              break
            case 3:
              this.scheduleForm.patchValue({ dailySettings: 'monwedfri' });
              break
            case 4:
              this.scheduleForm.patchValue({ dailySettings: 'tuesthurs' });
              break
          }
        }
        else if (this.ts.scheduleStore.selected_schedule.recurringInterval.intervalType == 2) {
          this.scheduleForm.patchValue({ weeklyValue: this.ts.scheduleStore.selected_schedule.recurringInterval.weekInterval });
          this.scheduleForm.patchValue({ isWeeklyMonday: this.ts.scheduleStore.selected_schedule.recurringInterval.isMondaySet });
          this.scheduleForm.patchValue({ isWeeklyTuesday: this.ts.scheduleStore.selected_schedule.recurringInterval.isTuesdaySet });
          this.scheduleForm.patchValue({ isWeeklyWednesday: this.ts.scheduleStore.selected_schedule.recurringInterval.isWednesdaySet });
          this.scheduleForm.patchValue({ isWeeklyThursday: this.ts.scheduleStore.selected_schedule.recurringInterval.isThursdaySet });
          this.scheduleForm.patchValue({ isWeeklyFriday: this.ts.scheduleStore.selected_schedule.recurringInterval.isFridaySet });
          this.scheduleForm.patchValue({ isWeeklySaturday: this.ts.scheduleStore.selected_schedule.recurringInterval.isSaturdaySet });
          this.scheduleForm.patchValue({ isWeeklySunday: this.ts.scheduleStore.selected_schedule.recurringInterval.isSundaySet });
        }
        else if (this.ts.scheduleStore.selected_schedule.recurringInterval.intervalType == 3) {
          this.scheduleForm.patchValue({ monthlyValue: this.ts.scheduleStore.selected_schedule.recurringInterval.monthInterval });
          if (this.ts.scheduleStore.selected_schedule.recurringInterval.subType == 1) {
            this.scheduleForm.patchValue({ dayOfMonth: this.ts.scheduleStore.selected_schedule.recurringInterval.dayOfMonth });
          }
          else {
            this.scheduleForm.patchValue({ weekOfMonth: this.ts.scheduleStore.selected_schedule.recurringInterval.dayQualifier });
            this.scheduleForm.patchValue({ dayOfWeekOfMonth: this.ts.scheduleStore.selected_schedule.recurringInterval.dayOfWeek });
          }
        }
        else if (this.ts.scheduleStore.selected_schedule.recurringInterval.intervalType == 4) {
          this.scheduleForm.patchValue({ monthOfYear: this.ts.scheduleStore.selected_schedule.recurringInterval.month });
          this.scheduleForm.patchValue({ dayOfMonthYearValue: this.ts.scheduleStore.selected_schedule.recurringInterval.dayOfMonth });
        }
        switch (this.ts.scheduleStore.selected_schedule.recurringInterval.endType) {
          case 1:
            this.scheduleForm.patchValue({ endDateOption: 'noEndDate' });
            break;
          case 2:
            this.scheduleForm.patchValue({ occurrences: this.ts.scheduleStore.selected_schedule.recurringInterval.endOccurs });
            this.scheduleForm.patchValue({ endDateOption: 'endAfterOccurrences' });
            break;
          case 3:
            this.scheduleForm.patchValue({ endByDate: new Date(this.ts.scheduleStore.selected_schedule.recurringInterval.endDate) });
            this.scheduleForm.patchValue({ endDateOption: 'endBy' });
            break;
        }
      }
    }
    this.scheduleForm.valueChanges.subscribe(data => {
      this._hasBeenValidated = false;
      this.displayDeleteMessage = false;
      this.ts.scheduleStore.validation_results = null;
      this.errorMessage = null;
      this.showDeleteSingle = false;
    });
  }

  disable() {
    this.f['providerid'].disable();
    this.f['siteid'].disable();
    this.f['date'].disable();
    var x = this.f['backgroundColor'];
    this.cpIsDisabled = true;
    this.startTimeEntryComponent.disable();
    this.duranceEditor.disable();
  }

  handlesNotesClick() {
    this.isNotesSelected = true;
  }

  handlesRecurrenceClick() {
    this.isNotesSelected = false;
  }

  ngAfterViewInit(): void {
    this.endTimeEntryComponent.disable();
    if (this.ts.scheduleStore.selected_schedule.recurringInterval != null) {
      this.disable();
    }
  }

  hasValidationMessage() {
    return this.ts.scheduleStore.validation_results != null && this.ts.scheduleStore.validation_results.length > 0;
  }

  hasErrorMessage() {
    return !this.errorMessage;
  }

  showDelete() {
    if (!this.canDeleteSchedule) return false;
    return this.ts.scheduleStore.selected_schedule != null && this.ts.scheduleStore.selected_schedule.id > 0 && !this._hasBeenValidated;
  }

  showSave() {
    if (this.isNew() && !this.canAddSchedule) return false;
    if (!this.canEditSchedule) return false;
    return !this.displayDeleteMessage;
  }

  validateForm() {
    _.each(this.f, (x) => { x.markAsTouched() });
  }

  onSubmit() {
    this.errorMessage = null;
    this.validateForm();

    if (this.scheduleForm.invalid) {
      return;
    }

    const result = Object.assign({}, this.scheduleForm.getRawValue());

    if (result.isrecurrence) {
      if (result.endDateOption == 'endBy' && !result.endByDate) {
        this.errorMessage = 'End by date required';
        return;
      }

      if (result.recurrenceType == 2) {
        if (!result.isWeeklyMonday &&
          !result.isWeeklyTuesday &&
          !result.isWeeklyWednesday &&
          !result.isWeeklyThursday &&
          !result.isWeeklyFriday &&
          !result.isWeeklySaturday &&
          !result.isWeeklySunday) {
          this.errorMessage = 'At least 1 day must be set for weekly recurrence';
          return;
        }
      }
    }

    let startsAt = DateUtils.createDateTime(result.date, this.startTime);
    if (this.isNew() || !result.isrecurrence) {
      this.ts.scheduleStore.selected_schedule.siteId = Number(result.siteid);
      this.ts.scheduleStore.selected_schedule.providerId = Number(result.providerid);
      this.ts.scheduleStore.selected_schedule.web_Color = result.backgroundColor;
      this.ts.scheduleStore.selected_schedule.startsAt = startsAt.toFormat(DateUtils.Constants.API_DATE_FORMAT);
      this.ts.scheduleStore.selected_schedule.endsAt = DateUtils.createApiDateTime(result.date, this.endTime);
    }

    this.ts.scheduleStore.selected_schedule.notes = result.notes;
    this.ts.scheduleStore.selected_schedule.location = result.location;
    this.ts.scheduleStore.selected_schedule.title = result.subject;

    this.ts.scheduleStore.selected_schedule.providerIds = _.map(result.selectedProviders, (p) => p.id);
    this.ts.scheduleStore.selected_schedule.siteIds = _.map(result.selectedSites, (p) => p.id);
    this.ts.scheduleStore.validate(this.ts.scheduleStore.selected_schedule);
    when(() => this.ts.scheduleStore.inprogress == false,
      () => {
        let validationResults = this.ts.scheduleStore.validation_results;
        if (validationResults.length > 0) {
          let errors = _.filter(validationResults, v => v.severity == Severity.error);
          if (errors.length > 0) {
            this._hasBeenValidated = true;
            return;
          }
          if (!this._hasBeenValidated) {
            this._hasBeenValidated = true;
            return;
          }
        }

        if (this.isNew()) {
          if (result.isrecurrence) {
            this.ts.scheduleStore.selected_schedule.recurringInterval = new RecurringInterval();
            this.ts.scheduleStore.selected_schedule.recurringInterval.startDate = this.ts.scheduleStore.selected_schedule.startsAt;
            if (result.recurrenceType == 1) {
              // Default
              this.ts.scheduleStore.selected_schedule.recurringInterval.dayInterval = 1
              // Daily
              this.ts.scheduleStore.selected_schedule.recurringInterval.intervalType = 1;
              this.ts.scheduleStore.selected_schedule.recurringInterval.weekInterval = 1;
              if (result.dailySettings == 'weekdays') {
                this.ts.scheduleStore.selected_schedule.recurringInterval.subType = 2;
              }
              else if (result.dailySettings == 'monwedfri') {
                this.ts.scheduleStore.selected_schedule.recurringInterval.subType = 3;
              }
              else if (result.dailySettings == 'tuesthurs') {
                this.ts.scheduleStore.selected_schedule.recurringInterval.subType = 4;
              }
              else {
                this.ts.scheduleStore.selected_schedule.recurringInterval.subType = 1;
                // Number of days
                let numberOfDays = Number(result.dailyValue);
                this.ts.scheduleStore.selected_schedule.recurringInterval.dayInterval = numberOfDays;
              }
            }
            else if (result.recurrenceType == 2) {
              // Default
              this.ts.scheduleStore.selected_schedule.recurringInterval.dayInterval = 1
              // Weekly
              this.ts.scheduleStore.selected_schedule.recurringInterval.intervalType = 2;
              this.ts.scheduleStore.selected_schedule.recurringInterval.weekInterval = Number(result.weeklyValue);
              this.ts.scheduleStore.selected_schedule.recurringInterval.isMondaySet = result.isWeeklyMonday;
              this.ts.scheduleStore.selected_schedule.recurringInterval.isTuesdaySet = result.isWeeklyTuesday;
              this.ts.scheduleStore.selected_schedule.recurringInterval.isWednesdaySet = result.isWeeklyWednesday;
              this.ts.scheduleStore.selected_schedule.recurringInterval.isThursdaySet = result.isWeeklyThursday;
              this.ts.scheduleStore.selected_schedule.recurringInterval.isFridaySet = result.isWeeklyFriday;
              this.ts.scheduleStore.selected_schedule.recurringInterval.isSaturdaySet = result.isWeeklySaturday;
              this.ts.scheduleStore.selected_schedule.recurringInterval.isSundaySet = result.isWeeklySunday;
            }
            else if (result.recurrenceType == 3) {
              // Default
              this.ts.scheduleStore.selected_schedule.recurringInterval.dayInterval = 1
              // Monthly
              this.ts.scheduleStore.selected_schedule.recurringInterval.intervalType = 3;
              this.ts.scheduleStore.selected_schedule.recurringInterval.monthInterval = Number(result.monthlyValue);
              if (result.monthlySettings == 'dayOfMonth') {
                this.ts.scheduleStore.selected_schedule.recurringInterval.subType = 1;
                this.ts.scheduleStore.selected_schedule.recurringInterval.dayOfMonth = Number(result.dayOfMonthValue);
              }
              else {
                this.ts.scheduleStore.selected_schedule.recurringInterval.subType = 2;
                this.ts.scheduleStore.selected_schedule.recurringInterval.dayQualifier = Number(result.weekOfMonth);
                this.ts.scheduleStore.selected_schedule.recurringInterval.dayOfWeek = Number(result.dayOfWeekOfMonth);
              }
            }
            else {
              // Yearly
              this.ts.scheduleStore.selected_schedule.recurringInterval.intervalType = 4;
              this.ts.scheduleStore.selected_schedule.recurringInterval.month = result.monthOfYear;
              this.ts.scheduleStore.selected_schedule.recurringInterval.dayOfMonth = result.dayOfMonthYearValue;
            }

            // Set End
            if (result.endDateOption == 'endAfterOccurrences') {
              let numberOfOccurrences = Number(result.occurrences);
              this.ts.scheduleStore.selected_schedule.recurringInterval.endType = 2;
              this.ts.scheduleStore.selected_schedule.recurringInterval.endOccurs = numberOfOccurrences;
              let endDate = startsAt.plus({ days: numberOfOccurrences });
              this.ts.scheduleStore.selected_schedule.recurringInterval.endDate = endDate.toFormat(DateUtils.Constants.API_DATE_FORMAT);
            }
            else if (result.endDateOption == 'endBy') {
              this.ts.scheduleStore.selected_schedule.recurringInterval.endType = 3;
              this.ts.scheduleStore.selected_schedule.recurringInterval.endDate = DateTime.fromJSDate(result.endByDate).toFormat(DateUtils.Constants.API_DATE_FORMAT);
              this.ts.scheduleStore.selected_schedule.recurringInterval.endOccurs = 3;
            }
            else {
              this.ts.scheduleStore.selected_schedule.recurringInterval.endType = 1;
            }
          }

          this.ts.scheduleStore.create(this.ts.scheduleStore.selected_schedule);
        }
        else {
          this.ts.scheduleStore.update(this.ts.scheduleStore.selected_schedule);
        }

        this.onClose.next(true);
        this.bsModalRef.hide();
      });
  }

  onDelete() {
    if (this.isRecurring()) {
      if (!this.displayDeleteMessage) {
        this.deleteMessage = "Delete single non-patient appointment or the series?";
        this.showDeleteSingle = true;
        this.displayDeleteMessage = true;
        return;
      }
      this.scheduler.deleteEvent('X-' + this.ts.scheduleStore.selected_schedule.id);
      this.ts.scheduleStore.delete(this.ts.scheduleStore.selected_schedule.id);
    }
    else {
      if (!this.displayDeleteMessage) {
        this.deleteMessage = "Delete non-patient appointment?";
        this.displayDeleteMessage = true;
        return;
      }
      this.scheduler.deleteEvent('X-' + this.ts.scheduleStore.selected_schedule.id);
      this.ts.scheduleStore.delete(this.ts.scheduleStore.selected_schedule.id);
    }
    this.onClose.next(true);
    this.bsModalRef.hide();
  }

  onDeleteOccurrence() {
    let rrRemoved = new RecurringIntervalRemoved();
    rrRemoved.scheduleId = this.ts.scheduleStore.selected_schedule.id;
    rrRemoved.itemNumber = this.occurrenceNumber;
    rrRemoved.recurringIntervalId = this.ts.scheduleStore.selected_schedule.recurringInterval.id;
    rrRemoved.itemDate = DateTime.fromJSDate(this.occurrenceDate).toFormat(DateUtils.Constants.API_DATE_FORMAT);
    this.ts.scheduleStore.deleteOccurrence(rrRemoved);
    this.scheduler.deleteEvent(this.id);
    this.onClose.next(true);
    this.bsModalRef.hide();
  }

  onCancel() {
    this.onClose.next(false);
    this.bsModalRef.hide();
  }

}
