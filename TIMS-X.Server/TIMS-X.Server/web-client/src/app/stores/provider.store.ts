import { Injectable } from '@angular/core';
import { makeObservable, observable } from 'mobx';
import * as _ from 'underscore';

import { BaseStore } from './base.store';
import { ProviderService } from '../services/provider.service';

import { Entities } from '@app/entities/entities';
import ProviderSummary = Entities.ProviderSummary;

@Injectable()
export class ProviderStore extends BaseStore {
    @observable provider_summary_list: ProviderSummary[] = [];
    @observable provider_list_w_blank: ProviderSummary[] = [];
    @observable provider_w_Hours_list: ProviderSummary[] = null;

    constructor(private providerService: ProviderService) {
        super();
        makeObservable(this);
    }

    public getSummaries() {
        this.inprogress = true;
        this.providerService.getSummaries().subscribe((p) => {
            this.provider_summary_list = _.sortBy(p, 'firstName');
            let blankProvider = new ProviderSummary();
            blankProvider.id = 0;
            blankProvider.firstName = '';
            blankProvider.lastName = '';
            this.provider_list_w_blank = [blankProvider].concat(this.provider_summary_list);
            this.inprogress = false;
        });
    }

    public getSummary(id: number) {
        this.inprogress = true;
        this.providerService.getSummary(id).subscribe((p) => {
            this.provider_summary_list = this.provider_summary_list.concat(p);
            this.inprogress = false;
        });
    }

    public getWithHours() {
        this.inprogress = true;
        this.provider_w_Hours_list = null;
        this.providerService.getWithHours().subscribe((p) => {
            this.provider_w_Hours_list = _.sortBy(p, 'firstName');
            this.inprogress = false;
        });
    }

    public removeInactiveProviders() {
        this.provider_summary_list = this.provider_summary_list.filter(p => !p.inactive);
    }
}