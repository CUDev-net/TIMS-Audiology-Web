import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { BsModalService, BsModalRef, ModalOptions } from 'ngx-bootstrap/modal';
import { when } from "mobx";
import * as _ from 'underscore';
import { DateTime } from "luxon";

import { LinkAppointmentDialogComponent } from "../link-appointment-dialog/link-appointment-dialog.component";
import { AppointmentEditorComponent } from "../appointment-editor/appointment-editor.component";
import { PatientEditorComponent } from '../patient-editor/patient-editor.component';

import { TimsStore } from '@app/stores/tims.store';

import { Entities } from '@app/entities/entities';

@Component({
  selector: 'app-patient-appointment-center',
  templateUrl: './patient-appointment-center.component.html',
  styleUrl: './patient-appointment-center.component.scss'
})
export class PatientAppointmentCenterComponent implements OnInit {
  @Output() closeCenter = new EventEmitter<boolean>();
  @Output() schedulerGotoDate = new EventEmitter<Date>();
  @Output() updateEvent = new EventEmitter<string>();
  private bsModalRef: BsModalRef;
  private bsApptModalRef: BsModalRef;

  constructor(private modalService: BsModalService,
    public ts: TimsStore) {
  }

  ngOnInit(): void {
  }

  onGetPatientAppointments() {
    this.ts.schedulerStore.getPatientAppointments();
    when(
      () => this.ts.schedulerStore.is_fetching_patient_appointments == false,
      () => {
      });
  }

  selectPatient(appointmentId: number) {
    this.ts.patientScheduleStore.createNewPatient = false;
    this.ts.schedulerStore.patient_appointment_candidates = [];
    let appointment = _.find(this.ts.schedulerStore.patient_appointments, (x) => { return x.appointmentId == appointmentId });
    const initialState: ModalOptions = { initialState: { appointment }, class: 'modal-lg', ignoreBackdropClick: true };
    this.ts.schedulerStore.getPatientAppointmentCandidates(appointment.firstName, appointment.lastName, appointment.birthDate, appointment.email, appointment.phone);
    this.bsModalRef = this.modalService.show(LinkAppointmentDialogComponent, initialState);
    this.bsModalRef.content.onClose.subscribe(result => {
      if (result) {
        if (this.ts.patientScheduleStore.createNewPatient) {
          let currentPatient = new Entities.Patient();
          currentPatient.firstName = appointment.firstName;
          currentPatient.initial = appointment.initial;
          currentPatient.lastName = appointment.lastName;
          currentPatient.birthDate = DateTime.fromISO(appointment.birthDate);
          currentPatient.email = appointment.email;
          currentPatient.mobilePhone = appointment.phone;
          const initialState: ModalOptions = { initialState: { currentPatient }, class: 'ex-modal-dialog', ignoreBackdropClick: true, id: PatientEditorComponent.modlaId };
          this.bsModalRef = this.modalService.show(PatientEditorComponent, initialState);
          this.bsModalRef.content.onClose.subscribe(result => {
            if (result) {
              when(() => this.ts.patientStore.selected_patient != null,
                () => {
                  // Link
                  let link = new Entities.PatientLinkDto();
                  link.appointmentId = appointmentId;
                  link.patientId = this.ts.patientStore.selected_patient.id;
                  let patient = this.ts.patientStore.selected_patient;
                  link.firstName = patient.firstName;
                  link.initial = patient.initial;
                  link.lastName = patient.lastName;
                  link.email = patient.email;
                  link.phone = patient.mobilePhone;
                  this.ts.schedulerStore.linkPatientAppointment(link);
                  when(
                    () => this.ts.schedulerStore.inprogress == false,
                    () => {
                      let patientName = patient.firstName + ' ' + patient.lastName;
                      this.ts.appointmentStore.updatePatientName(patientName, appointmentId);
                      this.updateEvent.emit('A-' + appointmentId);
                      this.ts.appointmentStore.getById(Number(appointmentId));
                      when(() => this.ts.appointmentStore.inprogress == false,
                        () => {
                          this.bsApptModalRef = this.modalService.show(AppointmentEditorComponent, initialState);
                          this.bsApptModalRef.content.onClose.subscribe(result => {
                            if (result) {
                              this.updateEvent.emit('A-' + appointmentId);
                              this.onGetPatientAppointments();
                            }
                          });
                        });
                    });
                });
            }
          });
        }
        else {
          when(() => this.ts.patientStore.appointment_patient != null,
            () => {
              let patientName = this.ts.patientStore.appointment_patient.firstName + ' ' + this.ts.patientStore.appointment_patient.lastName;
              this.ts.appointmentStore.updatePatientName(patientName, appointmentId);
              this.updateEvent.emit('A-' + appointmentId);
            });
          this.ts.appointmentStore.getById(Number(appointmentId));
          when(() => this.ts.appointmentStore.inprogress == false,
            () => {
              this.bsApptModalRef = this.modalService.show(AppointmentEditorComponent, initialState);
              this.bsApptModalRef.content.onClose.subscribe(result => {
                if (result) {
                  this.updateEvent.emit('A-' + appointmentId);
                  this.onGetPatientAppointments();
                }
              });
            });
        }
      }
    });
  }

  gotoDate(appointmentId: number) {
    let appointment = _.find(this.ts.schedulerStore.patient_appointments, (x) => { return x.appointmentId == appointmentId });
    var date = DateTime.fromISO(appointment.date);
    this.schedulerGotoDate.emit(date);
  }

  onClose() {
    this.closeCenter.emit(true);
    this.ts.schedulerStore.patient_appointments = [];
  }
}
