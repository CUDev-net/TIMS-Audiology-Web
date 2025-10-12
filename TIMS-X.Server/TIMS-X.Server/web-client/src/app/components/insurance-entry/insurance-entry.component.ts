import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ValidationErrors } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import * as _ from 'underscore';
import { DateUtils } from '@app/helpers/date-utils';
import { when } from "mobx";

import { StringUtils } from '@app/utilities/string-utils';

import { TimsStore } from '@app/stores/tims.store';
import { Entities } from '@app/entities/entities';

@Component({
  selector: 'app-insurance-entry',
  templateUrl: './insurance-entry.component.html',
  styleUrl: './insurance-entry.component.scss'
})
export class InsuranceEntryComponent implements OnInit {
  @Input() payerform: FormGroup;
  @Input() header: string;
  public isRelatedSelected: boolean = false;
  public otherinsured: boolean = false;
  bsConfig?: Partial<BsDatepickerConfig>;

  public patientInsurance: Entities.PatientInsurance;

  constructor(private formBuilder: FormBuilder,
    public ts: TimsStore) {
  }

  get f() {
    return this.payerform.controls;
  }

  public isValid(): boolean {
    return this.payerform.value.payerid != null;
  }

  public isNew(): boolean {
    return this.patientInsurance == null || this.patientInsurance.id == null || this.patientInsurance.id == 0;
  }

  ngOnInit(): void {
    this.payerform = this.formBuilder.group({
      payerid: [null],
      relationshipid: [null],
      insuranceid: [null],
      groupname: [null],
      groupnumber: [null],
      medicaresecondarycodeid: [null],
      carrierid: [null],
      notes: [null],
      lastname: [null],
      middlename: [null],
      firstname: [null],
      address1: [null],
      address2: [null],
      city: [null],
      state: [null],
      zipcode: [null],
      phone: [null],
      birthdate: [null],
      genderid: [null],
      releasesignaturedate: [null]
    });
    this.onPayerChanged();
    this.onRelationshipChanged();
    this.bsConfig = Object.assign({}, { containerClass: 'theme-dark-blue', customTodayClass: 'tims-today-class', showWeekNumbers: false });
  }

  ngAfterViewInit(): void {
  }

  public handlesInsuranceClick(isRelatedSelected: boolean) {
    this.isRelatedSelected = !isRelatedSelected;
  }

  onPayerChanged(): void {
    var payerid = this.f['payerid'].value
    if (payerid == null || payerid == '0') {
      this.f['relationshipid'].disable();
      this.f['insuranceid'].disable();
      this.f['groupname'].disable();
      this.f['groupnumber'].disable();
      this.f['medicaresecondarycodeid'].disable();
      this.f['carrierid'].disable();
      this.f['notes'].disable();
      this.clearform();
    }
    else {
      this.f['relationshipid'].enable();
      this.f['insuranceid'].enable();
      this.f['groupname'].enable();
      this.f['groupnumber'].enable();
      this.f['medicaresecondarycodeid'].enable();
      this.f['groupnumber'].enable();
      this.f['carrierid'].enable();
      this.f['notes'].enable();
    }
  }

  onRelationshipChanged() {
    var relationshipid = this.f['relationshipid'].value
    if (relationshipid == null || relationshipid == '01') {
      this.f['firstname'].disable();
      this.f['middlename'].disable();
      this.f['lastname'].disable();
      this.f['address1'].disable();
      this.f['address2'].disable();
      this.f['city'].disable();
      this.f['state'].disable();
      this.f['zipcode'].disable();
      this.f['phone'].disable();
      this.f['birthdate'].disable();
      this.f['genderid'].disable();
      this.f['releasesignaturedate'].disable();
    }
    else {
      this.f['firstname'].enable();
      this.f['middlename'].enable();
      this.f['lastname'].enable();
      this.f['address1'].enable();
      this.f['address2'].enable();
      this.f['city'].enable();
      this.f['state'].enable();
      this.f['zipcode'].enable();
      this.f['phone'].enable();
      this.f['birthdate'].enable();
      this.f['genderid'].enable();
      this.f['releasesignaturedate'].enable();
    }
  }

  zipChanged(event) {
    if (this.f.zipcode.value.length == 5) {
      this.ts.lookupStore.getCityAndStateFromZipCode(this.f.zipcode.value);
      when(() => this.ts.lookupStore.cityAndState != null, () => {
        let city = StringUtils.toTitleCase(this.ts.lookupStore.cityAndState.city);
        this.f.city.setValue(city);
        this.f.state.setValue(this.ts.lookupStore.cityAndState.state);
      });
    }
  }

  public markAsTouched() {
    _.each(this.f, (x) => { x.markAsTouched() });
  }

  public markAsUntouched() {
    _.each(this.f, (x) => { x.markAsUntouched(); x.markAsPristine(); });
  }

  public patchForm(patientInsurance: Entities.PatientInsurance): void {
    this.patientInsurance = patientInsurance;
    this.payerform.patchValue({ payerid: this.patientInsurance.insurancePayerId });
    this.payerform.patchValue({ relationshipid: this.patientInsurance.relationtoInsured });
    this.payerform.patchValue({ insuranceid: this.patientInsurance.idNumber });
    this.payerform.patchValue({ groupname: this.patientInsurance.policyGroupName });
    this.payerform.patchValue({ groupnumber: this.patientInsurance.policyGroupNum });
    this.payerform.patchValue({ medicaresecondarycodeid: this.patientInsurance.policyType });
    this.payerform.patchValue({ carrierid: this.patientInsurance.carrierCode });
    this.payerform.patchValue({ notes: this.patientInsurance.insurNotes });

    this.payerform.patchValue({ firstname: this.patientInsurance.firstName });
    this.payerform.patchValue({ middlename: this.patientInsurance.middleName });
    this.payerform.patchValue({ lastname: this.patientInsurance.lastName });
    this.payerform.patchValue({ address1: this.patientInsurance.address1 });
    this.payerform.patchValue({ address2: this.patientInsurance.address2 });
    this.payerform.patchValue({ city: this.patientInsurance.city });
    this.payerform.patchValue({ state: this.patientInsurance.state });
    this.payerform.patchValue({ zipcode: this.patientInsurance.zipCode });
    this.payerform.patchValue({ phone: this.patientInsurance.phone });
    this.payerform.patchValue({ genderid: this.patientInsurance.sex });
    if (this.patientInsurance.birthDate)
      this.payerform.patchValue({ birthdate: new Date(this.patientInsurance.birthDate) });
    if (this.patientInsurance.signatureDate)
      this.payerform.patchValue({ releasesignaturedate: new Date(this.patientInsurance.signatureDate) });
    this.onPayerChanged();
    this.onRelationshipChanged();
  }

  public submitForm(): void {
    const result = Object.assign({}, this.payerform.value);
    this.patientInsurance.insurancePayerId = result.payerid;
    this.patientInsurance.relationtoInsured = result.relationshipid;
    this.patientInsurance.idNumber = result.insuranceid;
    this.patientInsurance.policyGroupName = result.groupname;
    this.patientInsurance.policyGroupNum = result.groupnumber;
    this.patientInsurance.policyType = result.medicaresecondarycodeid;
    this.patientInsurance.carrierCode = result.carrierid;
    this.patientInsurance.insurNotes = result.notes;

    if (this.patientInsurance.relationtoInsured == null || this.patientInsurance.relationtoInsured == '01')
      this.clearOtherForm();

    this.patientInsurance.firstName = result.firstname;
    this.patientInsurance.lastName = result.lastname;
    this.patientInsurance.middleName = result.middlename;
    this.patientInsurance.address1 = result.address1;
    this.patientInsurance.address2 = result.address2;
    this.patientInsurance.city = result.city;
    this.patientInsurance.state = result.state;
    this.patientInsurance.zipCode = result.zipcode;
    this.patientInsurance.phone = result.phone;
    this.patientInsurance.sex = result.genderid;
    if (result.birthdate)
      this.patientInsurance.birthDate = DateUtils.createDbDateOnlyFromDate(result.birthdate);
    else
      this.patientInsurance.birthDate = null;
    if (result.releasesignaturedate)
      this.patientInsurance.signatureDate = DateUtils.createDbDateOnlyFromDate(result.releasesignaturedate);
    else
      this.patientInsurance.signatureDate = null;
  }

  private clearform() {
    this.payerform.patchValue({ payerid: null });
    this.payerform.patchValue({ relationshipid: null });
    this.payerform.patchValue({ insuranceid: null });
    this.payerform.patchValue({ groupname: null });
    this.payerform.patchValue({ groupnumber: null });
    this.payerform.patchValue({ medicaresecondarycodeid: null });
    this.payerform.patchValue({ carrierid: null });
    this.payerform.patchValue({ notes: null });
    this.clearOtherForm();
  }

  private clearOtherForm() {
    this.payerform.patchValue({ firstname: '' });
    this.payerform.patchValue({ middlename: null });
    this.payerform.patchValue({ lastname: '' });
    this.payerform.patchValue({ address1: null });
    this.payerform.patchValue({ address2: null });
    this.payerform.patchValue({ city: null });
    this.payerform.patchValue({ state: null });
    this.payerform.patchValue({ zipcode: null });
    this.payerform.patchValue({ phone: null });
    this.payerform.patchValue({ genderid: null });
    this.payerform.patchValue({ birthdate: null });
    this.payerform.patchValue({ releasesignaturedate: null });
  }
}
