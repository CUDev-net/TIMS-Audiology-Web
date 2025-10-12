import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subject } from 'rxjs';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { DateTime } from "luxon";
import * as _ from 'underscore';

import { TimsStore } from '@app/stores/tims.store';
import { Entities } from '@app/entities/entities';
import { DateUtils } from '@app/helpers/date-utils';


@Component({
  selector: 'app-authorization-editor',
  templateUrl: './authorization-editor.component.html',
  styleUrl: './authorization-editor.component.scss'
})
export class AuthorizationEditorComponent implements OnInit {

  static modlaId: number = 9001;
  public authform: FormGroup;
  public onClose: Subject<boolean>;
  public currentAuthorization: Entities.ApptAuthorization;
  bsConfig?: Partial<BsDatepickerConfig>;

  constructor(private formBuilder: FormBuilder,
    public timsStore: TimsStore,
    public authModalRef: BsModalRef,
    private bsModalServie: BsModalService) {
  }

  public get f() { return this.authform.controls; }

  ngOnInit(): void {
    this.authform = this.formBuilder.group({
      name: [null, Validators.required],
      authorizations: [1],
      expirationdate: [null],
      isinactive: [false],
      notes: [null]
    });
    this.bsConfig = Object.assign({}, { containerClass: 'theme-dark-blue', customTodayClass: 'tims-today-class', showWeekNumbers: false });

    if (this.currentAuthorization.id > 0) {
      this.authform.patchValue({ name: this.currentAuthorization.name });
      this.authform.patchValue({ authorizations: this.currentAuthorization.authorizations });
      if (this.currentAuthorization.expires)
        this.authform.patchValue({ expirationdate: new Date(this.currentAuthorization.expires) });
      this.authform.patchValue({ isinactive: this.currentAuthorization.inactive });
      this.authform.patchValue({ notes: this.currentAuthorization.notes });
    }
    this.onClose = new Subject();
  }

  validateForm() {
    _.each(this.f, (x) => { x.markAsTouched() });
  }

  onSubmit() {
    this.validateForm();

    if (this.authform.invalid) {
      return;
    }

    const result = Object.assign({}, this.authform.getRawValue());

    this.currentAuthorization.name = result.name;
    this.currentAuthorization.authorizations = result.authorizations;
    this.currentAuthorization.expires = DateUtils.createDbDateOnlyFromDate(result.expirationdate);
    this.currentAuthorization.inactive = result.inactive;
    this.currentAuthorization.notes = result.notes;

    if (this.currentAuthorization.expires) {
      let expDate = DateTime.fromJSDate(this.currentAuthorization.expires).toLocaleString(DateTime.DATE_SHORT);
      this.currentAuthorization.displayString = `${this.currentAuthorization.name} (${this.currentAuthorization.numberUsed}/${this.currentAuthorization.authorizations}) - Exp. ${expDate}`;
    }
    else
      this.currentAuthorization.displayString = `${this.currentAuthorization.name} (${this.currentAuthorization.numberUsed}/${this.currentAuthorization.authorizations})`;

    this.onClose.next(true);
    this.bsModalServie.hide(AuthorizationEditorComponent.modlaId);
  }

  showDelete() {
    return this.currentAuthorization != null && this.currentAuthorization.id != 0;
  }

  onDelete() {
    this.currentAuthorization.isDeleted = true;
    this.onClose.next(true);
    this.bsModalServie.hide(AuthorizationEditorComponent.modlaId);
  }

  onCancel() {
    this.onClose.next(false);
    this.bsModalServie.hide(AuthorizationEditorComponent.modlaId);
  }
}

