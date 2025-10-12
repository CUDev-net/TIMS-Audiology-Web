import { AfterViewInit, ChangeDetectorRef, Component, Input } from '@angular/core';
import { DateTime } from "luxon";
import { BsModalService, BsModalRef, ModalOptions } from 'ngx-bootstrap/modal';

import { MobileScheduleDialogComponent } from '@app/components/mobile-schedule-dialog/mobile-schedule-dialog.component';

import { TimsStore } from '@app/stores/tims.store';

import { Entities } from '@app/entities/entities';

@Component({
  selector: 'app-mobile-schedule-item',
  templateUrl: './mobile-schedule-item.component.html',
  styleUrl: './mobile-schedule-item.component.scss'
})
export class MobileScheduleItemComponent {
  @Input() scheduleItem: Entities.ScheduleItem;
  private bsModalRef: BsModalRef;

  constructor(private modalService: BsModalService, private changeDetector: ChangeDetectorRef, private ts: TimsStore) {
  }
  ngAfterViewInit() {
    this.changeDetector.detectChanges();
  }

  private timeFormat = 'h:mm a';

  public get displayDate(): string {
    return DateTime.fromJSDate(this.scheduleItem.start_date).toFormat(this.timeFormat) + ' - ' + DateTime.fromJSDate(this.scheduleItem.end_date).toFormat(this.timeFormat);
  }

  public get BackgroundColor(): string {
    const defaultColor = '#333333';
    let calendarItemColor = defaultColor;
    if (this.scheduleItem.type === 'A') {
      switch (this.ts.userstore.currentUser.user.colorFrom) {
        case 0:
          calendarItemColor = this.scheduleItem.appointment_type_web_color || defaultColor;
          break;
        case 1:
          calendarItemColor = this.scheduleItem.site_web_color || defaultColor;
          break;
        // case 2:
        //     return this.resourceColor() || defaultColor;
        default:
          calendarItemColor = defaultColor;
      }
    }
    else if(this.scheduleItem.background_color) {
      calendarItemColor = this.scheduleItem.background_color;
    }
    else {
      calendarItemColor = '#ffffff';
    }

    let backgroundR = parseInt(calendarItemColor.substring(1, 2), 16),
      backgroundG = parseInt(calendarItemColor.substring(3, 2), 16),
      backgroundB = parseInt(calendarItemColor.substring(5, 2), 16);

    let backgroundDelta = (backgroundR * 0.03) + (backgroundG * 0.59) + (backgroundB * 0.11);

    this.ForegroundColor = backgroundDelta > 128 ? '#333333' : '#ffffff';
    return calendarItemColor
  }

  public ForegroundColor: string;

  public get subject(): string {
    if (this.scheduleItem.type === 'A') {
      return this.scheduleItem.patient_name;
    } else {
      return this.scheduleItem.title;
    }
  }

  public get text(): string {
    if (this.scheduleItem.type === 'A') {
      return `: ${this.scheduleItem.appointment_type_name} at ${this.scheduleItem.site_name} with ${this.scheduleItem.provider_name}`;
    }
    else {
      return ` at ${this.scheduleItem.site_name} with ${this.scheduleItem.provider_name}`;
    }
  }

  handlesViewSummary() {
    if (this.scheduleItem.type === 'A')
      this.ts.patientStore.getSummary(this.scheduleItem.patientId);
    let scheduleItem = this.scheduleItem;
    const initialState: ModalOptions = { initialState: { scheduleItem } };
    this.bsModalRef = this.modalService.show(MobileScheduleDialogComponent, initialState);
  }
}
