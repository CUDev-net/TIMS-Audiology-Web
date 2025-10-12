import { Component } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { DateTime } from "luxon";
import { TimsStore } from '@app/stores/tims.store';

import { Entities } from '@app/entities/entities';

@Component({
  selector: 'app-mobile-schedule-dialog',
  templateUrl: './mobile-schedule-dialog.component.html',
  styleUrl: './mobile-schedule-dialog.component.scss'
})
export class MobileScheduleDialogComponent {
  scheduleItem: Entities.ScheduleItem;
  private timeFormat = 'h:mm a';

  public get Patient() {
    return this.ts.patientStore.selected_patient_summary;
  }

  constructor(public bsModalRef: BsModalRef, public ts: TimsStore) {
  }

  public get displayDate(): string {
    return DateTime.fromJSDate(this.scheduleItem.start_date).toFormat(this.timeFormat) + ' - ' + DateTime.fromJSDate(this.scheduleItem.end_date).toFormat(this.timeFormat);
  }

  handlesClose() {
    this.bsModalRef.hide();
  }
}
