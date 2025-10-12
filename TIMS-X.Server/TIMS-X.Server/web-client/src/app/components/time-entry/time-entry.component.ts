import { AfterViewInit, Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import * as _ from 'underscore';

@Component({
  selector: 'app-time-entry',
  templateUrl: './time-entry.component.html',
  styleUrls: ['./time-entry.component.scss']
})
export class TimeEntryComponent implements OnInit, AfterViewInit {
  @Input() timeForm: FormGroup;
  @Input() get dateTime() {
    return this._dateTime;
  }
  set dateTime(value) {
    //console.log('TimeEntry:datetime setter ' + value.toString());
    this._dateTime = value;
    this.setTime();
  }
  @Output() dateTimeChange = new EventEmitter<Date>();
  private _dateTime: Date;
  public isDisabled: boolean = false;
  private readonly AM = 'AM';
  private readonly PM = 'PM';

  constructor(private formBuilder: FormBuilder) { }

  public amOrPm: string = this.AM;

  get f() {
    return this.timeForm.controls;
  }

  ngOnInit() {
    this.timeForm = this.formBuilder.group({
      hours: [0, Validators.required],
      minutes: [0, Validators.required],
      am: [true, Validators.required]
    });
  }

  ngAfterViewInit(): void {
    this.setTime();
  }

  public updateModel() {
    let hours = Number(this.timeForm.value.hours);
    let minutes = Number(this.timeForm.value.minutes);
    if (this.amOrPm == this.PM) hours = hours + 12;
    this._dateTime = new Date(
      this._dateTime.getFullYear(), this._dateTime.getMonth(), this._dateTime.getDay(),
      hours, minutes, 0);
    this.dateTimeChange.emit(this._dateTime);
  }

  private setTime() {
    //console.log('Time Entry:SetTime ' + this.dateTime.toString());
    if (this.timeForm && this.dateTime) {
      let hrs = this._dateTime.getHours();
      if (hrs >= 12) {
        if (hrs > 12)
          hrs = hrs - 12;
        this.amOrPm = this.PM
      }
      else {
        this.amOrPm = this.AM
      }
      this.timeForm.patchValue({ hours: hrs });
      let minutes = this._dateTime.getMinutes();
      let value = '00';
      if (minutes > 0) {
        if (minutes < 10)
          value = `0${minutes}`;
        else
          value = `${minutes}`;
      }
      this.timeForm.patchValue({ minutes: value });
    }
  }

  public markAsTouched() {
    _.each(this.f, (x) => { x.markAsTouched() });
  }

  public markAsUntouched() {
    _.each(this.f, (x) => { x.markAsUntouched(); x.markAsPristine(); });
  }

  public disable() {
    if (this.timeForm) this.timeForm.disable();
    this.isDisabled = true;
  }

  public enable() {
    if (this.timeForm) this.timeForm.enable();
    this.isDisabled = false;
  }

  public setAmPm() {
    if (this.amOrPm == this.AM)
      this.amOrPm = this.PM;
    else
      this.amOrPm = this.AM;
    this.updateModel();
  }
}
