import { AfterViewInit, Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormGroup } from '@angular/forms';
import * as _ from 'underscore';

import { when } from "mobx";

import { StringUtils } from '@app/utilities/string-utils';
import { TimsStore } from '@app/stores/tims.store';

@Component({
  selector: 'app-address-entry-2',
  templateUrl: './address-entry-2.component.html',
  styleUrls: ['./address-entry-2.component.css']
})
export class AddressEntry2Component implements AfterViewInit, OnInit {
  @Input() address2form: FormGroup;
  @Output() formReady = new EventEmitter<FormGroup>()

  constructor(private ts: TimsStore) { }

  get f() {
    return this.address2form.controls;
  }

  ngOnInit(): void {
  }

  ngAfterViewInit(): void {
    this.formReady.emit(this.address2form);
  }

  public zipChanged(event) {
    if (this.f.zipcode2.value.length == 5) {
      this.ts.lookupStore.getCityAndStateFromZipCode(this.f.zipcode2.value);
      when(() => this.ts.lookupStore.cityAndState != null, () => {
        let city = StringUtils.toTitleCase(this.ts.lookupStore.cityAndState.city);
        this.f.city2.setValue(city);
        this.f.state2.setValue(this.ts.lookupStore.cityAndState.state);
      });
    }
  }

  public markAsTouched() {

  }

  public markAsUntouched() {

  }

  public disable() {
    this.f['address2line1'].disable();
    this.f['address2line2'].disable();
    this.f['city2'].disable();
    this.f['state2'].disable();
    this.f['zipcode2'].disable();
  }

  public enable() {
    this.f['address2line1'].enable();
    this.f['address2line2'].enable();
    this.f['city2'].enable();
    this.f['state2'].enable();
    this.f['zipcode2'].enable();
  }
}
