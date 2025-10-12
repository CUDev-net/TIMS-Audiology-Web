import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { CookieService } from 'ngx-cookie-service';
import { LuxonModule } from 'luxon-angular';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { ModalModule, BsModalService } from 'ngx-bootstrap/modal';
import { ColorPickerModule } from 'ngx-color-picker';
import { LoadingBarHttpClientModule } from '@ngx-loading-bar/http-client';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { PopoverModule } from 'ngx-bootstrap/popover';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { RecaptchaModule, RecaptchaFormsModule } from 'ng-recaptcha';

import { ScheduleStartUp } from '@app/components/schedule/schedule.startup';
import { SchedulerColorFactory } from '@app/components/schedule/scheduler-color.factory';
import { TimeSpanFactory } from '@app/components/schedule/timespan.factory';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { SchedulerHostComponent } from './components/scheduler-host/scheduler-host.component';

import { AppointmentStore } from './stores/appointment.store';
import { LookupStore } from './stores/lookup.store';
import { PatientStore } from './stores/patient.store';
import { ProviderStore } from './stores/provider.store';
import { PracticeStore } from './stores/practice.store';
import { ResourceStore } from './stores/resource.store';
import { SchedulerStore } from './stores/scheduler.store';
import { ScheduleStore } from './stores/schedule.store';
import { SiteStore } from './stores/site.store';
import { TimsStore } from './stores/tims.store';
import { UserStore } from './stores/user.store';
import { PatientScheduleStore } from './stores/patient-schedule.store';
import { InsurancePayerStore } from './stores/insurance-payer.store';
import { PatientInsuranceStore } from './stores/patient-insurance.store';
import { ApptAuthorizationStore } from './stores/appt-authorization.store';


import { PatientNameFormatterComponent } from './components/patient-name-formatter/patient-name-formatter.component';
import { PatientNameDirective } from './directives/patientname.directive';
import { NumberOnlyDirective } from './directives/numberonly.directive';
import { BirthDateDirective } from './directives/birthdate.directive';
import { PhoneMaskDirective } from './directives/phonemask.directive';

import { AuthInterceptor } from './guards/authInterceptor';

import { AppointmentEditorComponent } from './components/appointment-editor/appointment-editor.component';
import { ScheduleFilterPresenterComponent } from './components/schedule-filter-presenter/schedule-filter-presenter.component';
import { PatientSelectorComponent } from './components/patient-selector/patient-selector.component';
import { PatientListComponent } from './components/patient-list/patient-list.component';
import { ScheduleComponent } from './components/schedule/schedule.component';
import { PatientSummaryComponent } from './components/patient-summary/patient-summary.component';
import { AddressFormatterComponent } from './components/address-formatter/address-formatter.component';
import { PatientEditorComponent } from './components/patient-editor/patient-editor.component';
import { NameEntryComponent } from './components/name-entry/name-entry.component';
import { AddressEntryComponent } from './components/address-entry/address-entry.component';
import { AddressEntry2Component } from './components/address-entry-2/address-entry-2.component';
import { ShowErrorsComponent } from './components/show-errors/show-errors.component';
import { ScheduleEditorComponent } from './components/schedule-editor/schedule-editor.component';
import { TimeEntryComponent } from './components/time-entry/time-entry.component';
import { ValidationResultsComponent } from './components/validation-results/validation-results.component';
import { DurationEntryComponent } from './components/duration-entry/duration-entry.component';
import { ScheduleFilterComponent } from './components/schedule-filter/schedule-filter.component';
import { RecurrenceEditorComponent } from './components/recurrence-editor/recurrence-editor.component';
import { SearchResultsComponent } from './components/search-results/search-results.component';
import { MobileScheduleComponent } from './components/mobile-schedule/mobile-schedule.component';
import { MobileScheduleItemComponent } from './components/mobile-schedule-item/mobile-schedule-item.component';
import { MobileScheduleDialogComponent } from './components/mobile-schedule-dialog/mobile-schedule-dialog.component';
import { PatientScheduleComponent } from './components/patient-schedule/patient-schedule.component';
import { PatientScheduleCompleteComponent } from './components/patient-schedule-complete/patient-schedule-complete.component';
import { PatientAppointmentCenterComponent } from './components/patient-appointment-center/patient-appointment-center.component';
import { LinkAppointmentDialogComponent } from './components/link-appointment-dialog/link-appointment-dialog.component';
import { InsuranceEntryComponent } from './components/insurance-entry/insurance-entry.component';
import { AuthorizationEditorComponent } from './components/authorization-editor/authorization-editor.component';

@NgModule({
  declarations: [
    AppComponent,
    AppointmentEditorComponent,
    AddressEntryComponent,
    AddressEntry2Component,
    AddressFormatterComponent,
    NameEntryComponent,
    NumberOnlyDirective,
    PatientNameDirective,
    PatientEditorComponent,
    BirthDateDirective,
    PatientListComponent,
    PatientNameFormatterComponent,
    PatientSelectorComponent,
    PatientSummaryComponent,
    PhoneMaskDirective,
    ScheduleComponent,
    ScheduleEditorComponent,
    ScheduleFilterComponent,
    ScheduleFilterPresenterComponent,
    SchedulerHostComponent,
    ShowErrorsComponent,
    TimeEntryComponent,
    ValidationResultsComponent,
    DurationEntryComponent,
    RecurrenceEditorComponent,
    SearchResultsComponent,
    MobileScheduleComponent,
    MobileScheduleItemComponent,
    MobileScheduleDialogComponent,
    PatientScheduleComponent,
    PatientScheduleCompleteComponent,
    PatientAppointmentCenterComponent,
    LinkAppointmentDialogComponent,
    InsuranceEntryComponent,
    AuthorizationEditorComponent
  ],
  imports: [
    BrowserAnimationsModule,
    BrowserModule,
    ColorPickerModule,
    FormsModule,
    LuxonModule,
    ModalModule,
    ReactiveFormsModule,
    HttpClientModule,
    AppRoutingModule,
    LoadingBarHttpClientModule,
    BsDatepickerModule.forRoot(),
    TabsModule.forRoot(),
    NgMultiSelectDropDownModule.forRoot(),
    PopoverModule.forRoot(),
    BsDropdownModule.forRoot(),
    RecaptchaModule,
    RecaptchaFormsModule
  ],
  providers: [
    // { provide: APP_BASE_HREF, useValue: '/scheduler' },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    },
    BsModalService,
    AppointmentStore,
    ApptAuthorizationStore,
    InsurancePayerStore,
    LookupStore,
    PatientStore,
    PatientScheduleStore,
    PracticeStore,
    ProviderStore,
    ResourceStore,
    ScheduleStore,
    SchedulerStore,
    SiteStore,
    TimsStore,
    UserStore,
    SchedulerColorFactory,
    ScheduleStartUp,
    TimeSpanFactory,
    CookieService,
    PatientInsuranceStore
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
