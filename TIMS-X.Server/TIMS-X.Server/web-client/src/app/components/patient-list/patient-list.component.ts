import { Component, EventEmitter, Input, Output } from '@angular/core';

import { TimsStore } from '@app/stores/tims.store';


@Component({
  selector: 'app-patient-list',
  templateUrl: './patient-list.component.html',
  styleUrl: './patient-list.component.scss'
})
export class PatientListComponent {
  @Output() patientSelected = new EventEmitter<number>();
  @Input() isCensus: boolean = false;
  @Input() header: string;

  public get PatientSummaries() {
    if (this.isCensus)
      return this.ts.patientStore.censusPatients;
    else
      return this.ts.userstore.currentUser.lastPatientSummaries;
  }

  constructor(public ts: TimsStore) {
  }

  selectPatient(id: number) {
    this.ts.patientStore.censusPatients = null;
    this.ts.patientStore.new_appointment_patientId = id;
    this.ts.patientStore.getSummary(id);
    this.patientSelected.emit(id);
  }

  cachePatientId(id: number) {
    this.ts.patientStore.new_appointment_patientId = id;
  }
}
