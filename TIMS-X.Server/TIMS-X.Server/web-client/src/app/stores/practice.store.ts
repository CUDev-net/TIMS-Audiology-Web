import { Injectable } from '@angular/core';
import { makeObservable, observable } from 'mobx';
import * as _ from 'underscore';

import { PracticeService } from '../services/practice.service';
import { BaseStore } from './base.store';

import { Entities } from '@app/entities/entities';
import PracticeSummary = Entities.PracticeSummary;
import HoursOfOperationModel = Entities.HoursOfOperationModel;

@Injectable()
export class PracticeStore extends BaseStore {
    @observable public practiceSummary: PracticeSummary = null;
    @observable public hoursOfOperation: HoursOfOperationModel[] = [];
    @observable public businessRules: Entities.BusinessRuleDto = null;

    constructor(private practiceService: PracticeService) {
        super();
        makeObservable(this);
    }

    public get() {
        this.practiceService.get().subscribe((p) => {
            this.practiceSummary = p;
        });
    }

    public getBusinessRules() {
        this.businessRules = null;
        this.practiceService.getBusinessRules().subscribe((p) => {
            this.businessRules = p;
        });
    }

    public getHours() {
        this.inprogress = true;
        this.practiceService.getHours().subscribe((p) => {
            this.hoursOfOperation = p;
            this.inprogress = false;
        });
    }
}