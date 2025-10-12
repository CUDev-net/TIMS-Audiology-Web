import { Component, Input, OnInit } from '@angular/core';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { FormGroup } from '@angular/forms';
import * as _ from 'underscore';

@Component({
  selector: 'app-recurrence-editor',
  templateUrl: './recurrence-editor.component.html',
  styleUrl: './recurrence-editor.component.scss'
})
export class RecurrenceEditorComponent implements OnInit {
  public bsConfig?: Partial<BsDatepickerConfig>;
  public isDaily: boolean = false;
  public isWeekly: boolean = false;
  public isMonthly: boolean = false;
  public isYearly: boolean = false;

  @Input() recurrenceForm: FormGroup;
  @Input() allowEndDate: boolean = true;
  recurrenceOptions = RecurrenceEditorComponent.RecurrenceOptions;
  dailyOptions = RecurrenceEditorComponent.DailyOptions;
  weekOfMonthOptions = RecurrenceEditorComponent.WeekOfMonthOptions;
  dayOfWeekOptions = RecurrenceEditorComponent.DayOfWeekOptions;
  monthOptions = RecurrenceEditorComponent.MonthOptions;

  constructor() { }

  get f() { return this.recurrenceForm.controls; }

  ngOnInit() {
    this.bsConfig = Object.assign({}, { containerClass: 'theme-dark-blue', customTodayClass: 'tims-today-class', showWeekNumbers: false });
    this.isrecurrenceChanged();
    this.setRecurrenceOptions();
    if (!this.f['isnew'].value) {
      this.f['isrecurrence'].disable();
      this.disable();
    }
  }

  onRecurrenceTypeChanged() {
    this.setRecurrenceOptions();
  }

  setRecurrenceOptions() {
    let value = this.f['recurrenceType'].value;
    if (value == '1') {
      this.isDaily = true;
      this.isWeekly = false;
      this.isMonthly = false;
      this.isYearly = false;
    }
    else if (value == '2') {
      this.isDaily = false;
      this.isWeekly = true;
      this.isMonthly = false;
      this.isYearly = false;
    }
    else if (value == '3') {
      this.isDaily = false;
      this.isWeekly = false;
      this.isMonthly = true;
      this.isYearly = false;
    }
    else if (value == '4') {
      this.isDaily = false;
      this.isWeekly = false;
      this.isMonthly = false;
      this.isYearly = true;
    }
  }

  isrecurrenceChanged() {
    let value = this.f['isrecurrence'].value;
    this.enable(value);
  }

  disable() {
    this.enable(false);
  }

  enable(enable: boolean) {
    this.enableControl('recurrenceType', enable);
    this.enableControl('dailySettings', enable);
    this.enableControl('endDateOption', enable);
    this.enableControl('dailyValue', enable);
    this.enableControl('occurrences', enable);
    this.enableControl('endByDate', enable);

    this.enableControl('weeklyValue', enable);
    this.enableControl('isWeeklyMonday', enable);
    this.enableControl('isWeeklyTuesday', enable);
    this.enableControl('isWeeklyWednesday', enable);
    this.enableControl('isWeeklyThursday', enable);
    this.enableControl('isWeeklyFriday', enable);
    this.enableControl('isWeeklySaturday', enable);
    this.enableControl('isWeeklySunday', enable);

    this.enableControl('monthlyValue', enable);
    this.enableControl('monthlySettings', enable);
    this.enableControl('dayOfMonthValue', enable);
    this.enableControl('weekOfMonth', enable);
    this.enableControl('dayOfWeekOfMonth', enable);

    this.enableControl('monthOfYear', enable);
    this.enableControl('dayOfMonthYearValue', enable);
  }

  enableControl(controlName: string, enable: boolean) {
    if (enable) {
      this.f[controlName].enable();
    }
    else {
      this.f[controlName].disable();
    }
  }

  public static RecurrenceOptions = [
    { id: 1, label: 'Daily' },
    { id: 2, label: 'Weekly' },
    { id: 3, label: 'Monthly' },
    { id: 4, label: 'Yearly' }
  ]

  public static WeekOfMonthOptions = [
    { id: 1, label: 'first' },
    { id: 2, label: 'second' },
    { id: 3, label: 'third' },
    { id: 4, label: 'fourth' },
    { id: 5, label: 'last' }
  ]

  public static DayOfWeekOptions = [
    { id: 1, label: 'Monday' },
    { id: 2, label: 'Tuesday' },
    { id: 3, label: 'Wednesday' },
    { id: 4, label: 'Thursday' },
    { id: 5, label: 'Friday' },
    { id: 6, label: 'Saturday' },
    { id: 7, label: 'Sunday' }
  ]

  public static DailyOptions = [
    { id: 1 },
    { id: 2 },
    { id: 3 },
    { id: 4 },
    { id: 5 },
    { id: 6 },
    { id: 7 },
    { id: 8 },
    { id: 9 },
    { id: 10 },
    { id: 11 },
    { id: 12 },
    { id: 13 },
    { id: 14 },
    { id: 15 },
    { id: 16 },
    { id: 17 },
    { id: 18 },
    { id: 19 },
    { id: 20 },
    { id: 21 },
    { id: 22 },
    { id: 23 },
    { id: 24 },
    { id: 25 },
    { id: 26 },
    { id: 27 },
    { id: 29 },
    { id: 29 },
    { id: 30 }
  ]

  public static MonthOptions = [
    { id: 1, label: 'January' },
    { id: 2, label: 'February' },
    { id: 3, label: 'March' },
    { id: 4, label: 'April' },
    { id: 5, label: 'May' },
    { id: 6, label: 'June' },
    { id: 7, label: 'July' },
    { id: 8, label: 'August' },
    { id: 9, label: 'September' },
    { id: 10, label: 'October' },
    { id: 11, label: 'November' },
    { id: 12, label: 'December' }
  ]

}
