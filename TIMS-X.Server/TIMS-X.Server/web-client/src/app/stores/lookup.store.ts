import { Injectable } from '@angular/core';
import { makeObservable, observable, runInAction } from 'mobx';
import * as _ from 'underscore';

import { LookupService } from '../services/lookup.service';

import { Entities } from '@app/entities/entities';
import Gender = Entities.Sex;
import Ethnicity = Entities.Ethnicity;
import Race = Entities.Race;
import PreferredLanguage = Entities.PreferredLanguage;
import MarketingReferenceCategory = Entities.MarketingReferenceCategory;
import MarketingReference = Entities.MarketingReference;
import AppointmentStatus = Entities.AppointmentStatus;
import AppointmentType = Entities.AppointmentType;
import ScheduleBlock = Entities.ScheduleBlock;
import { BaseStore } from './base.store';

@Injectable()
export class LookupStore extends BaseStore {
    @observable public appointmentStatus_list: AppointmentStatus[] = null;
    @observable public appointmentType_list: AppointmentType[] = null
    @observable public gender_list: Gender[] = null
    @observable public ethnicity_list: Ethnicity[] = null
    @observable public race_list: Race[] = null
    @observable public preferred_language_list: PreferredLanguage[] = null
    @observable public marketcategory_list: MarketingReferenceCategory[] = null
    @observable public marketingphyscians_list: MarketingReference[] = null
    @observable public marketingReference_list: MarketingReference[] = null
    @observable public marketingReference_cache: MarketingReference = null;
    @observable public appointment_marketingReference_cache: MarketingReference = null;
    @observable public scheduleBlock_list: ScheduleBlock[] = null
    @observable public cityAndState: Entities.CityStateLookup = null;
    @observable public medicare_secondary_codes_list: Entities.MedicareSecondaryCodeDto[] = null
    @observable public patient_realionship_list: Entities.PatientRelationDto[] = null
    @observable public emplstatus_list: Entities.EmplStatus[] = null
    @observable public maritalstatus_list: Entities.MaritalStatus[] = null
    @observable public studentstatus_list: Entities.StudentStatus[] = null
    @observable public patienttypes_list: Entities.PatientType[] = null
    @observable public patientstatuses_list: Entities.PatientStatus[] = null
    @observable public authorization_list: Entities.Authorization[] = null
    @observable public communicationRestriction_list: Entities.CommunicationRestriction[] = null;
    @observable public descriptions: Entities.Description = null;

    public patient_required_fields: Entities.PatientRequiredFieldEnum[] = null

    constructor(private lookupService: LookupService) {
        super();
        makeObservable(this);
    }

    public get storesLoading() {
        return this.appointmentStatus_list == null ||
            this.appointmentType_list == null ||
            this.gender_list == null ||
            this.ethnicity_list == null ||
            this.race_list == null ||
            this.preferred_language_list == null ||
            this.marketcategory_list == null ||
            this.marketingphyscians_list == null ||
            this.scheduleBlock_list == null ||
            this.medicare_secondary_codes_list == null ||
            this.patient_realionship_list == null ||
            this.emplstatus_list == null ||
            this.maritalstatus_list == null ||
            this.studentstatus_list == null ||
            this.patienttypes_list == null ||
            this.patientstatuses_list == null ||
            this.authorization_list == null ||
            this.communicationRestriction_list == null
    }

    public getAll() {
        this.getEthnicities();
        this.getGenders();
        this.getLanguages();
        this.getRaces();
        this.getMarketCategories();
        this.getAppointmentStatuses();
        this.getAppointmentTypes();
        this.getScheduleBlocks();
        this.getPatientRequiredFields();
        this.getPatientMedciareSecondaryCodes();
        this.getPatientRelationships();
        this.getEmplStatuses();
        this.getMaritalStatuses();
        this.getStudentStatuses();
        this.getPatientTypes();
        this.getPatientStatuses();
        this.getAuthoriztions();
        this.getCommunicationRestrictions();
        this.getDescriptions();
    }

    public getAppointmentTypes() {

        this.lookupService.getAppointmentTypes().subscribe((p) => {
            this.appointmentType_list = _.sortBy(p, 'name');
        });
    }
    public getAppointmentStatuses() {
        this.lookupService.getAppointmentStatuses().subscribe((p) => {
            this.appointmentStatus_list = _.sortBy(p, 'name');
        });
    }

    public getGenders() {
        this.lookupService.getGenders().subscribe((p) => {
            this.gender_list = _.sortBy(p, 'name');
        });
    }

    public getLanguages() {
        this.lookupService.getLanguages().subscribe((p) => {
            this.preferred_language_list = p;
        });
    }

    public getRaces() {
        this.lookupService.getRaces().subscribe((p) => {
            this.race_list = p;
        });
    }

    public getEthnicities() {
        this.lookupService.getEthnicities().subscribe((p) => {
            runInAction(() => {
                this.ethnicity_list = p;
            });
        });
    }

    public getMarketCategories() {
        this.lookupService.getMarketCategories().subscribe((p) => {
            runInAction(() => {
                this.marketcategory_list = p;
                let category = _.find(p, (x: MarketingReferenceCategory) => { return x.id == 7; });
                this.marketingphyscians_list = _.sortBy(category.marketingReferences, 'name');
            });
        });
    }

    public getMarketingReferenceByCategory(categoryId: number) {
        this.marketingReference_list = null;
        this.lookupService.getMarketReferences(categoryId).subscribe((p) => {
            runInAction(() => {
                this.marketingReference_list = p;
            });
        });
    }

    public getMarketingReferenceToCache(id: number) {
        this.marketingReference_cache = null;
        this.lookupService.getMarketReferenceById(id).subscribe((p) => {
            runInAction(() => {
                this.marketingReference_cache = p;
            });
        });
    }

    public getMarketingAppointmentReferenceToCache(id: number) {
        this.appointment_marketingReference_cache = null;
        this.lookupService.getMarketReferenceById(id).subscribe((p) => {
            runInAction(() => {
                this.appointment_marketingReference_cache = p;
            });
        });
    }

    public getScheduleBlocks() {
        this.scheduleBlock_list = null;
        this.lookupService.getScheduleBlocks().subscribe((p) => {
            runInAction(() => {
                this.scheduleBlock_list = p;
            });
        });
    }

    public getCityAndStateFromZipCode(zipcode: string) {
        this.cityAndState = null;
        this.lookupService.getCityAndStateFromZipCode(zipcode).subscribe((p) => {
            this.cityAndState = p
        });
    }

    public getPatientRequiredFields() {
        this.patient_required_fields = null;
        this.lookupService.getPatientRequiredFields().subscribe((p) => {
            this.patient_required_fields = _.map(p, (x) => {
                return x.fieldId;
            });
        });
    }

    public getPatientMedciareSecondaryCodes() {
        this.medicare_secondary_codes_list = null;
        this.lookupService.getPatientMedicareSecondaryCodes().subscribe((p) => {
            this.medicare_secondary_codes_list = _.sortBy(p, 'name');
        });
    }

    public getPatientRelationships() {
        this.patient_realionship_list = null;
        this.lookupService.getPatientRelationships().subscribe((p) => {
            this.patient_realionship_list = _.sortBy(p, 'name');;
        });
    }

    public getEmplStatuses() {
        this.emplstatus_list = null;
        this.lookupService.getEmplStatuses().subscribe((p) => {
            this.emplstatus_list = _.sortBy(p, 'description');;
        });
    }

    public getMaritalStatuses() {
        this.maritalstatus_list = null;
        this.lookupService.getMaritalStatuses().subscribe((p) => {
            this.maritalstatus_list = _.sortBy(p, 'description');;
        });
    }

    public getStudentStatuses() {
        this.studentstatus_list = null;
        this.lookupService.getStudentStatuses().subscribe((p) => {
            this.studentstatus_list = _.sortBy(p, 'description');;
        });
    }

    public getPatientTypes() {
        this.patienttypes_list = null;
        this.lookupService.getPatientTypes().subscribe((p) => {
            this.patienttypes_list = _.sortBy(p, 'name');;
        });
    }

    public getPatientStatuses() {
        this.patientstatuses_list = null;
        this.lookupService.getPatientStatuses().subscribe((p) => {
            this.patientstatuses_list = _.sortBy(p, 'name');;
        });
    }

    public getAuthoriztions() {
        this.authorization_list = null;
        this.lookupService.getAuthorizations().subscribe((p) => {
            this.authorization_list = _.sortBy(p, 'name');;
        });
    }

    public getCommunicationRestrictions() {
        this.communicationRestriction_list = null;
        this.lookupService.getCommunicationRestrictions().subscribe((p) => {
            this.communicationRestriction_list = _.sortBy(p, 'name');;
        });
    }

    public getDescriptions() {
        this.descriptions = null;
        this.lookupService.getDescriptions().subscribe((p) => {
            this.descriptions = p;
        });
    }
}