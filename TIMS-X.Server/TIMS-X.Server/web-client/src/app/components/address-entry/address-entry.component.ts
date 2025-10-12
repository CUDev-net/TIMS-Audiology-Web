import { Component, Input, OnInit } from '@angular/core';
import { FormGroup, ValidationErrors } from '@angular/forms';
import * as _ from 'underscore';
import { when } from "mobx";

import { StringUtils } from '@app/utilities/string-utils';

import { TimsStore } from '@app/stores/tims.store';

@Component({
  selector: 'app-address-entry',
  templateUrl: './address-entry.component.html',
  styleUrls: ['./address-entry.component.scss']
})
export class AddressEntryComponent implements OnInit {
  @Input() addressform: FormGroup;
  @Input() addressLine1Required: ValidationErrors = null;
  @Input() addressLine2Required: ValidationErrors = null;
  @Input() cityRequired: ValidationErrors = null;
  @Input() stateRequired: ValidationErrors = null;
  @Input() zipCodeRequired: ValidationErrors = null;

  constructor(public ts: TimsStore) { }

  get f() {
    return this.addressform.controls;
  }

  ngOnInit(): void {
  }

  public zipChanged(event) {
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

  public disable() {
    if (this.addressform) this.addressform.disable();
  }

  public enable() {
    if (this.addressform) this.addressform.enable();
  }

}
