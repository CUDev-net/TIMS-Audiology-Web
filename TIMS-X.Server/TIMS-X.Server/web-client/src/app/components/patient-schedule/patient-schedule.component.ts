import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { DateTime } from "luxon";
import { DateUtils } from '@app/helpers/date-utils';
import * as _ from 'underscore';
import { when } from "mobx";
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';

import { CustomValidators } from '@app/components/custom.validators';

import { TimsStore } from '@app/stores/tims.store';

import { Entities } from '@app/entities/entities';

@Component({
  selector: 'app-patient-schedule',
  templateUrl: './patient-schedule.component.html',
  styleUrl: './patient-schedule.component.scss'
})
export class PatientScheduleComponent implements OnInit {
  constructor(private formBuilder: FormBuilder, private router: Router, public ts: TimsStore) {
    this.ts.patientScheduleStore.getPractice();
    when(() => this.ts.patientScheduleStore.practice != null, () => {
      this.practiceName = this.ts.patientScheduleStore.practice.name;
    });
  }

  get f() { return this.scheduleForm.controls; }
  get locale() {
    if (this.ts.patientScheduleStore.practice == null)
      return 'US';
    return this.ts.patientScheduleStore.practice.locale;
  }
  public bsConfig?: Partial<BsDatepickerConfig>;
  public scheduleForm: FormGroup;
  public endTime: string;
  public practiceName: string;
  public logo;
  public badDate: boolean = false;
  public isSubmitted = false;
  public phonemask: string = '###-###-####';

  ngOnInit(): void {

    this.scheduleForm = this.formBuilder.group({
      lastname: ["", Validators.required],
      initial: [""],
      firstname: ["", Validators.required],
      birthdate: [null, Validators.required],
      phone: ["", Validators.required],
      emailaddress: ["", CustomValidators.email],
      providerId: [null, Validators.required],
      reason: [null, Validators.required],
      siteId: [null, Validators.required],
      date: [new Date(DateTime.now().plus({ days: 1 })), Validators.required],
      selectedTimeSlot: [null, Validators.required],
      isNewPatient: [false, Validators.required],
      recaptchaToken: new FormControl(null, Validators.required)
    });

    this.bsConfig = Object.assign({}, { containerClass: 'theme-dark-blue', customTodayClass: 'tims-today-class', showWeekNumbers: false, adaptivePosition: true });
    when(() => this.ts.patientScheduleStore.practice != null, () => {
      if (this.ts.patientScheduleStore.practice.locale == 'NZ' || this.ts.patientScheduleStore.practice.locale == 'AU') {
        this.bsConfig.dateInputFormat = 'DD/MM/YYYY';
        this.phonemask = '### ### ####'
      }
    });

    this.scheduleForm.get('date').valueChanges.subscribe(val => {
      this.badDate = false;
      if (val == null) return;
      let d = DateTime.fromJSDate(val);
      let today = DateTime.now();
      if (d.startOf("day") <= today.startOf("day")) {
        this.scheduleForm.patchValue({ date: new Date(DateTime.now().plus({ days: 1 })) });
        this.badDate = true;
        return;
      }
      this.getOpenings();
    });
    this.scheduleForm.get('providerId').valueChanges.subscribe(val => {
      this.getOpenings();
    });

    this.scheduleForm.get('siteId').valueChanges.subscribe(val => {
      this.getOpenings();
    });
    this.scheduleForm.get('birthdate').valueChanges.subscribe(val => {
      if (val == null) return;
      let x = DateTime.fromJSDate(val);
      if (!x.isValid || x.get('year') < 1899 || x.get('month') > 12 || x.get('day') > 31)
        this.scheduleForm.patchValue({ birthdate: null });
    });
  }

  onTimeChanged() {
    if (this.scheduleForm.value.selectedTimeSlot) {
      let st = DateTime.fromISO(this.scheduleForm.value.selectedTimeSlot);
      console.log(st);
      let et = st.plus({ minutes: 60 });
      this.endTime = et.toLocaleString(DateTime.TIME_SIMPLE);
    }
    else {
      this.endTime = '';
    }
  }

  getOpenings() {
    this.endTime = '';
    this.scheduleForm.patchValue({ selectedTimeSlot: null });
    if (this.scheduleForm.value.providerId && this.scheduleForm.value.siteId && this.scheduleForm.value.date) {
      let x = DateTime.fromJSDate(this.scheduleForm.value.date);
      this.ts.patientScheduleStore.getTimeSlots(this.scheduleForm.value.providerId,
        this.scheduleForm.value.siteId,
        x.toFormat(DateUtils.Constants.API_DATE_FORMAT)
      );
    }
  }

  validateForm() {
    _.each(this.f, (x) => { x.markAsTouched() })
  }

  onSubmit() {
    this.validateForm();

    if (this.scheduleForm.invalid) {
      return;
    }
    this.isSubmitted = true;
    const result = Object.assign({}, this.scheduleForm.value);

    var toCreate = new Entities.PatientScheduledAppointmentDto();
    toCreate.firstName = result.firstname;
    toCreate.lastName = result.lastname;
    toCreate.middleInitial = result.initial;
    toCreate.birthDate = new Date(result.birthdate.getFullYear(), result.birthdate.getMonth(), result.birthdate.getDate());
    toCreate.email = result.emailaddress;
    toCreate.phone = result.phone;
    toCreate.siteId = result.siteId;
    toCreate.reason = result.reason;
    toCreate.isNewPatient = result.isNewPatient;
    let st = new Date(this.scheduleForm.value.selectedTimeSlot);
    toCreate.date = DateUtils.createApiDateTime(result.date, st);

    let that = this.scheduleForm.value.selectedTimeSlot;
    var ts = _.find(this.ts.patientScheduleStore.timeSlots, function (x) { return x.timeSlot == that; });
    toCreate.providerId = ts.providerId;

    this.ts.patientScheduleStore.create(toCreate);
    when(() => this.ts.patientScheduleStore.inprogress == false, () => {
      this.router.navigateByUrl(`/patient-schedule-complete?officeCode=${this.ts.patientScheduleStore.officeCode}`);
    });
  }
}
