import { AfterViewInit, Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import * as _ from 'underscore';

@Component({
  selector: 'app-duration-entry',
  templateUrl: './duration-entry.component.html',
  styleUrls: ['./duration-entry.component.scss']
})
export class DurationEntryComponent implements OnInit, AfterViewInit {
  @Input() durationForm: FormGroup;
  @Input() get minutes() {
    return this._minutes;
  }
  set minutes(value) {
    this._minutes = value;
    this.setTime();
  }
  @Output() minutesChange = new EventEmitter<number>();
  private _minutes: number;
  public isDisabled: boolean = false;

  constructor(private formBuilder: FormBuilder) { }
  get f() { return this.durationForm.controls; }

  ngOnInit() {
    this.durationForm = this.formBuilder.group({
      hours: [0, Validators.required],
      minutes: [0, Validators.required],
    });
  }

  ngAfterViewInit(): void {
    this.setTime();
  }

  public updateModel() {
    let hours = Number(this.durationForm.value.hours);
    let minutes = Number(this.durationForm.value.minutes);
    this.minutes = (hours * 60) + minutes;
    this.minutesChange.emit(this.minutes);
  }

  private setTime() {
    if (this.durationForm && this.minutes) {
      let min = this.minutes % 60;
      let hr = Math.floor(this.minutes / 60);
      this.durationForm.patchValue({ hours: hr });
      this.durationForm.patchValue({ minutes: min });
    }
  }

  public markAsTouched() {
    _.each(this.f, (x) => { x.markAsTouched() });
  }

  public markAsUntouched() {
    _.each(this.f, (x) => { x.markAsUntouched(); x.markAsPristine(); });
  }

  public disable() {
    if (this.durationForm) this.durationForm.disable();
    this.isDisabled = true;
  }

  public enable() {
    if (this.durationForm) this.durationForm.enable();
    this.isDisabled = false;
  }
}
