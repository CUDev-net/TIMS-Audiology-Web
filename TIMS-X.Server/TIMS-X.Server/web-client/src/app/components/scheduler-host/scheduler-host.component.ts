import { Component, ViewChild } from '@angular/core';

import { ScheduleComponent } from '@app/components/schedule/schedule.component';

import { TimsStore } from '@app/stores/tims.store';


@Component({
  selector: 'app-scheduler-host',
  templateUrl: './scheduler-host.component.html',
  styleUrl: './scheduler-host.component.scss'
})
export class SchedulerHostComponent {
  @ViewChild('scheduleElement') scheduleElement: ScheduleComponent;
  public isPatientListVisible: boolean = true;
  public isPatientSummaryVisible: boolean = false;
  public isPatientAppointmentCenterVisible: boolean = false;
  public mobile: boolean = false;

  constructor(public ts: TimsStore) {
    if (window.screen.width < 500) { // 768px portrait
      this.mobile = true;
    }
  }

  onOpenCenter(event) {
    this.isPatientAppointmentCenterVisible = !this.isPatientAppointmentCenterVisible;
    if (this.isPatientAppointmentCenterVisible)
      this.ts.schedulerStore.getPatientAppointments();
  }

  onPatientSelected(id) {
    this.isPatientListVisible = false;
    this.isPatientSummaryVisible = true;
  }

  onSummaryCancel() {
    this.isPatientListVisible = true;
    this.isPatientSummaryVisible = false;
  }

  schedulerGotoDate(date: Date) {
    this.scheduleElement.gotoDate(date);
  }

  updateEvent(id: string) {
    this.scheduleElement.updateEvent(id);
  }
}
