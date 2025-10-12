import { Component } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Subject } from 'rxjs';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import * as _ from 'underscore';
import { DateTime } from "luxon";

import { TimsStore } from '@app/stores/tims.store';

import { Entities } from '@app/entities/entities';
import Site = Entities.Site;
import ProviderSummary = Entities.ProviderSummary;
import AppointmentType = Entities.AppointmentType;


export class CheckedProvider {
  public provider: ProviderSummary;
  public isChecked: boolean;
}

export class CheckedSite {
  public site: Site;
  public isChecked: boolean;
}

export class CheckedResource {
  public resource: Entities.Resource;
  public isChecked: boolean;
}

@Component({
  selector: 'app-schedule-filter',
  templateUrl: './schedule-filter.component.html',
  styleUrls: ['./schedule-filter.component.scss']
})
export class ScheduleFilterComponent {
  public onClose: Subject<boolean>;
  public areAllProvidersChecked: boolean;
  public areAllSitesChecked: boolean;
  public areAllResourcesChecked: boolean;
  public areAllSpecialtiesChecked: boolean;
  public isAudiologyChecked: boolean;
  public isSlpChecked: boolean;
  public hasError: boolean = false;
  public error: string;
  public providerList: CheckedProvider[];
  public siteList: CheckedSite[];
  public resourceList: CheckedResource[];
  public isFindOpeningsSelected: boolean = false;
  public bsConfig?: Partial<BsDatepickerConfig>;
  public startdate: Date;
  public enddate: Date;
  public appointmentTypeId: number;
  public duration: number;

  constructor(
    public ts: TimsStore,
    public bsModalRef: BsModalRef
  ) { }


  ngOnInit(): void {
    this.startdate = DateTime.local().toJSDate();
    this.enddate = DateTime.local().plus({ days: 30 }).toJSDate();
    this.duration = 30;
    this.appointmentTypeId = null;

    this.bsConfig = Object.assign({}, { containerClass: 'theme-dark-blue', customTodayClass: 'tims-today-class', showWeekNumbers: false });
    this.onClose = new Subject();

    this.providerList = [];
    let providerIds = this.ts.userstore.currentUser.user.scheduleProviderFilter.split('-');
    _.each(this.ts.providerStore.provider_w_Hours_list, p => {
      let checkedProvider = new CheckedProvider();
      checkedProvider.provider = p;
      checkedProvider.isChecked = _.contains(providerIds, p.id.toString());
      this.providerList.push(checkedProvider);
    });
    if (providerIds[0] == '0') {
      this.areAllProvidersChecked = true;
      this.onAllProvidersChanged();
    }

    this.siteList = [];
    let siteIds = this.ts.userstore.currentUser.user.scheduleSiteFilter.split('-');
    _.each(this.ts.siteStore.site_list, p => {
      let checkedSite = new CheckedSite();
      checkedSite.site = p;
      checkedSite.isChecked = _.contains(siteIds, p.id.toString());
      this.siteList.push(checkedSite);
    });
    if (siteIds[0] == '0') {
      this.areAllSitesChecked = true;
      this.onAllSitesChanged();
    }

    this.loadResourcesForSite();
    let resourceIds = this.ts.userstore.currentUser.user.scheduleResourceFilter.split('-');
    if (resourceIds[0] == '0') {
      this.areAllResourcesChecked = true;
      this.onAllResourcesChanged();
    }
    else {
      _.each(this.resourceList, r => {
        r.isChecked = _.contains(resourceIds, r.resource.id.toString());
      });
    }

    let specialityIds = this.ts.userstore.currentUser.user.scheduleSpecialtyFilter.split('-');
    if (specialityIds[0] == '0') {
      this.areAllSpecialtiesChecked = true;
    }
    else if (specialityIds[0] == '1') {
      this.isAudiologyChecked = true;
    }
    else if (specialityIds[0] == '2') {
      this.isSlpChecked = true;
    }
    this.onAllSpecialityChanged();
  }

  onAllProvidersChanged() {
    if (this.areAllProvidersChecked)
      _.each(this.providerList, (p) => p.isChecked = true);
    else {
      let allChecked = _.every(this.providerList, (p: CheckedProvider) => { return p.isChecked; });
      if (allChecked)
        _.each(this.providerList, (p) => p.isChecked = false);
    }
  }

  onProviderChanged() {
    if (_.any(this.providerList, p => !p.isChecked))
      this.areAllProvidersChecked = false;
  }

  onAllSitesChanged() {
    if (this.areAllSitesChecked)
      _.each(this.siteList, (p) => p.isChecked = true);
    else {
      let allChecked = _.every(this.siteList, (p: CheckedSite) => { return p.isChecked; });
      if (allChecked)
        _.each(this.siteList, (p) => p.isChecked = false);
    }
    this.loadResourcesForSite();
  }

  onAllResourcesChanged() {
    if (this.areAllResourcesChecked)
      _.each(this.resourceList, (p) => p.isChecked = true);
    else {
      let allChecked = _.every(this.resourceList, (p: CheckedResource) => { return p.isChecked; });
      if (allChecked)
        _.each(this.resourceList, (p) => p.isChecked = false);
    }
  }

  onSiteChanged() {
    if (_.any(this.siteList, p => !p.isChecked))
      this.areAllSitesChecked = false;
    this.loadResourcesForSite();
  }

  onResourceChanged() {
    if (_.any(this.resourceList, p => !p.isChecked))
      this.areAllResourcesChecked = false;
  }

  onAppointmentTypeChanged() {
    if (this.appointmentTypeId) {
      let at_id = this.appointmentTypeId;
      let at = _.find(this.ts.lookupStore.appointmentType_list, (x) => { return x.id == at_id; });
      if (at) {
        this.duration = at.duration;
      }
    }
  }

  onAllSpecialityChanged() {
    if (this.areAllSpecialtiesChecked) {
      this.isAudiologyChecked = true;
      this.isSlpChecked = true;
    }
  }

  onSpecialityChanged() {
    if (!this.isAudiologyChecked || !this.isSlpChecked)
      this.areAllSpecialtiesChecked = false;
  }

  loadResourcesForSite() {
    if (!this.ts.practiceStore.businessRules.usesCalendarResources) return;

    this.resourceList = [];
    let checkedSites = _.filter(this.siteList, s => s.isChecked);
    if (checkedSites.length == 1) {
      let siteId = checkedSites[0].site.id;
      this.resourceList = this.ts.resourceStore.getResourcesForSite(siteId).map(r => {
        let checkedResource = new CheckedResource();
        checkedResource.resource = r;
        checkedResource.isChecked = false;
        return checkedResource;
      });
    }
    else {
      this.areAllResourcesChecked = true
    }
  }

  onSubmit() {
    // Validate one provider checked
    if (!this.areAllProvidersChecked && !_.any(this.providerList, p => p.isChecked)) {
      this.hasError = true;
      this.error = 'Please select at least 1 provider';
      return;
    }

    if (!this.areAllSitesChecked && !_.any(this.siteList, p => p.isChecked)) {
      this.hasError = true;
      this.error = 'Please select at least 1 site';
      return;
    }

    if (!this.areAllResourcesChecked && !_.any(this.resourceList, p => p.isChecked)) {
      this.areAllResourcesChecked = true;
    }

    if (this.isFindOpeningsSelected) {
      if (!this.startdate) {
        this.hasError = true;
        this.error = 'Please select a start date';
        return;
      }
      if (!this.enddate) {
        this.hasError = true;
        this.error = 'Please select an end date';
        return;
      }
      if (this.duration < 1) {
        this.hasError = true;
        this.error = 'Duration must be greater than 0';
        return;
      }
      let searchModel = new Entities.ScheduleOpeningsSearchModel();
      searchModel.startDate = this.startdate;
      searchModel.endDate = this.enddate;
      searchModel.providerIds = [];
      searchModel.siteIds = [];
      _.each(this.providerList, p => {
        if (p.isChecked) {
          searchModel.providerIds.push(p.provider.id);
        }
      });
      _.each(this.siteList, p => {
        if (p.isChecked) {
          searchModel.siteIds.push(p.site.id);
        }
      });
      if (this.appointmentTypeId)
        searchModel.appointmentTypeId = Number(this.appointmentTypeId);
      searchModel.durationTotalMinutes = this.duration;
      this.ts.appointmentStore.searchForOpenings(searchModel);
      this.ts.appointmentStore.openingsAppointmentTypeId = searchModel.appointmentTypeId;
      this.ts.appointmentStore.openingsDuration = this.duration;
      this.ts.appointmentStore.openingsStartDate = this.startdate;
      this.ts.appointmentStore.openingsEndDate = this.enddate;
      // Don't refresh calendar
      this.onClose.next(false);
      this.bsModalRef.hide();

    }
    else {
      if (this.areAllProvidersChecked)
        this.ts.userstore.currentUser.user.scheduleProviderFilter = "0";
      else {
        let providerIds = "";
        _.each(this.providerList, p => {
          if (p.isChecked) {
            providerIds = providerIds + p.provider.id.toString() + "-";
          }
        });
        this.ts.userstore.currentUser.user.scheduleProviderFilter = providerIds.slice(0, -1);
      }
      if (this.areAllSitesChecked)
        this.ts.userstore.currentUser.user.scheduleSiteFilter = "0";
      else {
        let siteIds = "";
        _.each(this.siteList, s => {
          if (s.isChecked) {
            siteIds = siteIds + s.site.id.toString() + "-";
          }
        });
        this.ts.userstore.currentUser.user.scheduleSiteFilter = siteIds.slice(0, -1);
      }
      if (this.areAllResourcesChecked)
        this.ts.userstore.currentUser.user.scheduleResourceFilter = "0";
      else {
        let resourceIds = "";
        _.each(this.resourceList, s => {
          if (s.isChecked) {
            resourceIds = resourceIds + s.resource.id.toString() + "-";
          }
        });
        this.ts.userstore.currentUser.user.scheduleResourceFilter = resourceIds.slice(0, -1);
      }

      if (this.areAllSpecialtiesChecked || (this.isAudiologyChecked && this.isSlpChecked)) {
        this.ts.userstore.currentUser.user.scheduleSpecialtyFilter = "0";
      }
      else if (this.isAudiologyChecked) {
        this.ts.userstore.currentUser.user.scheduleSpecialtyFilter = "1";
      }
      else if (this.isSlpChecked) {
        this.ts.userstore.currentUser.user.scheduleSpecialtyFilter = "2";
      }
      else {
        this.ts.userstore.currentUser.user.scheduleSpecialtyFilter = "0";
      }
      this.ts.userstore.update(this.ts.userstore.currentUser);
      this.ts.appointmentStore.isSearchForOpenings = false;
      this.onClose.next(true);
      this.bsModalRef.hide();
    }
  }

  handlesFindOpeningsClick() {
    this.isFindOpeningsSelected = true;
  }

  handlesSelectFilterClick() {
    this.isFindOpeningsSelected = false;
  }

  onCancel() {
    this.onClose.next(null);
    this.bsModalRef.hide();
  }
}
