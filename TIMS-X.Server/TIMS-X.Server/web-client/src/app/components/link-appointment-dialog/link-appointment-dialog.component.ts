import { Component, OnInit, TemplateRef } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Subject } from 'rxjs';
import * as _ from 'underscore';
import { DateTime } from "luxon";
import { when } from "mobx";

import { TimsStore } from '@app/stores/tims.store';
import { Entities } from '@app/entities/entities';

import PatientSearchCriteria = Entities.PatientSearchCriteriaDto;

@Component({
  selector: 'app-link-appointment-dialog',
  templateUrl: './link-appointment-dialog.component.html',
  styleUrl: './link-appointment-dialog.component.scss'
})
export class LinkAppointmentDialogComponent implements OnInit {
  public onClose: Subject<boolean>;
  public linkPatientForm: FormGroup;
  public appointment: Entities.PatientAppointmentItemDto;
  modalRef?: BsModalRef;
  public confirmMessage: string;
  private selectedPatientId;

  public get ts() { return this.timsStore; }
  public get f() { return this.linkPatientForm.controls; }

  constructor(private formBuilder: FormBuilder,
    public timsStore: TimsStore,
    public bsModalRef: BsModalRef,
    private modalService: BsModalService) {

  }

  ngOnInit(): void {
    this.linkPatientForm = this.formBuilder.group({
      searchParameter: ['']
    });
    this.onClose = new Subject();
  }

  onSearch() {
    const result = Object.assign({}, this.linkPatientForm.value);
    let criteria = new PatientSearchCriteria();
    if (result.searchCriteria == 'lastname')
      criteria.lastName = result.searchParameter;
    else if (result.searchCriteria == 'firstname')
      criteria.firstName = result.searchParameter;
    else if (result.searchCriteria == 'birthdate')
      criteria.dateOfBirth = result.searchParameter;
    else if (result.searchCriteria == 'phone')
      criteria.phoneNumber = result.searchParameter;
    criteria.includeInactive = result.includeInactive;

    this.ts.schedulerStore.getPatientAppointmentSearch(result.searchParameter);
  }

  onSearchValue(event) {
    if (event.key == "Enter") {
      this.onSearch();
    }
  }

  selectPatient(patientId: number, template: TemplateRef<void>) {
    this.selectedPatientId = patientId;
    let patient = _.find(this.ts.schedulerStore.patient_appointment_candidates, (x) => { return x.patientId == patientId });
    let date = DateTime.fromISO(patient.dateOfBirth);
    let name = patient.initial ? `${patient.lastName}, ${patient.firstName} ${patient.initial}` : `${patient.lastName}, ${patient.firstName}`
    this.confirmMessage = `Link appointment to ${name} ${date.toLocaleString(DateTime.DATE_SHORT)}?`;
    this.modalRef = this.modalService.show(template, { class: 'modal-med' });
  }

  validateForm() {
    _.each(this.f, (x) => { x.markAsTouched() });
  }

  confirm() {
    let link = new Entities.PatientLinkDto();
    link.appointmentId = this.appointment.appointmentId;
    link.patientId = this.selectedPatientId;
    let patient = _.find(this.ts.schedulerStore.patient_appointment_candidates, (x) => { return x.patientId == this.selectedPatientId });
    link.firstName = patient.firstName;
    link.initial = patient.initial;
    link.lastName = patient.lastName;
    link.email = patient.email;
    link.phone = patient.phone;
    this.modalRef?.hide();

    this.ts.schedulerStore.linkPatientAppointment(link);
    when(
      () => this.ts.schedulerStore.inprogress == false,
      () => {
        this.onClose.next(true);
        this.bsModalRef.hide();
      });
  }

  decline() {
    this.modalRef?.hide();
  }

  onCancel() {
    this.onClose.next(false);
    this.bsModalRef.hide();
  }

  onCreatePatient() {
    this.ts.patientScheduleStore.createNewPatient = true;
    this.onClose.next(true);
    this.bsModalRef.hide();
  }
}
