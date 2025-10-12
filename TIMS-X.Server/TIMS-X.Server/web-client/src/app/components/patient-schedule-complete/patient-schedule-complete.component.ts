import { Component } from '@angular/core';
import { when } from "mobx";

import { TimsStore } from '@app/stores/tims.store';

@Component({
  selector: 'app-patient-schedule-complete',
  templateUrl: './patient-schedule-complete.component.html',
  styleUrl: './patient-schedule-complete.component.scss'
})
export class PatientScheduleCompleteComponent {
  constructor(public ts: TimsStore) {
    when(() => this.ts.patientScheduleStore.createdPatientAppointmentDto != null, () => {
      this.message = this.ts.patientScheduleStore.createdPatientAppointmentDto.message;
      this.practiceMessage = this.ts.patientScheduleStore.createdPatientAppointmentDto.practiceMessage;
      this.pendingMessage = this.ts.patientScheduleStore.createdPatientAppointmentDto.pendingMessage;
      this.emailMessage = this.ts.patientScheduleStore.createdPatientAppointmentDto.emailMessage;
    });
    this.practiceName = this.ts.patientScheduleStore.practice.name;
  }

  public logo;
  public message: string = null;
  public practiceMessage: string = null;
  public pendingMessage: string = null;
  public emailMessage: string = null;
  public practiceName: string;
}
