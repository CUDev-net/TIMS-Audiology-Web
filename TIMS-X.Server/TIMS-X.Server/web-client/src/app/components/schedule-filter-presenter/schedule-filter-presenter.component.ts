import { Component } from '@angular/core';
import { BsModalService, BsModalRef, ModalOptions } from 'ngx-bootstrap/modal';
import { when } from "mobx";
import * as _ from 'underscore';
import { DateTime, Duration } from "luxon";

import { TimsStore } from '@app/stores/tims.store';

import { ScheduleFilterComponent } from '@app/components/schedule-filter/schedule-filter.component';
import { ScheduleStartUp } from '@app/components/schedule/schedule.startup';

@Component({
  selector: 'app-schedule-filter-presenter',
  templateUrl: './schedule-filter-presenter.component.html',
  styleUrl: './schedule-filter-presenter.component.scss'
})
export class ScheduleFilterPresenterComponent {
  private bsModalRef: BsModalRef;
  public providerString;
  public siteString;
  public resourceString;
  public slpString;

  public get DisplayCalendarResources(): boolean {
    return this.ts.practiceStore.businessRules != null && this.ts.practiceStore.businessRules.usesCalendarResources;
  }

  public get StartDate() {
    return DateTime.fromJSDate(this.ts.appointmentStore.openingsStartDate).toLocaleString(DateTime.DATE_SHORT);
  }

  public get EndDate() {
    return DateTime.fromJSDate(this.ts.appointmentStore.openingsEndDate).toLocaleString(DateTime.DATE_SHORT);
  }

  public get Duration() {
    let x = this.ts.appointmentStore.openingsDuration;
    return Duration.fromObject({ minutes: x }).toFormat("h'h':m'm'");
  }

  constructor(private modalService: BsModalService, private scheduleStartup: ScheduleStartUp, public ts: TimsStore) {
    when(
      () => this.ts.userstore.currentUser != null,
      () => {
        this.parseProviderString();
        this.parseSiteString();
        this.parseResourceString();
        this.parseSLPString();
      });
  }

  parseProviderString() {
    if (this.ts.userstore.currentUser.user.scheduleProviderFilter == null || this.ts.userstore.currentUser.user.scheduleProviderFilter == '0') {
      this.providerString = 'All Providers';
    }
    else {
      let providerIds = this.ts.userstore.currentUser.user.scheduleProviderFilter.split('-');
      if (providerIds.length > 1) {
        this.providerString = 'Many Providers';
      }
      else {
        this.providerString = '1 Providers';
        var provider = _.find(this.ts.providerStore.provider_summary_list, s => s.id == Number(providerIds[0]))
        if (provider)
          this.providerString = provider.simpleName;
      }
    }
  }

  parseSiteString() {
    if (this.ts.userstore.currentUser.user.scheduleSiteFilter == null || this.ts.userstore.currentUser.user.scheduleSiteFilter == '0') {
      this.siteString = 'All Sites';
    }
    else {
      let siteIds = this.ts.userstore.currentUser.user.scheduleSiteFilter.split('-');
      if (siteIds.length > 1) {
        this.siteString = 'Many Sites';
      }
      else {
        this.siteString = '1 Site';
        var site = _.find(this.ts.siteStore.site_list, s => s.id == Number(siteIds[0]))
        if (site)
          this.siteString = site.name;
      }
    }
  }

  parseResourceString() {
    if (this.ts.userstore.currentUser.user.scheduleResourceFilter == null || this.ts.userstore.currentUser.user.scheduleResourceFilter == '0') {
      this.resourceString = 'All Resources';
    }
    else {
      let resourceIds = this.ts.userstore.currentUser.user.scheduleResourceFilter.split('-');
      if (resourceIds.length > 1) {
        this.resourceString = 'Many Resources';
      }
      else {
        this.resourceString = '1 Resource';
        var resource = _.find(this.ts.resourceStore.resource_list, s => s.id == Number(resourceIds[0]))
        if (resource)
          this.resourceString = resource.name;
      }
    }
  }

  parseSLPString() {
    if (this.ts.userstore.currentUser.user.scheduleSpecialtyFilter == null || this.ts.userstore.currentUser.user.scheduleSpecialtyFilter == '0') {
      this.slpString = '';
    }
    else {
      if (this.ts.userstore.currentUser.user.scheduleSpecialtyFilter == '1') {
        this.slpString = ' Audiology';
      }
      else {
        this.slpString = ' SLP';
      }
    }
  }

  onShowFilters() {
    if (this.ts.inProgress() || this.ts.userstore.currentUser == null || this.ts.providerStore.provider_w_Hours_list == null) return;

    const initialState: ModalOptions = { class: 'modal-lg', ignoreBackdropClick: true };
    this.bsModalRef = this.modalService.show(ScheduleFilterComponent, initialState);
    this.bsModalRef.content.onClose.subscribe(result => {
      if (result) {
        this.scheduleStartup.setFilters();
        when(
          () => this.ts.userstore.inprogress == false,
          () => {
            this.parseProviderString();
            this.parseSiteString();
            this.parseResourceString();
            this.parseSLPString();
            this.scheduleStartup.RefreshProviders();
            this.scheduleStartup.RefreshSites();
          });
      }
    });
  }
}
