import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators, ValidationErrors } from '@angular/forms';
import { Subject } from 'rxjs';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { SchedulerStatic } from "@app/dhtmlx/dhtmlxscheduler";
import { DateTime } from "luxon";
import * as _ from 'underscore';
import { when } from "mobx";

import { DateUtils } from '@app/helpers/date-utils';
import { TimeEntryComponent } from '@app/components/time-entry/time-entry.component';
import { RecurrenceEditorComponent } from '@app/components/recurrence-editor/recurrence-editor.component';
import { TimsStore } from '@app/stores/tims.store';

import { Entities } from '@app/entities/entities';
import Severity = Entities.Severity;

enum apptFormState {
  clean,
  validatingSave,
  validatingDelete,
  validatingEndSeries
}

@Component({
  selector: 'app-appointment-editor',
  templateUrl: './appointment-editor.component.html',
  styleUrls: ['./appointment-editor.component.scss']
})
export class AppointmentEditorComponent implements OnInit, AfterViewInit {
  @ViewChild('startElement') startTimeEntryComponent: TimeEntryComponent;
  @ViewChild('endElement') endTimeEntryComponent: TimeEntryComponent;
  @ViewChild('recurranceEditor') recurranceEditor: RecurrenceEditorComponent;
  public onClose: Subject<boolean>;
  public appointmentForm: FormGroup;
  public bsConfig?: Partial<BsDatepickerConfig>;
  public endTime: Date;
  public patientName: string = '';
  public displayPhone: string = '';
  public dob: string = '';
  public displayDeleteMessage: boolean = false;
  public displayEndSeriesMessage: boolean = false;
  public isNotesSelected: boolean = true;
  public otStatus: string = '';
  public otStatusDescription: string = '';
  public isOpportunity: boolean = false;
  public usesResources: boolean;
  public resourceList: Entities.Resource[] = [];

  private _hasBeenValidated: boolean = false;
  private scheduler: SchedulerStatic;
  private _startTime: Date;
  private _duration: number;
  public marketingRequired: ValidationErrors = null;
  public appointmentTypeRequired: ValidationErrors = null;
  private canAddAppointment: boolean;
  public canEditAppointment: boolean;
  private canDeleteAppointment: boolean;
  private apptFormState: apptFormState = apptFormState.clean;

  constructor(private formBuilder: FormBuilder,
    public ts: TimsStore,
    public bsModalRef: BsModalRef) {
    this.usesResources = this.ts.practiceStore.businessRules.usesCalendarResources;
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

  public get f() { return this.appointmentForm.controls; }

  public get LastUpdate() {
    if (this.ts.appointmentStore.selected_appointment == null)
      return '';
    return 'Last updated ' + DateTime.fromISO(this.ts.appointmentStore.selected_appointment.updatedDate).toLocaleString(DateTime.DATETIME_SHORT) + ' by ' + this.ts.appointmentStore.selected_appointment.updatedByUserName;
  }

  isNew() {
    return this.ts.appointmentStore.selected_appointment != null && this.ts.appointmentStore.selected_appointment.id <= 0;
  }

  showDelete() {
    if (!this.canDeleteAppointment) return false;
    if (this.ts.appointmentStore.selected_appointment == null || this.ts.appointmentStore.selected_appointment.id <= 0) return false;
    if (this.apptFormState == apptFormState.validatingEndSeries) return false;
    return this.apptFormState == apptFormState.clean || this.apptFormState == apptFormState.validatingDelete;
  }

  showEndSeries() {
    if (this.isNew() && !this.canAddAppointment) return false;
    if (!this.canEditAppointment) return false;
    if (this.apptFormState == apptFormState.validatingEndSeries) return true;
    if (this.apptFormState == apptFormState.validatingDelete || this.apptFormState == apptFormState.validatingSave) return false;
    return this.ts.appointmentStore.selected_appointment != null &&
      this.ts.appointmentStore.selected_appointment.id > 0 &&
      this.ts.appointmentStore.selected_appointment.recurringInterval != null;
  }

  showSave() {
    if (this.isNew() && !this.canAddAppointment) return false;
    if (!this.canEditAppointment) return false;
    return !this.displayDeleteMessage && !this.displayEndSeriesMessage;
  }

  ngOnInit(): void {
    this.canAddAppointment = this.ts.userstore.doesUserHaveSetting(Entities.SettingEnum.canCreateAppointments);
    this.canEditAppointment = this.ts.userstore.doesUserHaveSetting(Entities.SettingEnum.canModifyAppointments);
    this.canDeleteAppointment = this.ts.userstore.doesUserHaveSetting(Entities.SettingEnum.canDeleteAppointments);

    this.ts.lookupStore.marketingReference_list = null;
    this.marketingRequired = this.ts.practiceStore.businessRules.requireMarketingSourceForPatientAppointment ? Validators.required : null;
    this.appointmentTypeRequired = this.ts.practiceStore.businessRules.requireAppointmentTypeForAppointment ? Validators.required : null;

    this.appointmentForm = this.formBuilder.group({
      isnew: [true],
      providerId: [null, Validators.required],
      appointmentTypeId: [null, this.appointmentTypeRequired],
      siteId: [null, Validators.required],
      resourceId: [null],
      statusId: [null, Validators.required],
      marketingReferenceId: [null, this.marketingRequired],
      authorizationId: [0],
      date: [null, Validators.required],
      hours: [null],
      minutes: [null],
      nextContactDate: [null],
      notes: [""],
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
    this.appointmentForm.valueChanges.subscribe(f => {
      this._hasBeenValidated = false;
    });
    this.ts.appointmentStore.validation_results = null;
    this.bsConfig = Object.assign({}, {
      containerClass: 'theme-dark-blue',
      customTodayClass: 'tims-today-class',
      showWeekNumbers: false
    });
    if (this.ts.practiceStore.practiceSummary.locale == 'NZ' || this.ts.practiceStore.practiceSummary.locale == 'AU')
      this.bsConfig.dateInputFormat = 'DD/MM/YYYY';
    this.onClose = new Subject();

    this.appointmentForm.patchValue({ date: new Date(this.ts.appointmentStore.selected_appointment.startsAt) });
    this._startTime = new Date(this.ts.appointmentStore.selected_appointment.startsAt);
    this.duration = 30;

    let isSiteSet = false;
    if (this.ts.appointmentStore.selected_appointment.providerId) {
      // In case of inactive provider
      let provider = _.find(this.ts.providerStore.provider_list_w_blank, (x) => { return x.id == this.ts.appointmentStore.selected_appointment.providerId });
      if (!provider) {
        this.ts.providerStore.getSummary(this.ts.appointmentStore.selected_appointment.providerId);
        when(() => !this.ts.providerStore.inprogress,
          () => {
            this.appointmentForm.patchValue({ providerId: this.ts.appointmentStore.selected_appointment.providerId });
          });
      }
      else
        this.appointmentForm.patchValue({ providerId: this.ts.appointmentStore.selected_appointment.providerId });
    }
    if (this.ts.appointmentStore.selected_appointment.siteId) {
      isSiteSet = true;
      // In case of inactive site
      let site = _.find(this.ts.siteStore.site_list, (x) => { return x.id == this.ts.appointmentStore.selected_appointment.siteId });
      if (!site) {
        this.ts.siteStore.getSummary(this.ts.appointmentStore.selected_appointment.siteId);
        when(() => !this.ts.siteStore.inprogress,
          () => {
            this.appointmentForm.patchValue({ siteId: this.ts.appointmentStore.selected_appointment.siteId });
            this.onSiteChanged();
          });
      }
      else
        this.appointmentForm.patchValue({ siteId: this.ts.appointmentStore.selected_appointment.siteId });
      this.onSiteChanged();
    }

    if (this.ts.practiceStore.businessRules.useApptAuthorizations) {
      this.ts.apptAuthorizationStore.getForPatient(this.ts.appointmentStore.selected_appointment.patientId, false, this.ts.appointmentStore.selected_appointment.authorizationId);
      when(() => this.ts.apptAuthorizationStore.apptAuthorization_list != null,
        () => {
          if (!this.isNew() && this.ts.appointmentStore.selected_appointment.authorizationId != null &&
            this.ts.appointmentStore.selected_appointment.authorizationId > 0) {
            this.appointmentForm.patchValue({ authorizationId: this.ts.appointmentStore.selected_appointment.authorizationId });
          }
        });
    }

    this.ts.patientStore.getAppointmentPatient(this.ts.appointmentStore.selected_appointment.patientId);
    when(() => this.ts.patientStore.appointment_patient != null,
      () => {
        this.patientName = this.ts.patientStore.appointment_patient.firstName + ' ' + this.ts.patientStore.appointment_patient.lastName;
        this.displayPhone = this.ts.patientStore.appointment_patient.phoneToDisplay;
        if (this.ts.patientStore.appointment_patient.birthDate) {
          this.dob = DateTime.fromISO(this.ts.patientStore.appointment_patient.birthDate).toFormat('D');
        }
        if (this.isNew()) {
          this.ts.appointmentStore.selected_appointment.otStatus = this.ts.patientStore.appointment_patient.otStatusId;
          this.otStatus = this.ts.patientStore.appointment_patient.opportunity;
          this.otStatusDescription = this.ts.patientStore.appointment_patient.opportunityDescription;
          this.setOtStatus();
          if (!isSiteSet) {
            this.appointmentForm.patchValue({ siteId: this.ts.patientStore.appointment_patient.siteId });
            this.onSiteChanged();
          }
        }
      });

    if (!this.isNew()) {
      this.setOtStatus();
      this.otStatus = this.ts.appointmentStore.selected_appointment.opportunity;
      this.otStatusDescription = this.ts.appointmentStore.selected_appointment.opportunityDescription;
      this.resourceList = this.ts.resourceStore.getResourcesForSite(this.ts.appointmentStore.selected_appointment.siteId);

      this.appointmentForm.patchValue({ isnew: false });
      this.endTime = new Date(this.ts.appointmentStore.selected_appointment.endsAt);
      this.duration = Math.round((this.endTime.getTime() - this.startTime.getTime()) / 60000);
      this.appointmentForm.patchValue({ appointmentTypeId: this.ts.appointmentStore.selected_appointment.appointmentTypeId });
      this.appointmentForm.patchValue({ statusId: this.ts.appointmentStore.selected_appointment.appointmentStatusId });
      this.appointmentForm.patchValue({ notes: this.ts.appointmentStore.selected_appointment.notes });
      if (this.ts.appointmentStore.selected_appointment.marketingId > 0)
        this.appointmentForm.patchValue({ marketingReferenceId: this.ts.appointmentStore.selected_appointment.marketingId });
      if (this.ts.appointmentStore.selected_appointment.resourceId > 0)
        this.appointmentForm.patchValue({ resourceId: this.ts.appointmentStore.selected_appointment.resourceId });
      if (this.ts.appointmentStore.selected_appointment.recurringInterval != null) {
        this.appointmentForm.patchValue({ isrecurrence: true });
        this.appointmentForm.patchValue({ recurrenceType: this.ts.appointmentStore.selected_appointment.recurringInterval.intervalType });
        if (this.ts.appointmentStore.selected_appointment.recurringInterval.intervalType == 1) {
          switch (this.ts.appointmentStore.selected_appointment.recurringInterval.subType) {
            case 1:
              this.appointmentForm.patchValue({ dailyValue: this.ts.appointmentStore.selected_appointment.recurringInterval.dayInterval });
              this.appointmentForm.patchValue({ dailySettings: 'singleDay' });
              break
            case 2:
              this.appointmentForm.patchValue({ dailySettings: 'weekdays' });
              break
            case 3:
              this.appointmentForm.patchValue({ dailySettings: 'monwedfri' });
              break
            case 4:
              this.appointmentForm.patchValue({ dailySettings: 'tuesthurs' });
              break
          }
        }
        else if (this.ts.appointmentStore.selected_appointment.recurringInterval.intervalType == 2) {
          this.appointmentForm.patchValue({ weeklyValue: this.ts.appointmentStore.selected_appointment.recurringInterval.weekInterval });
          this.appointmentForm.patchValue({ isWeeklyMonday: this.ts.appointmentStore.selected_appointment.recurringInterval.isMondaySet });
          this.appointmentForm.patchValue({ isWeeklyTuesday: this.ts.appointmentStore.selected_appointment.recurringInterval.isTuesdaySet });
          this.appointmentForm.patchValue({ isWeeklyWednesday: this.ts.appointmentStore.selected_appointment.recurringInterval.isWednesdaySet });
          this.appointmentForm.patchValue({ isWeeklyThursday: this.ts.appointmentStore.selected_appointment.recurringInterval.isThursdaySet });
          this.appointmentForm.patchValue({ isWeeklyFriday: this.ts.appointmentStore.selected_appointment.recurringInterval.isFridaySet });
          this.appointmentForm.patchValue({ isWeeklySaturday: this.ts.appointmentStore.selected_appointment.recurringInterval.isSaturdaySet });
          this.appointmentForm.patchValue({ isWeeklySunday: this.ts.appointmentStore.selected_appointment.recurringInterval.isSundaySet });
        }
        else if (this.ts.appointmentStore.selected_appointment.recurringInterval.intervalType == 3) {
          this.appointmentForm.patchValue({ monthlyValue: this.ts.appointmentStore.selected_appointment.recurringInterval.monthInterval });
          if (this.ts.appointmentStore.selected_appointment.recurringInterval.subType == 1) {
            this.appointmentForm.patchValue({ dayOfMonth: this.ts.appointmentStore.selected_appointment.recurringInterval.dayOfMonth });
          }
          else {
            this.appointmentForm.patchValue({ weekOfMonth: this.ts.appointmentStore.selected_appointment.recurringInterval.dayQualifier });
            this.appointmentForm.patchValue({ dayOfWeekOfMonth: this.ts.appointmentStore.selected_appointment.recurringInterval.dayOfWeek });
          }
        }
        else if (this.ts.appointmentStore.selected_appointment.recurringInterval.intervalType == 4) {
          this.appointmentForm.patchValue({ monthOfYear: this.ts.appointmentStore.selected_appointment.recurringInterval.month });
          this.appointmentForm.patchValue({ dayOfMonthYearValue: this.ts.appointmentStore.selected_appointment.recurringInterval.dayOfMonth });
        }
        switch (this.ts.appointmentStore.selected_appointment.recurringInterval.endType) {
          case 1:
            this.appointmentForm.patchValue({ endDateOption: 'noEndDate' });
            break;
          case 2:
            this.appointmentForm.patchValue({ occurrences: this.ts.appointmentStore.selected_appointment.recurringInterval.endOccurs });
            this.appointmentForm.patchValue({ endDateOption: 'endAfterOccurrences' });
            break;
          case 3:
            this.appointmentForm.patchValue({ endByDate: new Date(this.ts.appointmentStore.selected_appointment.recurringInterval.endDate) });
            this.appointmentForm.patchValue({ endDateOption: 'endBy' });
            break;
        }
      }
    }
    else {
      let s = _.find(this.ts.lookupStore.appointmentStatus_list, (x) => { return x.name.toLocaleLowerCase() == 'new' });
      if (s) {
        this.appointmentForm.patchValue({ statusId: s.id });
      }
    }

    this.appointmentForm.valueChanges.subscribe(data => {
      this._hasBeenValidated = false;
      this.displayDeleteMessage = false;
      this.displayEndSeriesMessage = false;
      this.ts.appointmentStore.validation_results = null;
      this.ts.appointmentStore.delete_validation_results = null;
    });
  }

  setOtStatus() {
    switch (this.ts.appointmentStore.selected_appointment.otStatus) {
      case Entities.OpportunityStatusEnum.NoMarketing:
      case Entities.OpportunityStatusEnum.NoMarketingOneEar:
      case Entities.OpportunityStatusEnum.NoOpportunity:
      case Entities.OpportunityStatusEnum.CurrentUser:
      case Entities.OpportunityStatusEnum.NotSet:
        this.isOpportunity = false;
        return
    }
    this.isOpportunity = true;
  }

  ngAfterViewInit(): void {
    this.endTimeEntryComponent.disable();
  }

  addMinutes(date, minutes) {
    let m = Number(minutes);
    let newDate = DateTime.fromJSDate(date).plus({ minutes: m })
    return newDate.toJSDate();
  }

  onAppointmentTypeChanged() {
    if (this.appointmentForm.value.appointmentTypeId) {
      let at_id = this.appointmentForm.value.appointmentTypeId;
      let at = _.find(this.ts.lookupStore.appointmentType_list, (x) => { return x.id == at_id; });
      if (at) {
        this.duration = at.duration;
      }
    }
  }

  onSiteChanged() {
    let siteId = this.appointmentForm.value.siteId;
    this.resourceList = this.ts.resourceStore.getResourcesForSite(siteId);
  }

  handlesNotesClick() {
    this.isNotesSelected = true;
  }

  handlesRecurrenceClick() {
    this.isNotesSelected = false;
  }

  validateForm() {
    _.each(this.f, (x) => { x.markAsTouched() });
    this.startTimeEntryComponent.markAsTouched();
  }

  onSubmit() {
    this.apptFormState = apptFormState.validatingSave;
    this.validateForm();
    if (this.appointmentForm.invalid) {
      return;
    }
    const result = Object.assign({}, this.appointmentForm.value);

    if (this.isNew()) {
      this.ts.appointmentStore.selected_appointment.patientId = this.ts.patientStore.new_appointment_patientId;
      this.ts.appointmentStore.selected_appointment.otStatusDescriptionId = this.ts.patientStore.appointment_patient.otStatusDescriptionId;
    }

    this.ts.appointmentStore.selected_appointment.marketingId = result.marketingReferenceId;
    this.ts.appointmentStore.selected_appointment.appointmentTypeId = result.appointmentTypeId;
    this.ts.appointmentStore.selected_appointment.providerId = result.providerId;
    this.ts.appointmentStore.selected_appointment.appointmentStatusId = result.statusId;
    this.ts.appointmentStore.selected_appointment.siteId = result.siteId;
    this.ts.appointmentStore.selected_appointment.resourceId = result.resourceId;
    this.ts.appointmentStore.selected_appointment.nextContactDate = result.nextContactDate;
    this.ts.appointmentStore.selected_appointment.notes = result.notes;
    this.ts.appointmentStore.selected_appointment.authorizationId = result.authorizationId

    this.ts.appointmentStore.selected_appointment.startsAt = DateUtils.createApiDateTime(result.date, this.startTime);
    this.ts.appointmentStore.selected_appointment.endsAt = DateUtils.createApiDateTime(result.date, this.endTime);

    if (this.isNew() && result.isrecurrence) {
      this.ts.appointmentStore.selected_appointment.recurringInterval = new Entities.ApptRecurringInterval();
      this.ts.appointmentStore.selected_appointment.recurringInterval.startDate = this.ts.appointmentStore.selected_appointment.startsAt;
      if (result.recurrenceType == 1) {
        // Default
        this.ts.appointmentStore.selected_appointment.recurringInterval.dayInterval = 1
        // Daily
        this.ts.appointmentStore.selected_appointment.recurringInterval.intervalType = 1;
        this.ts.appointmentStore.selected_appointment.recurringInterval.weekInterval = 1;
        if (result.dailySettings == 'weekdays') {
          this.ts.appointmentStore.selected_appointment.recurringInterval.subType = 2;
        }
        else if (result.dailySettings == 'monwedfri') {
          this.ts.appointmentStore.selected_appointment.recurringInterval.subType = 3;
        }
        else if (result.dailySettings == 'tuesthurs') {
          this.ts.appointmentStore.selected_appointment.recurringInterval.subType = 4;
        }
        else {
          this.ts.appointmentStore.selected_appointment.recurringInterval.subType = 1;
          // Number of days
          let numberOfDays = Number(result.dailyValue);
          this.ts.appointmentStore.selected_appointment.recurringInterval.dayInterval = numberOfDays;
        }
      }
      else if (result.recurrenceType == 2) {
        // Default
        this.ts.appointmentStore.selected_appointment.recurringInterval.dayInterval = 1
        // Weekly
        this.ts.appointmentStore.selected_appointment.recurringInterval.intervalType = 2;
        this.ts.appointmentStore.selected_appointment.recurringInterval.weekInterval = Number(result.weeklyValue);
        this.ts.appointmentStore.selected_appointment.recurringInterval.isMondaySet = result.isWeeklyMonday;
        this.ts.appointmentStore.selected_appointment.recurringInterval.isTuesdaySet = result.isWeeklyTuesday;
        this.ts.appointmentStore.selected_appointment.recurringInterval.isWednesdaySet = result.isWeeklyWednesday;
        this.ts.appointmentStore.selected_appointment.recurringInterval.isThursdaySet = result.isWeeklyThursday;
        this.ts.appointmentStore.selected_appointment.recurringInterval.isFridaySet = result.isWeeklyFriday;
        this.ts.appointmentStore.selected_appointment.recurringInterval.isSaturdaySet = result.isWeeklySaturday;
        this.ts.appointmentStore.selected_appointment.recurringInterval.isSundaySet = result.isWeeklySunday;
      }
      else if (result.recurrenceType == 3) {
        // Default
        this.ts.appointmentStore.selected_appointment.recurringInterval.dayInterval = 1
        // Monthly
        this.ts.appointmentStore.selected_appointment.recurringInterval.intervalType = 3;
        this.ts.appointmentStore.selected_appointment.recurringInterval.monthInterval = Number(result.monthlyValue);
        if (result.monthlySettings == 'dayOfMonth') {
          this.ts.appointmentStore.selected_appointment.recurringInterval.subType = 1;
          this.ts.appointmentStore.selected_appointment.recurringInterval.dayOfMonth = Number(result.dayOfMonthValue);
        }
        else {
          this.ts.appointmentStore.selected_appointment.recurringInterval.subType = 2;
          this.ts.appointmentStore.selected_appointment.recurringInterval.dayQualifier = Number(result.weekOfMonth);
          this.ts.appointmentStore.selected_appointment.recurringInterval.dayOfWeek = Number(result.dayOfWeekOfMonth);
        }
      }
      else {
        // Yearly
        this.ts.appointmentStore.selected_appointment.recurringInterval.intervalType = 4;
        this.ts.appointmentStore.selected_appointment.recurringInterval.month = result.monthOfYear;
        this.ts.appointmentStore.selected_appointment.recurringInterval.dayOfMonth = result.dayOfMonthYearValue;
      }

      // Set End
      if (result.endDateOption == 'endAfterOccurrences') {
        let numberOfOccurrences = Number(result.occurrences);
        this.ts.appointmentStore.selected_appointment.recurringInterval.endType = 2;
        this.ts.appointmentStore.selected_appointment.recurringInterval.endOccurs = numberOfOccurrences;
        let startsAt = DateUtils.createDateTime(result.date, this.endTime);
        let endDate = startsAt.plus({ days: numberOfOccurrences });
        this.ts.appointmentStore.selected_appointment.recurringInterval.endDate = endDate.toFormat(DateUtils.Constants.API_DATE_FORMAT);
      }
      else if (result.endDateOption == 'endBy') {
        this.ts.appointmentStore.selected_appointment.recurringInterval.endType = 3;
        if (result.endByDate == null) {
          this.ts.appointmentStore.validation_results = [{ severity: Severity.error, message: 'End by date is required' }];
          return;
        }
        let endDate = DateUtils.createDateTime(result.endByDate, this.endTime);
        this.ts.appointmentStore.selected_appointment.recurringInterval.endDate = endDate.toFormat(DateUtils.Constants.API_DATE_FORMAT);
        this.ts.appointmentStore.selected_appointment.recurringInterval.endOccurs = 3;
      }
      else {
        this.ts.appointmentStore.selected_appointment.recurringInterval.endType = 1;
      }
    }

    this.ts.appointmentStore.validate(this.ts.appointmentStore.selected_appointment);

    when(() => this.ts.appointmentStore.inprogress == false,
      () => {
        let validationResults = this.ts.appointmentStore.validation_results;
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
          this.ts.appointmentStore.create(this.ts.appointmentStore.selected_appointment);
        }
        else {
          this.ts.appointmentStore.update(this.ts.appointmentStore.selected_appointment);
        }

        this.onClose.next(true);
        this.bsModalRef.hide();
      })
  }

  hasValidationMessage() {
    return this.ts.appointmentStore.validation_results != null && this.ts.appointmentStore.validation_results.length > 0;
  }

  hasDeleteMessage() {
    return this.ts.appointmentStore.delete_validation_results != null && this.ts.appointmentStore.delete_validation_results.length > 0;
  }

  onEndSeries() {
    this.apptFormState = apptFormState.validatingEndSeries;
    if (!this.displayEndSeriesMessage) {
      this.displayEndSeriesMessage = true;
      return
    }
    this.ts.appointmentStore.endRecurringSeries(this.ts.appointmentStore.selected_appointment.id);
    this.onClose.next(null);
    this.bsModalRef.hide();
  }

  onDelete() {
    this.apptFormState = apptFormState.validatingDelete;
    if (!this.displayDeleteMessage) {
      this.displayDeleteMessage = true;
      this.ts.appointmentStore.validateDelete(this.ts.appointmentStore.selected_appointment.id);
      return;
    }

    when(() => this.ts.appointmentStore.delete_validation_results != null,
      () => {
        if (this.ts.appointmentStore.delete_validation_results.length > 0)
          return;

        this.scheduler.deleteEvent('A-' + this.ts.appointmentStore.selected_appointment.id);
        this.ts.appointmentStore.delete(this.ts.appointmentStore.selected_appointment.id);

        this.onClose.next(null);
        this.bsModalRef.hide();
      });
  }

  onCancel() {
    this.onClose.next(false);
    this.bsModalRef.hide();
  }
}
