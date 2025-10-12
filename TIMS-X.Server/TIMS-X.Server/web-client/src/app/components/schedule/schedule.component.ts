import { Component, ElementRef, EventEmitter, OnInit, OnDestroy, Output, ViewEncapsulation } from '@angular/core';
import { when } from "mobx";

import { Scheduler, SchedulerStatic } from "@app/dhtmlx/dhtmlxscheduler";

import { ScheduleStartUp } from './schedule.startup';

import { LocalStorageService } from '@app/services/local-storage.service';
import { TimsStore } from '@app/stores/tims.store';

@Component({
  encapsulation: ViewEncapsulation.None,
  selector: 'app-schedule',
  templateUrl: './schedule.component.html',
  styleUrl: './schedule.component.scss'
})
export class ScheduleComponent implements OnInit, OnDestroy {
  @Output() openCenter = new EventEmitter<boolean>()
  private scheduler: SchedulerStatic | null = null;
  public usesPatientAppointments: boolean = false;
  public blockLegend: string = 'block';
  public views = [
    { id: 'provider', name: "Provider" },
    { id: 'site', name: "Site" },
    { id: 'day', name: "Day" },
    { id: 'week', name: "Week" },
    { id: 'month', name: "Month" }
  ];
  public selectedView;

  constructor(
    private el: ElementRef,
    private scheduleStartup: ScheduleStartUp,
    private localStorageService: LocalStorageService,
    public ts: TimsStore) {
  }

  ngOnInit(): void {
    when(
      () => this.ts.practiceStore.businessRules != null,
      () => {
        if (this.ts.practiceStore.businessRules.onlineAppointmentPatientId > 0)
          this.usesPatientAppointments = true;
      });

    this.scheduler = Scheduler.getSchedulerInstance();
    //this.scheduler.config.date_format = '%Y-%m-%d %H:%i';
    const scheduler = this.scheduler;
    const container = this.el.nativeElement.querySelector('#scheduler');

    const timeFormat = 'h:mm a';
    scheduler.plugins({
      limit: true,
      html_templates: true,
      units: true,
      minical: true,
      tooltip: true,
      outerdrag: true,
      recurring: true
    });

    (function () {
      var old = scheduler.is_one_day_event;
      scheduler.is_one_day_event = function (ev) {
        if (ev.rec_type) return true;
        return old.call(this, ev);
      };
      var old_update_event = scheduler.updateEvent;
      scheduler.updateEvent = function (id) {
        var ev = scheduler.getEvent(id);
        if (ev && ev.rec_type && id.toString().indexOf('#') === -1) {
          scheduler.update_view();
        } else {
          old_update_event.call(this, id);
        }
      };
    })();


    if (this.ts.userstore.currentUser == null) {
      when(
        () => this.ts.userstore.inprogress == false,
        () => {
          this.scheduleStartup.InitializeSchedule(scheduler);
          scheduler.init(container, undefined, 'day');
          this.scheduleStartup.createMiniCalendars();
          this.setView();
        }
      );
    }
    else {
      this.scheduleStartup.InitializeSchedule(scheduler);
      scheduler.init(container, undefined, 'day');
      this.scheduleStartup.createMiniCalendars();
      this.setView();
    }
  }

  ngOnDestroy() {
    this.scheduler?.destructor();
    this.scheduler = null;
  }

  onPreviousMonth() {
    this.scheduleStartup.getPrevMonthForMiniCal();
  }

  onNextMonth() {
    this.scheduleStartup.getNextMonthForMiniCal();
  }

  public onViewSelected(event) {
    const view = event.target.value;
    this.scheduler.setCurrentView(null, view);
  }

  private setView() {
    let view = this.localStorageService.getItem('lastCalendarTimespan');
    if (view) {
      switch (view) {
        case 'day':
          this.selectedView = this.views[2].id;
          break;
        case 'week':
          this.selectedView = this.views[3].id;
          break;
        case 'month':
          this.selectedView = this.views[4].id;
          break;
        case 'provider':
          this.selectedView = this.views[0].id;
          break;
        case 'site':
          this.selectedView = this.views[1].id;
          break;
      }
      return;
    }

    if (this.ts.userstore.currentUser.user.lastCalendarTimespan == 2) {
      this.selectedView = this.views[3].id;
    }
    else if (this.ts.userstore.currentUser.user.lastCalendarTimespan == 3) {
      this.selectedView = this.views[4].id;
    }
    else {
      this.selectedView = this.views[2].id;
    }
  }

  onOpenCenter() {
    this.openCenter.emit(true);
  }

  gotoDate(date: Date) {
    this.scheduler.setCurrentView(date);
  }

  updateEvent(id: string) {
    this.scheduler.updateEvent(id);
  }
}
