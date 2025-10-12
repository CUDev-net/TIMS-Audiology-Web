import { Injectable } from '@angular/core';
import { makeObservable, observable } from 'mobx';
import * as _ from 'underscore';

import { BaseStore } from './base.store';

import { Entities } from '@app/entities/entities';
import { InsurancePayerService } from '@app/services/insurance-payer.service';

@Injectable()
export class InsurancePayerStore extends BaseStore {
    @observable public payer_list: Entities.InsurancePayer[] = null;
    @observable public selected_payer: Entities.InsurancePayer = null;

    constructor(private payerService: InsurancePayerService) {
        super();
        makeObservable(this);
    }

    public getPayers() {
        this.payer_list = null;
        this.payerService.getPayers().subscribe((p) => {
            this.payer_list = _.sortBy(p, 'name');
        });
    }

    public getPayer(id: number) {
        this.inprogress = true;
        this.payerService.getPayer(id).subscribe((p) => {
            this.selected_payer = p;
            this.inprogress = false;
        });
    }

}