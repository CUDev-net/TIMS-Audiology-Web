import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ValidationErrors } from '@angular/forms';
import { Subject } from 'rxjs';
import { BsModalService, BsModalRef, ModalOptions } from 'ngx-bootstrap/modal';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import * as _ from 'underscore';
import { when } from "mobx";
import { IDropdownSettings } from 'ng-multiselect-dropdown';

import { TimsStore } from '@app/stores/tims.store';
import { StringUtils } from '@app/utilities/string-utils';
import { CustomValidators } from '@app/components/custom.validators';

import { NameEntryComponent } from '@app/components/name-entry/name-entry.component';
import { AddressEntry2Component } from '@app/components/address-entry-2/address-entry-2.component';
import { InsuranceEntryComponent } from '@app/components/insurance-entry/insurance-entry.component';
import { AuthorizationEditorComponent } from '@app/components/authorization-editor/authorization-editor.component';

import { Entities } from '@app/entities/entities';
import Patient = Entities.Patient;
import { DateUtils } from '@app/helpers/date-utils';


@Component({
  selector: 'app-patient-editor',
  templateUrl: './patient-editor.component.html',
  styleUrl: './patient-editor.component.scss'
})
export class PatientEditorComponent implements AfterViewInit, OnInit {
  @ViewChild(NameEntryComponent) nameEntry: NameEntryComponent;
  @ViewChild(AddressEntry2Component) addressEntry2: AddressEntry2Component;
  @ViewChild('primary', { static: true }) primaryInsuranceEntryComponent: InsuranceEntryComponent;
  @ViewChild('secondary') secondaryInsuranceEntryComponent: InsuranceEntryComponent;
  @ViewChild('tertiary') tertiaryInsuranceEntryComponent: InsuranceEntryComponent;
  public onClose: Subject<boolean>;
  public patientForm: FormGroup;
  bsConfig?: Partial<BsDatepickerConfig>;
  public isSecondaryAddressChecked: boolean;
  public currentPatient: Patient;
  public isdetalsselected: boolean = true;
  private useSecondaryAddress: boolean = false;
  public multiSelectDropdownSettings: IDropdownSettings = {};
  public marketingauthorizationDisabled: boolean = true;
  public communicationRestrictions: Entities.CommunicationRestriction[];
  public appointmentAuthorizations: Entities.ApptAuthorization[] = [];
  public customtext1label: string = 'Custom Text 1';
  public customtext2label: string = 'Custom Text 2';
  public customdate1label: string = 'Custom Date 1';
  public customdate2label: string = 'Custom Date 2';
  private showInactiveApptAuthorizations: boolean = false;

  static modlaId: number = 8001;

  public phones: Entities.EnumPair[] = [
    { display: 'Home', value: 1 },
    { display: 'Work', value: 2 },
    { display: 'Mobile', value: 3 },
    { display: 'Other', value: 4 }
  ];

  constructor(private formBuilder: FormBuilder,
    public bsModalRef: BsModalRef,
    private modalService: BsModalService,
    public ts: TimsStore) {
  }

  get f() { return this.patientForm.controls; }

  isNew() {
    return this.currentPatient.id == null || this.currentPatient.id <= 0;
  }

  public birthDateRequired: ValidationErrors = null;
  public middleNameRequired: ValidationErrors = null;
  public languageRequired: ValidationErrors = null;
  public genderRequired: ValidationErrors = null;
  public raceRequired: ValidationErrors = null;
  public ethnicityRequired: ValidationErrors = null;
  public seenbyProviderRequired: ValidationErrors = null;
  public marketingRequired: ValidationErrors = null;
  public addressLine1Required: ValidationErrors = null;
  public addressLine2Required: ValidationErrors = null;
  public cityRequired: ValidationErrors = null;
  public stateRequired: ValidationErrors = null;
  public zipCodeRequired: ValidationErrors = null;
  public homePhoneRequired: ValidationErrors = null;
  public workPhoneRequired: ValidationErrors = null;
  public mobilePhoneRequired: ValidationErrors = null;
  public otherPhoneRequired: ValidationErrors = null;
  public emailRequired: ValidationErrors = null;

  public alternateContactReqired: ValidationErrors = null;
  public altphoneRequired: ValidationErrors = null;
  public selectedPatientTypesRequired: ValidationErrors = null;
  public primaryphysicianidRequired: ValidationErrors = null;
  public referringphysicianidRequired: ValidationErrors = null;

  public releasesignaturedateRequired: ValidationErrors = null;
  public assignmentdateRequired: ValidationErrors = null;
  public privacydateRequired: ValidationErrors = null;
  public consentdateRequired: ValidationErrors = null;

  ngAfterViewInit(): void {
    this.setSecondaryAddressState();

    let comm = _.filter(this.ts.lookupStore.communicationRestriction_list, (x: Entities.CommunicationRestriction) => x.name.toLowerCase().includes('notifications'));
    this.communicationRestrictions = comm.concat(_.filter(this.ts.lookupStore.communicationRestriction_list, (x: Entities.CommunicationRestriction) => !x.name.toLowerCase().includes('notifications')));
  }

  ngOnInit(): void {
    this.multiSelectDropdownSettings = {
      singleSelection: false,
      idField: 'id',
      textField: 'name',
      itemsShowLimit: 1,
      unSelectAllText: 'UnSelect All',
      allowSearchFilter: false
    };

    this.ts.lookupStore.marketingReference_cache = null;
    this.ts.lookupStore.marketingReference_list = null;
    this.birthDateRequired = _.find(this.ts.lookupStore.patient_required_fields, (p) => { return Number(Entities.PatientRequiredFieldEnum.dob) == p; }) ? Validators.required : null;
    this.middleNameRequired = _.find(this.ts.lookupStore.patient_required_fields, (p) => { return Number(Entities.PatientRequiredFieldEnum.initial) == p; }) ? Validators.required : null;
    this.languageRequired = _.find(this.ts.lookupStore.patient_required_fields, (p) => { return Number(Entities.PatientRequiredFieldEnum.language) == p; }) ? Validators.required : null;
    this.genderRequired = _.find(this.ts.lookupStore.patient_required_fields, (p) => { return Number(Entities.PatientRequiredFieldEnum.gender) == p; }) ? Validators.required : null;
    this.raceRequired = _.find(this.ts.lookupStore.patient_required_fields, (p) => { return Number(Entities.PatientRequiredFieldEnum.race) == p; }) ? Validators.required : null;
    this.ethnicityRequired = _.find(this.ts.lookupStore.patient_required_fields, (p) => { return Number(Entities.PatientRequiredFieldEnum.ethnicity) == p; }) ? Validators.required : null;
    this.seenbyProviderRequired = _.find(this.ts.lookupStore.patient_required_fields, (p) => { return Number(Entities.PatientRequiredFieldEnum.seenBy) == p; }) ? Validators.required : null;
    this.marketingRequired = this.ts.practiceStore.businessRules.requireMarketingSourceForPatientAppointment ? Validators.required : null;

    this.addressLine1Required = _.find(this.ts.lookupStore.patient_required_fields, (p) => { return Number(Entities.PatientRequiredFieldEnum.address1) == p; }) ? Validators.required : null;
    this.addressLine2Required = _.find(this.ts.lookupStore.patient_required_fields, (p) => { return Number(Entities.PatientRequiredFieldEnum.address2) == p; }) ? Validators.required : null;
    this.cityRequired = _.find(this.ts.lookupStore.patient_required_fields, (p) => { return Number(Entities.PatientRequiredFieldEnum.city) == p; }) ? Validators.required : null;
    this.stateRequired = _.find(this.ts.lookupStore.patient_required_fields, (p) => { return Number(Entities.PatientRequiredFieldEnum.state) == p; }) ? Validators.required : null;
    this.zipCodeRequired = _.find(this.ts.lookupStore.patient_required_fields, (p) => { return Number(Entities.PatientRequiredFieldEnum.zip) == p; }) ? Validators.required : null;
    this.homePhoneRequired = _.find(this.ts.lookupStore.patient_required_fields, (p) => { return Number(Entities.PatientRequiredFieldEnum.homePhone) == p; }) ? Validators.required : null;
    this.workPhoneRequired = _.find(this.ts.lookupStore.patient_required_fields, (p) => { return Number(Entities.PatientRequiredFieldEnum.workPhone) == p; }) ? Validators.required : null;
    this.mobilePhoneRequired = _.find(this.ts.lookupStore.patient_required_fields, (p) => { return Number(Entities.PatientRequiredFieldEnum.mobilePhone) == p; }) ? Validators.required : null;
    this.otherPhoneRequired = _.find(this.ts.lookupStore.patient_required_fields, (p) => { return Number(Entities.PatientRequiredFieldEnum.otherPhone) == p; }) ? Validators.required : null;
    this.emailRequired = _.find(this.ts.lookupStore.patient_required_fields, (p) => { return Number(Entities.PatientRequiredFieldEnum.email) == p; }) ? Validators.required : null;

    this.alternateContactReqired = _.find(this.ts.lookupStore.patient_required_fields, (p) => { return Number(Entities.PatientRequiredFieldEnum.alternateContact) == p; }) ? Validators.required : null;
    this.altphoneRequired = _.find(this.ts.lookupStore.patient_required_fields, (p) => { return Number(Entities.PatientRequiredFieldEnum.alternatePhone) == p; }) ? Validators.required : null;
    this.selectedPatientTypesRequired = _.find(this.ts.lookupStore.patient_required_fields, (p) => { return Number(Entities.PatientRequiredFieldEnum.patientType) == p; }) ? Validators.required : null;
    this.primaryphysicianidRequired = _.find(this.ts.lookupStore.patient_required_fields, (p) => { return Number(Entities.PatientRequiredFieldEnum.primaryCare) == p; }) ? Validators.required : null;
    this.referringphysicianidRequired = _.find(this.ts.lookupStore.patient_required_fields, (p) => { return Number(Entities.PatientRequiredFieldEnum.referringPhysician) == p; }) ? Validators.required : null;
    this.releasesignaturedateRequired = _.find(this.ts.lookupStore.patient_required_fields, (p) => { return Number(Entities.PatientRequiredFieldEnum.releaseSignature) == p; }) ? Validators.required : null;
    this.assignmentdateRequired = _.find(this.ts.lookupStore.patient_required_fields, (p) => { return Number(Entities.PatientRequiredFieldEnum.assignmentOfBenefits) == p; }) ? Validators.required : null;
    this.privacydateRequired = _.find(this.ts.lookupStore.patient_required_fields, (p) => { return Number(Entities.PatientRequiredFieldEnum.privacyAgreement) == p; }) ? Validators.required : null;
    this.consentdateRequired = _.find(this.ts.lookupStore.patient_required_fields, (p) => { return Number(Entities.PatientRequiredFieldEnum.consent) == p; }) ? Validators.required : null;

    this.patientForm = this.formBuilder.group({
      lastname: ["", Validators.required],
      firstname: ["", Validators.required],
      middlename: ["", this.middleNameRequired],
      birthdate: [null, this.birthDateRequired],

      addressline1: ["", this.addressLine1Required],
      addressline2: ["", this.addressLine2Required],
      city: ["", this.cityRequired],
      state: ["", this.stateRequired],
      zipcode: ["", this.zipCodeRequired],

      address2line1: [""],
      address2line2: [""],
      city2: [""],
      state2: [""],
      zipcode2: [""],

      chart_acct_num: [""],
      useSecondaryAddress: [false],

      siteid: [this.ts.userstore.currentUser.user.siteId, Validators.required],
      seenbyproviderid: [null, this.seenbyProviderRequired],
      isdeceased: [false],
      deceaseddate: [null],
      marketReferenceId: [null, this.marketingRequired],

      primaryphone: [3],
      homephone: ["", Validators.compose([this.homePhoneRequired ? Validators.required : null, CustomValidators.phone])],
      mobilephone: ["", Validators.compose([this.mobilePhoneRequired ? Validators.required : null, CustomValidators.phone])],
      workphone: ["", Validators.compose([this.workPhoneRequired ? Validators.required : null, CustomValidators.phone])],
      otherphone: ["", Validators.compose([this.otherPhoneRequired ? Validators.required : null, CustomValidators.phone])],
      emailaddress: ["", this.emailRequired],
      genderid: [null, this.genderRequired],
      raceid: [null, this.raceRequired],
      languageid: [null, this.languageRequired],
      ethnicityid: [null, this.ethnicityRequired],

      alternateContact: [null, this.alternateContactReqired],
      altphone: [null, this.altphoneRequired],
      responsibleparty: [null],
      isinactive: [false],

      primaryphysicianid: [null, this.primaryphysicianidRequired],
      patientstatusid: [null, this.referringphysicianidRequired],
      maritalstatusid: [null],
      referringphysicianid: [null],
      emplyomentstatusid: [null],
      studentstatusid: [null],
      qbcustomertypeid: [null],
      selectedPatientTypes: [[], this.selectedPatientTypesRequired],
      selectedAuthorizations: [[]],
      selectedRestrictions: [[]],

      hasreleasesignature: [false],
      releasesignaturedate: [null, this.releasesignaturedateRequired],
      hasassignment: [false],
      assignmentdate: [null, this.assignmentdateRequired],
      privacydate: [null, this.privacydateRequired],
      hasreleaseofinfodate: [false],
      releaseofinfodate: [null],
      consentdate: [null, this.consentdateRequired],
      hasmarketingauthorizationdate: [false],
      marketingauthorizationdate: [null],
      authorizedparties: [null],
      notes: [null],

      useinvoicingaddress: [false],
      invoicingfirstname: [null],
      invoicingmiddlename: [null],
      invoicinglastname: [null],
      invoicingphone: [null],
      invoicingaddreess1: [null],
      invoicingaddreess2: [null],
      invoicingcity: [null],
      invoicingstate: [null],
      invoicingzip: [null],

      customtext1: [null],
      customtext2: [null],
      customdate1: [null],
      customdate2: [null],

      payerid1: [null],
      relationshipid1: [null],
      insuranceid1: [null],
      groupname1: [null],
      groupnumber1: [null],
      medicaresecondarycodeid1: [null],
      carrierid1: [null],
      payernotes1: [null]
    });
    this.bsConfig = Object.assign({}, { containerClass: 'theme-dark-blue', customTodayClass: 'tims-today-class', showWeekNumbers: false });
    if (this.ts.practiceStore.practiceSummary.locale == 'NZ' || this.ts.practiceStore.practiceSummary.locale == 'AU')
      this.bsConfig.dateInputFormat = 'DD/MM/YYYY';

    if (this.currentPatient) {
      if (this.currentPatient.firstName) this.patientForm.patchValue({ firstname: this.currentPatient.firstName });
      if (this.currentPatient.initial) this.patientForm.patchValue({ middlename: this.currentPatient.initial });
      if (this.currentPatient.lastName) this.patientForm.patchValue({ lastname: this.currentPatient.lastName });
      if (this.currentPatient.birthDate) this.patientForm.patchValue({ birthdate: new Date(this.currentPatient.birthDate) });
      if (this.currentPatient.email) this.patientForm.patchValue({ emailaddress: this.currentPatient.email });
      if (this.currentPatient.mobilePhone) this.patientForm.patchValue({ mobilephone: this.currentPatient.mobilePhone });
    }

    if (!this.isNew()) {
      this.patientForm.patchValue({ addressline1: this.currentPatient.address1 });
      this.patientForm.patchValue({ addressline2: this.currentPatient.address2 });
      this.patientForm.patchValue({ city: this.currentPatient.city });
      this.patientForm.patchValue({ state: this.currentPatient.state });
      this.patientForm.patchValue({ zipcode: this.currentPatient.zipCode });

      this.patientForm.patchValue({ useSecondaryAddress: this.currentPatient.useSecondaryAddress });

      this.patientForm.patchValue({ address2line1: this.currentPatient.secondaryAddress1 });
      this.patientForm.patchValue({ address2line2: this.currentPatient.secondaryAddress2 });
      this.patientForm.patchValue({ city2: this.currentPatient.secondaryCity });
      this.patientForm.patchValue({ state2: this.currentPatient.secondaryCity });
      this.patientForm.patchValue({ zipcode2: this.currentPatient.secondaryZipCode });

      this.patientForm.patchValue({ chart_acct_num: this.currentPatient.accountNo });
      // In case of inactive site
      let site = _.find(this.ts.siteStore.site_list, (x) => { return x.id == this.currentPatient.siteId });
      if (!site) {
        this.ts.siteStore.getSummary(this.currentPatient.siteId);
        when(() => !this.ts.siteStore.inprogress,
          () => {
            this.patientForm.patchValue({ siteid: this.currentPatient.siteId });
          });
      }
      else
        this.patientForm.patchValue({ siteid: this.currentPatient.siteId });

      // In case of inactive provider
      let provider = _.find(this.ts.providerStore.provider_list_w_blank, (x) => { return x.id == this.currentPatient.providerId });
      if (!provider) {
        this.ts.providerStore.getSummary(this.currentPatient.providerId);
        when(() => !this.ts.providerStore.inprogress,
          () => {
            this.patientForm.patchValue({ seenbyproviderid: this.currentPatient.providerId });
          });
      }
      else
        this.patientForm.patchValue({ seenbyproviderid: this.currentPatient.providerId });

      this.patientForm.patchValue({ isdeceased: this.currentPatient.deceased });
      if (this.currentPatient.deceased && this.currentPatient.deathDate)
        this.patientForm.patchValue({ deceaseddate: new Date(this.currentPatient.deathDate) });
      this.isdeceasedChanged(this.currentPatient.deceased);

      this.patientForm.patchValue({ homephone: this.currentPatient.homePhone });
      this.patientForm.patchValue({ workphone: this.currentPatient.workPhone });
      this.patientForm.patchValue({ mobilephone: this.currentPatient.mobilePhone });
      this.patientForm.patchValue({ otherphone: this.currentPatient.otherPhone });
      this.patientForm.patchValue({ emailaddress: this.currentPatient.email });

      this.patientForm.patchValue({ languageid: this.currentPatient.language });
      this.patientForm.patchValue({ raceid: this.currentPatient.race });
      this.patientForm.patchValue({ genderid: this.currentPatient.sex });
      this.patientForm.patchValue({ ethnicityid: this.currentPatient.ethnicity });
      this.patientForm.patchValue({ marketReferenceId: this.currentPatient.marketingId });

      this.patientForm.patchValue({ alternateContact: this.currentPatient.alternateContact });
      this.patientForm.patchValue({ altphone: this.currentPatient.alternateContactPhone });
      this.patientForm.patchValue({ responsibleparty: this.currentPatient.responsibleParty });
      this.patientForm.patchValue({ isinactive: this.currentPatient.inactive });

      this.patientForm.patchValue({ primaryphysicianid: this.currentPatient.primaryCareId });
      this.patientForm.patchValue({ patientstatusid: this.currentPatient.patientStatusId });
      this.patientForm.patchValue({ maritalstatusid: this.currentPatient.maritalStatusId });
      this.patientForm.patchValue({ referringphysicianid: this.currentPatient.referringPhysicianId });
      this.patientForm.patchValue({ emplyomentstatusid: this.currentPatient.emplStatusId });
      this.patientForm.patchValue({ studentstatusid: this.currentPatient.studentStatusId });
      this.patientForm.patchValue({ patientTypeId: this.currentPatient.patientTypeId });

      this.patientForm.patchValue({ hasreleasesignature: this.currentPatient.releaseSignature });
      if (this.currentPatient.releaseSignatureDate)
        this.patientForm.patchValue({ releasesignaturedate: new Date(this.currentPatient.releaseSignatureDate) });
      this.patientForm.patchValue({ hasassignment: this.currentPatient.assignBenefits });
      if (this.currentPatient.assignBenefitsDate)
        this.patientForm.patchValue({ assignmentdate: new Date(this.currentPatient.assignBenefitsDate) });
      if (this.currentPatient.privacyDate)
        this.patientForm.patchValue({ privacydate: new Date(this.currentPatient.privacyDate) });
      this.patientForm.patchValue({ hasreleaseofinfodate: this.currentPatient.releaseInformation });
      if (this.currentPatient.releaseInformationDate)
        this.patientForm.patchValue({ releaseofinfodate: new Date(this.currentPatient.releaseInformationDate) });
      if (this.currentPatient.consentDate)
        this.patientForm.patchValue({ consentdate: new Date(this.currentPatient.consentDate) });
      this.patientForm.patchValue({ hasmarketingauthorizationdate: this.currentPatient.marketingAuthorization });
      if (this.currentPatient.marketingAuthorizationDate)
        this.patientForm.patchValue({ marketingauthorizationdate: new Date(this.currentPatient.marketingAuthorizationDate) });
      this.patientForm.patchValue({ authorizedparties: this.currentPatient.authorizedParties });

      this.patientForm.patchValue({ notes: this.currentPatient.notes });

      this.patientForm.patchValue({ useinvoicingaddress: this.currentPatient.legalRep });
      this.patientForm.patchValue({ invoicingfirstname: this.currentPatient.legalRepFirstName });
      this.patientForm.patchValue({ invoicingmiddlename: this.currentPatient.legalRepInitial });
      this.patientForm.patchValue({ invoicinglastname: this.currentPatient.legalRepLastName });
      this.patientForm.patchValue({ invoicingphone: this.currentPatient.legalRepPhone });
      this.patientForm.patchValue({ invoicingaddreess1: this.currentPatient.legalRepAddress1 });
      this.patientForm.patchValue({ invoicingaddreess2: this.currentPatient.legalRepAddress2 });
      this.patientForm.patchValue({ invoicingcity: this.currentPatient.legalRepCity });
      this.patientForm.patchValue({ invoicingstate: this.currentPatient.legalRepState });
      this.patientForm.patchValue({ invoicingzip: this.currentPatient.legalRepZipCode });

      this.patientForm.patchValue({ customtext1: this.currentPatient.customText1 });
      this.patientForm.patchValue({ customtext2: this.currentPatient.customText2 });
      if (this.currentPatient.customDate1)
        this.patientForm.patchValue({ customdate1: new Date(this.currentPatient.customDate1) });
      if (this.currentPatient.customDate2)
        this.patientForm.patchValue({ customdate2: new Date(this.currentPatient.customDate2) });

      var restrictionIds = _.map(this.currentPatient.restrictions, (x) => { return x.restrictionId });
      var communcitaitionRestrictons: Entities.CommunicationRestriction[] = [];
      _.each(restrictionIds, (v) => {
        var cr = _.find(this.ts.lookupStore.communicationRestriction_list, (t: Entities.CommunicationRestriction) => { return t.id == v; });
        if (cr) communcitaitionRestrictons.push(cr);
      });
      this.patientForm.patchValue({ selectedRestrictions: communcitaitionRestrictons });

      var authorizationIds = _.map(this.currentPatient.patientTypeReferences, (x) => { return x.patientTypeId });
      var patientTypes: Entities.PatientType[] = [];
      _.each(authorizationIds, (v) => {
        var cr = _.find(this.ts.lookupStore.patienttypes_list, (t: Entities.PatientType) => { return t.id == v; });
        if (cr) patientTypes.push(cr);
      });
      this.patientForm.patchValue({ selectedPatientTypes: patientTypes });

      var authorizationIds = _.map(this.currentPatient.authorizationReferences, (x) => { return x.authorizationId });
      var athorizations: Entities.Authorization[] = [];
      _.each(authorizationIds, (v) => {
        var auth = _.find(this.ts.lookupStore.authorization_list, (t: Entities.Authorization) => { return t.id == v; });
        if (auth) athorizations.push(auth);
      });
      this.patientForm.patchValue({ selectedAuthorizations: athorizations });

      this.ts.patientInsuranceStore.getForPatient(this.currentPatient.id);
      when(() => this.ts.patientInsuranceStore.patientinsurance_list != null,
        () => {
          let primary = this.ts.patientInsuranceStore.patientinsurance_list.get(1);
          if (primary) {
            //this.ts.patientInsuranceStore.update(primary);
            this.primaryInsuranceEntryComponent.patchForm(primary);
          }
          let secondary = this.ts.patientInsuranceStore.patientinsurance_list.get(2);
          if (secondary) {
            this.secondaryInsuranceEntryComponent.patchForm(secondary);
          }
          let tertiary = this.ts.patientInsuranceStore.patientinsurance_list.get(3);
          if (tertiary) {
            this.tertiaryInsuranceEntryComponent.patchForm(tertiary);
          }
        });
      if (this.ts.practiceStore.businessRules.useApptAuthorizations) {
        this.ts.apptAuthorizationStore.getForPatient(this.currentPatient.id, false, 0);
        when(() => this.ts.apptAuthorizationStore.apptAuthorization_list != null,
          () => {
            _.each(this.ts.apptAuthorizationStore.apptAuthorization_list, (a: Entities.ApptAuthorization) => {
              this.appointmentAuthorizations.push(a);
            });
          });
      }
    }

    this.isdeceasedChanged(this.f['deceaseddate'].value);
    this.hasreleasesignatureChanged(this.f['releasesignaturedate'].value);
    this.hasassignmentChanged(this.f['assignmentdate'].value);
    this.hasreleaseofinfodateChanged(this.f['releaseofinfodate'].value);
    this.hasmarketingauthorizationdateChanged(this.f['marketingauthorizationdate'].value);
    this.useInvoicingAddressChanged(this.f['useinvoicingaddress'].value);

    this.patientForm.valueChanges.subscribe(data => {
      //this._hasBeenValidated = false;
    });
    if (this.ts.lookupStore.descriptions.customText1Label)
      this.customtext1label = this.ts.lookupStore.descriptions.customText1Label;
    if (this.ts.lookupStore.descriptions.customText2Label)
      this.customtext2label = this.ts.lookupStore.descriptions.customText2Label;
    if (this.ts.lookupStore.descriptions.customDate1Label)
      this.customdate1label = this.ts.lookupStore.descriptions.customDate1Label;
    if (this.ts.lookupStore.descriptions.customDate2Label)
      this.customdate2label = this.ts.lookupStore.descriptions.customDate2Label;

    this.onClose = new Subject();
  }

  editApptAuthorization(id) {
    let currentAuthorization = _.find(this.appointmentAuthorizations, (x: Entities.ApptAuthorization) => { return x.id == id });
    if (!currentAuthorization.inactive) {
      let initialState: ModalOptions = { initialState: { currentAuthorization }, ignoreBackdropClick: true, id: AuthorizationEditorComponent.modlaId };
      this.bsModalRef = this.modalService.show(AuthorizationEditorComponent, Object.assign({}, initialState));
      this.bsModalRef.content.onClose.subscribe(result => {
        if (result) {
          if (currentAuthorization.isDeleted) {
            this.appointmentAuthorizations = _.reject(this.appointmentAuthorizations, (x) => x.id == currentAuthorization.id && x.name == currentAuthorization.name);
          }
        }
      });
    }
  }

  showinactiveapptauthorizationsChanged() {
    this.showInactiveApptAuthorizations = !this.showInactiveApptAuthorizations;
    this.appointmentAuthorizations = [];
    this.ts.apptAuthorizationStore.getForPatient(this.currentPatient.id, this.showInactiveApptAuthorizations, 0);
    when(() => this.ts.apptAuthorizationStore.apptAuthorization_list != null,
      () => {
        _.each(this.ts.apptAuthorizationStore.apptAuthorization_list, (a: Entities.ApptAuthorization) => {
          this.appointmentAuthorizations.push(a);
        });
      });
  }

  useInvoicingAddressChanged(value) {
    if (value) {
      this.f['invoicingfirstname'].enable();
      this.f['invoicingmiddlename'].enable();
      this.f['invoicinglastname'].enable();
      this.f['invoicingphone'].enable();
      this.f['invoicingaddreess1'].enable();
      this.f['invoicingaddreess2'].enable();
      this.f['invoicingcity'].enable();
      this.f['invoicingstate'].enable();
      this.f['invoicingzip'].enable();
    }
    else {
      this.f['invoicingfirstname'].disable();
      this.f['invoicingmiddlename'].disable();
      this.f['invoicinglastname'].disable();
      this.f['invoicingphone'].disable();
      this.f['invoicingaddreess1'].disable();
      this.f['invoicingaddreess2'].disable();
      this.f['invoicingcity'].disable();
      this.f['invoicingstate'].disable();
      this.f['invoicingzip'].disable();
    }
  }

  useSecondaryAddressChanged(value) {
    this.useSecondaryAddress = value;
    this.setSecondaryAddressState();
  }

  addressForm2Ready() {
    this.setSecondaryAddressState();
  }

  setSecondaryAddressState() {
    if (!this.addressEntry2) return;
    if (this.useSecondaryAddress)
      this.addressEntry2.enable();
    else
      this.addressEntry2.disable();
  }

  validateForm() {
    _.each(this.f, (x) => { x.markAsTouched() });
  }

  isdeceasedChanged(value) {
    if (value)
      this.f['deceaseddate'].enable();
    else
      this.f['deceaseddate'].disable();
  }

  hasreleasesignatureChanged(value) {
    if (value)
      this.f['releasesignaturedate'].enable();
    else
      this.f['releasesignaturedate'].disable();
  }

  hasassignmentChanged(value) {
    if (value)
      this.f['assignmentdate'].enable();
    else
      this.f['assignmentdate'].disable();
  }

  hasreleaseofinfodateChanged(value) {
    if (value) {
      this.f['releaseofinfodate'].enable();
      this.f['authorizedparties'].enable();
    }
    else {
      this.f['releaseofinfodate'].disable();
      this.f['authorizedparties'].disable();
    }
  }

  hasmarketingauthorizationdateChanged(value) {
    if (value) {
      this.f['marketingauthorizationdate'].enable();
      this.marketingauthorizationDisabled = false;
    }
    else {
      this.f['marketingauthorizationdate'].disable();
      this.marketingauthorizationDisabled = true;
    }
  }

  handlesDetailsClick() {
    this.isdetalsselected = true;
  }

  handlesNotesClick() {
    this.isdetalsselected = false;
  }

  invoicingZipChanged(event) {
    if (this.f.invoicingzip.value.length == 5) {
      this.ts.lookupStore.getCityAndStateFromZipCode(this.f.invoicingzip.value);
      when(() => this.ts.lookupStore.cityAndState != null, () => {
        let city = StringUtils.toTitleCase(this.ts.lookupStore.cityAndState.city);
        this.f.invoicingcity.setValue(city);
        this.f.invoicingstate.setValue(this.ts.lookupStore.cityAndState.state);
      });
    }
  }

  handlesAddAuthoriztion() {
    let currentAuthorization = new Entities.ApptAuthorization();
    currentAuthorization.id = 0;
    currentAuthorization.numberUsed = 0;
    currentAuthorization.patientId = this.currentPatient.id;
    let initialState: ModalOptions = { initialState: { currentAuthorization }, ignoreBackdropClick: true, id: AuthorizationEditorComponent.modlaId };
    this.bsModalRef = this.modalService.show(AuthorizationEditorComponent, Object.assign({}, initialState));
    this.bsModalRef.content.onClose.subscribe(result => {
      if (result) {
        currentAuthorization.id = -1;
        this.appointmentAuthorizations.push(currentAuthorization);
      }
    });
  }

  onSubmit() {
    this.validateForm();

    if (this.patientForm.invalid) {
      return;
    }
    const result = Object.assign({}, this.patientForm.value);

    this.currentPatient.firstName = result.firstname;
    this.currentPatient.lastName = result.lastname;
    this.currentPatient.initial = result.middlename;
    if (result.birthdate)
      this.currentPatient.birthDate = DateUtils.createDbDateOnlyFromDate(result.birthdate);

    this.currentPatient.address1 = result.addressline1;
    this.currentPatient.address2 = result.addressline2;
    this.currentPatient.city = result.city;
    this.currentPatient.state = result.state;
    this.currentPatient.zipCode = result.zipcode;

    this.currentPatient.useSecondaryAddress = result.useSecondaryAddress;

    this.currentPatient.secondaryAddress1 = result.address2line1;
    this.currentPatient.secondaryAddress2 = result.address2line2;
    this.currentPatient.secondaryCity = result.city2;
    this.currentPatient.secondaryState = result.state2;
    this.currentPatient.secondaryZipCode = result.zipcode2;

    this.currentPatient.accountNo = result.chart_acct_num;
    this.currentPatient.siteId = Number(result.siteid);
    this.currentPatient.providerId = Number(result.seenbyproviderid);
    this.currentPatient.marketingId = Number(result.marketReferenceId);

    this.currentPatient.deceased = result.isdeceased;
    if (result.isdeceased && result.deceaseddate) {
      this.currentPatient.deathDate = DateUtils.createDbDateOnlyFromDate(result.deceaseddate);
    }
    else {
      this.currentPatient.deathDate = null;
    }

    this.currentPatient.homePhone = result.homephone;
    this.currentPatient.workPhone = result.workphone;
    this.currentPatient.otherPhone = result.otherphone;
    this.currentPatient.mobilePhone = result.mobilephone;
    this.currentPatient.email = result.emailaddress;

    this.currentPatient.language = result.languageid ? Number(result.languageid) : null;
    this.currentPatient.race = result.raceid ? Number(result.raceid) : null;
    this.currentPatient.ethnicity = result.ethnicityid ? Number(result.ethnicityid) : null;
    this.currentPatient.sex = result.genderid;

    this.currentPatient.alternateContact = result.alternateContact;
    this.currentPatient.alternateContactPhone = result.altphone;
    this.currentPatient.responsibleParty = result.responsibleparty;
    this.currentPatient.inactive = result.isinactive;

    this.currentPatient.primaryCareId = result.primaryphysicianid;
    this.currentPatient.patientStatusId = result.patientstatusid;
    this.currentPatient.maritalStatusId = result.maritalstatusid;
    this.currentPatient.referringPhysicianId = result.referringphysicianid;
    this.currentPatient.emplStatusId = result.emplyomentstatusid;
    this.currentPatient.studentStatusId = result.studentstatusid;
    this.currentPatient.patientTypeId = result.qbcustomertypeid;

    this.currentPatient.releaseSignature = result.hasreleasesignature;
    this.currentPatient.releaseSignatureDate = DateUtils.createDbDateOnlyFromDate(result.releasesignaturedate);
    this.currentPatient.assignBenefits = result.hasassignment;
    this.currentPatient.assignBenefitsDate = DateUtils.createDbDateOnlyFromDate(result.assignmentdate);
    this.currentPatient.privacyDate = DateUtils.createDbDateOnlyFromDate(result.privacydate);
    this.currentPatient.releaseInformation = result.hasreleaseofinfodate;
    this.currentPatient.releaseInformationDate = DateUtils.createDbDateOnlyFromDate(result.releaseofinfodate);
    this.currentPatient.consentDate = DateUtils.createDbDateOnlyFromDate(result.consentdate);
    this.currentPatient.marketingAuthorization = result.hasmarketingauthorizationdate;
    this.currentPatient.marketingAuthorizationDate = DateUtils.createDbDateOnlyFromDate(result.marketingauthorizationdate);
    this.currentPatient.authorizedParties = result.authorizedparties;

    this.currentPatient.legalRepFirstName = result.invoicingfirstname;
    this.currentPatient.legalRepInitial = result.invoicingmiddlename;
    this.currentPatient.legalRepLastName = result.invoicinglastname;
    this.currentPatient.legalRep = result.useinvoicingaddress;
    this.currentPatient.legalRepPhone = result.invoicingphone;
    this.currentPatient.legalRepAddress1 = result.invoicingaddreess1;
    this.currentPatient.legalRepAddress2 = result.invoicingaddreess2;
    this.currentPatient.legalRepCity = result.invoicingcity;
    this.currentPatient.legalRepState = result.invoicingstate;
    this.currentPatient.legalRepZipCode = result.invoicingzip;

    this.currentPatient.customText1 = result.customtext1;
    this.currentPatient.customText2 = result.customtext2;
    this.currentPatient.customDate1 = DateUtils.createDbDateOnlyFromDate(result.customdate1);
    this.currentPatient.customDate2 = DateUtils.createDbDateOnlyFromDate(result.customdate2);

    this.currentPatient.notes = result.notes;

    this.currentPatient.patientTypeIds = _.map(result.selectedPatientTypes, (p) => p.id);
    this.currentPatient.restrictionIds = _.map(result.selectedRestrictions, (p) => p.id);
    this.currentPatient.authorizationIds = _.map(result.selectedAuthorizations, (p) => p.id);

    let insurances: Entities.PatientInsurance[] = [];
    if (this.primaryInsuranceEntryComponent.isValid()) {
      if (this.primaryInsuranceEntryComponent.isNew()) {
        this.primaryInsuranceEntryComponent.patientInsurance = new Entities.PatientInsurance();
        this.primaryInsuranceEntryComponent.patientInsurance.payerLevel = 1;
      }
      this.primaryInsuranceEntryComponent.submitForm();
      insurances.push(this.primaryInsuranceEntryComponent.patientInsurance);
    }
    else {
      // It exists, but was cleared
      if (!this.primaryInsuranceEntryComponent.isNew()) {
        this.primaryInsuranceEntryComponent.submitForm();
        this.primaryInsuranceEntryComponent.patientInsurance.insurancePayerId = 0;
        insurances.push(this.primaryInsuranceEntryComponent.patientInsurance);
      }
    }
    if (this.secondaryInsuranceEntryComponent.isValid()) {
      if (this.secondaryInsuranceEntryComponent.isNew()) {
        this.secondaryInsuranceEntryComponent.patientInsurance = new Entities.PatientInsurance();
        this.secondaryInsuranceEntryComponent.patientInsurance.payerLevel = 2;
      }
      this.secondaryInsuranceEntryComponent.submitForm();
      insurances.push(this.secondaryInsuranceEntryComponent.patientInsurance);
    }
    else {
      // It exists, but was cleared
      if (!this.secondaryInsuranceEntryComponent.isNew()) {
        this.secondaryInsuranceEntryComponent.submitForm();
        this.secondaryInsuranceEntryComponent.patientInsurance.insurancePayerId = 0;
        insurances.push(this.secondaryInsuranceEntryComponent.patientInsurance);
      }
    }
    if (this.tertiaryInsuranceEntryComponent.isValid()) {
      if (this.tertiaryInsuranceEntryComponent.isNew()) {
        this.tertiaryInsuranceEntryComponent.patientInsurance = new Entities.PatientInsurance();
        this.tertiaryInsuranceEntryComponent.patientInsurance.payerLevel = 3;
      }
      this.tertiaryInsuranceEntryComponent.submitForm();
      insurances.push(this.tertiaryInsuranceEntryComponent.patientInsurance);
    }
    else {
      // It exists, but was cleared
      if (!this.tertiaryInsuranceEntryComponent.isNew()) {
        this.tertiaryInsuranceEntryComponent.submitForm();
        this.tertiaryInsuranceEntryComponent.patientInsurance.insurancePayerId = 0;
        insurances.push(this.tertiaryInsuranceEntryComponent.patientInsurance);
      }
    }

    if (this.isNew()) {
      this.ts.patientStore.selected_patient_summary = null;
      this.ts.patientStore.create(this.currentPatient, insurances, this.appointmentAuthorizations);
    }
    else {
      _.each(this.appointmentAuthorizations, (auth: Entities.ApptAuthorization) => {
        let a = _.find(this.ts.apptAuthorizationStore.apptAuthorization_list, (x) => { return x.id == auth.id });
        if (a == null) this.ts.apptAuthorizationStore.apptAuthorization_list.push(auth);
      });
      this.ts.patientStore.update(this.currentPatient, insurances, this.ts.apptAuthorizationStore.apptAuthorization_list);
    }

    this.onClose.next(true);
    this.modalService.hide(PatientEditorComponent.modlaId);

  }

  onCancel() {
    this.onClose.next(null);
    this.modalService.hide(PatientEditorComponent.modlaId);
  }
}
