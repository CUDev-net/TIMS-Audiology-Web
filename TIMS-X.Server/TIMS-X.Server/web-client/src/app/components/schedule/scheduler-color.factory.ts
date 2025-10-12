import { Injectable } from "@angular/core";
import * as _ from 'underscore';

import { TimsStore } from '@app/stores/tims.store';

@Injectable()
export class SchedulerColorFactory {
    constructor(private ts: TimsStore) {
    }

    public setAppointmentBackground(appointmentItem) {
        const defaultColor = '#333333';
        // 0 ApptType
        // 1 Site
        // 2 Resource
        switch (this.ts.userstore.currentUser.user.colorFrom) {
            case 0:
                appointmentItem.color = appointmentItem.appointment_type_web_color || defaultColor;
                break;
            case 1:
                appointmentItem.color = appointmentItem.site_web_color || defaultColor;
                break;
            // case 2:
            //     return this.resourceColor() || defaultColor;
            default:
                appointmentItem.color = defaultColor;
        }
        this._setForegroundColor(appointmentItem);
    }

    public setScheduleBackground(scheduleItem) {
        const defaultColor = '#FFFFFF';
        scheduleItem.color = scheduleItem.color_web ? scheduleItem.color_web : defaultColor;

        this._setForegroundColor(scheduleItem);
    }

    private _setForegroundColor(calendarItem) {
        let backgroundR = parseInt(calendarItem.color.substring(1, 2), 16),
            backgroundG = parseInt(calendarItem.color.substring(3, 2), 16),
            backgroundB = parseInt(calendarItem.color.substring(5, 2), 16);

        let backgroundDelta = (backgroundR * 0.03) + (backgroundG * 0.59) + (backgroundB * 0.11);

        calendarItem.textColor = backgroundDelta > 128 ? '#333333' : '#ffffff';
    }
}
