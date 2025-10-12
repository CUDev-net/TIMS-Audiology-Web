import { Component, OnInit } from '@angular/core';
import { BsModalService, BsModalRef, ModalOptions } from 'ngx-bootstrap/modal';
import { Scheduler, SchedulerStatic } from "@app/dhtmlx/dhtmlxscheduler";
import * as _ from 'underscore';
import { when } from "mobx";

import { TimsStore } from '@app/stores/tims.store';

import { AppointmentEditorComponent } from "../appointment-editor/appointment-editor.component";
import { ScheduleEditorComponent } from "../schedule-editor/schedule-editor.component";

import { Entities } from '@app/entities/entities';

@Component({
  selector: 'app-search-results',
  templateUrl: './search-results.component.html',
  styleUrl: './search-results.component.scss'
})
export class SearchResultsComponent implements OnInit {
  constructor(public ts: TimsStore, private modalService: BsModalService) {
  }

  private scheduler: SchedulerStatic | null = null;
  private bsModalRef: BsModalRef;

  ngOnInit(): void {
    this.scheduler = Scheduler.getSchedulerInstance();

    document.addEventListener('dragover', (event) => {
      let e: any = event;
      if (e.target.parentElement.classList.contains("search-row")) {
        console.log('dragoverHandler');
        event.preventDefault();
      }
    }, false);

    document.addEventListener('drop', (event) => {
      let e: any = event;
      if (e.target.parentElement.classList.contains("search-row")) {
        console.log('dropHandler');
        this.ts.appointmentStore.new_appointment_summary = null; // Clear
        this.ts.appointmentStore.selected_appointment = new Entities.Appointment();
        this.ts.appointmentStore.selected_appointment.patientId = this.ts.patientStore.new_appointment_patientId;
        this.ts.appointmentStore.selected_appointment.id = 0;
        let attr = e.target.parentElement.attributes;
        let providerId = attr.getNamedItem('provider-id').value;
        this.ts.appointmentStore.selected_appointment.providerId = Number(providerId);
        let siteId = attr.getNamedItem('site-id').value;
        this.ts.appointmentStore.selected_appointment.siteId = Number(siteId);
        let startsAt = attr.getNamedItem('starts-at').value;
        this.ts.appointmentStore.selected_appointment.startsAt = new Date(startsAt);
        this.bsModalRef = this.modalService.show(AppointmentEditorComponent, Object.assign({}, { class: 'modal-lg' }));
        this.bsModalRef.content.onClose.subscribe(result => {
          if (result) {
            when(() => this.ts.appointmentStore.inprogress == false,
              () => {
                this.safeAddCalendarItem(this.ts.appointmentStore.new_appointment_summary);
              }
            );
          }
        });
        event.preventDefault();
      }
    }, false);
  }

  createSchedule(opening: Entities.ScheduleOpeningModel) {
    this.ts.scheduleStore.selected_schedule = new Entities.Schedule();
    this.ts.scheduleStore.selected_schedule.startsAt = opening.startsAt;
    this.ts.scheduleStore.selected_schedule.providerId = opening.providerId;
    this.ts.scheduleStore.selected_schedule.siteId = opening.siteId;
    this.ts.scheduleStore.selected_schedule.id = 0;
    let x = this.scheduler;
    const initialState: ModalOptions = { initialState: { x }, class: 'modal-lg', ignoreBackdropClick: true };
    this.bsModalRef = this.modalService.show(ScheduleEditorComponent, initialState);
    this.bsModalRef.content.onClose.subscribe(result => {
      if (result) {
        when(() => this.ts.scheduleStore.inprogress == false,
          () => {
            if (this.ts.scheduleStore.new_recurring_schedules != null) {
              _.each(this.ts.scheduleStore.new_recurring_schedules, (x) => {
                this.safeAddCalendarItem(x);
              });
            }
            else {
              _.each(this.ts.scheduleStore.new_schedules, (x) => {
                this.safeAddCalendarItem(x);
              });
            }
          }
        );
      }
    });
  }

  private safeAddCalendarItem(a) {
    let event = this.scheduler.getEvent(a.id);
    if (!event) {
      this.scheduler.addEvent(a);
    }
  }

  onCancel() {
    this.ts.appointmentStore.isSearchForOpenings = false;
    this.ts.appointmentStore.scheduleOpenings = null;
  }
}
