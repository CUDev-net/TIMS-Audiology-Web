import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { BsModalService, BsModalRef, ModalOptions } from 'ngx-bootstrap/modal';
import { when } from 'mobx';

import { TimsStore } from '@app/stores/tims.store';

import { PatientEditorComponent } from '../patient-editor/patient-editor.component';

import { Entities } from '@app/entities/entities';
import Patient = Entities.Patient;
import PatientSearchCriteria = Entities.PatientSearchCriteriaDto;


@Component({
  selector: 'app-patient-selector',
  templateUrl: './patient-selector.component.html',
  styleUrl: './patient-selector.component.scss'
})
export class PatientSelectorComponent implements OnInit {
  private bsModalRef: BsModalRef;

  get isdisabled() {
    return this.ts.lookupStore.storesLoading;
  }

  constructor(private formBuilder: FormBuilder, public ts: TimsStore, private modalService: BsModalService) {
  }

  @Output() patientSelected = new EventEmitter<number>();
  public censusForm: FormGroup;
  public isSearching: boolean = false;

  ngOnInit(): void {
    this.censusForm = this.formBuilder.group({
      searchParameter: [''],
      searchCriteria: new FormControl('lastname'),
      includeInactive: false
    });

    this.censusForm.valueChanges.subscribe(data => {
      this.isSearching = data.searchParameter.length > 1;
      if (!this.isSearching) this.ts.patientStore.censusPatients = null;
    });
  }

  onPatientSelected(id) {
    this.patientSelected.emit(id);
    this.censusForm.patchValue({ searchParameter: '' });
    this.censusForm.patchValue({ searchCriteria: 'lastname' });
    this.ts.patientStore.censusPatients = null;
  }

  onSearch() {
    if (this.isSearching) {
      const result = Object.assign({}, this.censusForm.value);
      let criteria = new PatientSearchCriteria();
      if (result.searchCriteria == 'lastname')
        criteria.lastName = result.searchParameter;
      else if (result.searchCriteria == 'firstname')
        criteria.firstName = result.searchParameter;
      else if (result.searchCriteria == 'birthdate')
        criteria.dateOfBirth = new Date(result.searchParameter);
      else if (result.searchCriteria == 'phone')
        criteria.phoneNumber = result.searchParameter;
      criteria.includeInactive = result.includeInactive;

      this.ts.patientStore.searchPatients(criteria);
    }
  }
  onSearchValue(event) {
    if (event.key == "Enter") {
      this.onSearch();
    }
  }

  onAdd() {
    let currentPatient = new Patient();
    const initialState: ModalOptions = { initialState: { currentPatient }, class: 'ex-modal-dialog', ignoreBackdropClick: true, id: PatientEditorComponent.modlaId };
    this.bsModalRef = this.modalService.show(PatientEditorComponent, initialState);
    this.bsModalRef.content.onClose.subscribe(result => {
      if (result) {
        when(() => this.ts.patientStore.selected_patient != null,
          () => {
            this.ts.patientStore.getSummary(this.ts.patientStore.selected_patient.id);
            this.patientSelected.emit(this.ts.patientStore.selected_patient.id);
            this.ts.patientStore.new_appointment_patientId = this.ts.patientStore.selected_patient.id;
          });
      }
    });
  }
}
