import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { EnvironmentUrlService } from '../services/environment-url.service';

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

@Injectable({
    providedIn: 'root'
})
export class LookupService {

    constructor(private http: HttpClient, private envUrl: EnvironmentUrlService) { }

    public getAppointmentTypes() {
        let route = `/web/lookup/get-appointment-types`
        return this.http.get<AppointmentType[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public getAppointmentStatuses() {
        let route = `/web/lookup/get-appointment-statuses`
        return this.http.get<AppointmentStatus[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public getEthnicities() {
        let route = `/web/lookup/get-patient-ethnicities`
        return this.http.get<Ethnicity[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public getGenders() {
        let route = `/web/lookup/get-patient-genders`
        return this.http.get<Gender[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public getLanguages() {
        let route = `/web/lookup/get-patient-languages`
        return this.http.get<PreferredLanguage[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }
    public getRaces() {
        let route = `/web/lookup/get-patient-races`
        return this.http.get<Race[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public getMarketCategories() {
        let route = `/web/lookup/get-marketing-categories`
        return this.http.get<MarketingReferenceCategory[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public getMarketReferences(categoryId: number) {
        let route = `/web/lookup/get-marketing-references?marketingCategoryId=${categoryId}`
        return this.http.get<MarketingReference[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public getMarketReferenceById(id: number) {
        let route = `/web/lookup/get-marketing-reference?id=${id}`
        return this.http.get<MarketingReference>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public getScheduleBlocks() {
        let route = `/web/lookup/get-schedule-blocks`
        return this.http.get<ScheduleBlock[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public getCityAndStateFromZipCode(zipCode: string) {
        let route = `/web/lookup/lookup-city-state?zipcode=${zipCode}`
        return this.http.get<Entities.CityStateLookup>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public getPatientRequiredFields() {
        let route = `/web/lookup/get-patient-required-fields`
        return this.http.get<Entities.PatientRequiredField[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public getPatientRelationships() {
        let route = `/web/lookup/get-patient-relationships`
        return this.http.get<Entities.PatientRelationDto[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public getPatientMedicareSecondaryCodes() {
        let route = `/web/lookup/get-medicare-secondary-codes`
        return this.http.get<Entities.MedicareSecondaryCodeDto[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public getEmplStatuses() {
        let route = `/web/lookup/get-empl-statuses`
        return this.http.get<Entities.EmplStatus[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }
    public getMaritalStatuses() {
        let route = `/web/lookup/get-marital-statuses`
        return this.http.get<Entities.MaritalStatus[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }
    public getStudentStatuses() {
        let route = `/web/lookup/get-student-statuses`
        return this.http.get<Entities.StudentStatus[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public getPatientTypes() {
        let route = `/web/patienttype/get-all`
        return this.http.get<Entities.PatientType[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public getPatientStatuses() {
        let route = `/web/patientstatus/get-all`
        return this.http.get<Entities.PatientStatus[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public getAuthorizations() {
        let route = `/web/authorization/get-all`
        return this.http.get<Entities.Authorization[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public getCommunicationRestrictions() {
        let route = `/web/CommunicationRestriction/get-all`
        return this.http.get<Entities.CommunicationRestriction[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public getDescriptions() {
        let route = `/web/lookup/get-descriptions`
        return this.http.get<Entities.Description>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    private createCompleteRoute = (route: string, envAddress: string) => {
        return `${envAddress}${route}`;
    }
}