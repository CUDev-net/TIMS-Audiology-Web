import { Component, ElementRef, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { BsModalService, BsModalRef, ModalOptions } from 'ngx-bootstrap/modal';
import { DateTime } from "luxon";
import { observe, when } from "mobx";
import { SchedulerStatic } from "@app/dhtmlx/dhtmlxscheduler";

import { TimsStore } from '@app/stores/tims.store';

import { AppointmentEditorComponent } from "../appointment-editor/appointment-editor.component";
import { PatientEditorComponent } from '../patient-editor/patient-editor.component';

import { Entities } from '@app/entities/entities';
import PatientSummary = Entities.PatientSummary;

@Component({
  selector: 'app-patient-summary',
  templateUrl: './patient-summary.component.html',
  styleUrl: './patient-summary.component.scss'
})
export class PatientSummaryComponent implements OnInit {
  @Output() cancel = new EventEmitter();
  @ViewChild('patientSummaryElement', { static: true }) patientSummaryElement: ElementRef;
  @ViewChild('patientHistoryElement', { static: true }) patientHistoryElement: ElementRef;

  private emptyPatient = new PatientSummary();
  public isSummarySelected = true;
  public containerHeight: number;
  private bsModalRef: BsModalRef;
  public isOpportunity = false;
  private disposer;
  private scheduler: SchedulerStatic;

  public get CurrentPatient() {
    this.setOtStatus();
    return this.ts.patientStore.selected_patient_summary == null ? this.emptyPatient : this.ts.patientStore.selected_patient_summary;
  }

  public get NextAppointment() {
    return this.ts.patientStore.selected_patient_summary == null || this.ts.patientStore.selected_patient_summary.nextAppointmentDate == null ? '' :
      DateTime.fromISO(this.ts.patientStore.selected_patient_summary.nextAppointmentDate).toLocaleString(DateTime.DATETIME_SHORT) + ' : ' + this.ts.patientStore.selected_patient_summary.nextAppointmentStatus;
  }

  public get LastAppointment() {
    return this.ts.patientStore.selected_patient_summary == null || this.ts.patientStore.selected_patient_summary.lastAppointmentDate == null ? '' :
      DateTime.fromISO(this.ts.patientStore.selected_patient_summary.lastAppointmentDate).toLocaleString(DateTime.DATETIME_SHORT) + ' : ' + this.ts.patientStore.selected_patient_summary.lastAppointmentStatus;
  }

  public get hasPhone() {
    return this.CurrentPatient.homePhone || this.CurrentPatient.workPhone || this.CurrentPatient.mobilePhone;
  }

  public get LastUpdate() {
    return 'Last updated ' + DateTime.fromISO(this.CurrentPatient.updatedDate).toLocaleString(DateTime.DATETIME_SHORT) + ' by ' + this.CurrentPatient.updatedByUserName;
  }

  constructor(public ts: TimsStore, private modalService: BsModalService) {
  }

  ngOnInit(): void {
  }

  handlesCancelPreview() {
    this.cancel.emit();
    this.isSummarySelected = true;
  }

  handlesAppointmentsClick() {
    this.isSummarySelected = false;
    this.containerHeight = this.patientSummaryElement.nativeElement.offsetHeight - 1; // 1 keeps browser scroll bar hidden
  }

  handlesDisplayAppointmentClick(id: number) {
    this.ts.appointmentStore.getById(Number(id));
    when(() => this.ts.appointmentStore.inprogress == false,
      () => {
        let scheduler = this.scheduler
        const initialState: ModalOptions = { initialState: { scheduler }, class: 'modal-lg', ignoreBackdropClick: true };
        this.bsModalRef = this.modalService.show(AppointmentEditorComponent, initialState);
        this.bsModalRef.content.onClose.subscribe(result => {
          if (result) {
            this.scheduler.updateEvent('A-' + id);
          }
        });
      });
  }

  handlesEditPatient() {
    this.ts.patientStore.selected_patient = null;
    this.ts.patientStore.getById(this.ts.patientStore.selected_patient_summary.id);
    this.disposer = observe(this.ts.patientStore, "selected_patient", (change) => {
      this.disposer();
      let currentPatient = this.ts.patientStore.selected_patient;
      const initialState: ModalOptions = { initialState: { currentPatient }, class: 'ex-modal-dialog', ignoreBackdropClick: true, id: PatientEditorComponent.modlaId };
      this.bsModalRef = this.modalService.show(PatientEditorComponent, initialState);
      this.bsModalRef.content.onClose.subscribe(result => {
        if (result) {
          // this.disposer = observe(this.patientStore, "selected_patient", (change) => {
          //   this.router.navigate(['/scheduler/schedule'], { queryParams: { id: this.patientStore.selected_patient.id } });
          // });
        }
      });
    });
  }

  setOtStatus() {
    if (this.ts.patientStore.selected_patient_summary == null) return;
    switch (this.ts.patientStore.selected_patient_summary.otStatusId) {
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

  handlesSummaryClick() {
    this.isSummarySelected = true;
  }
}
