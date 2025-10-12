import { Injectable } from '@angular/core';
import { makeObservable, observable } from 'mobx';
import * as _ from 'underscore';

import { SiteService } from '../services/site.service';
import { BaseStore } from './base.store';

import { Entities } from '@app/entities/entities';
import Site = Entities.Site;

@Injectable()
export class SiteStore extends BaseStore {
    @observable public site_list: Site[] = null;
    @observable public selected_site: Site = null;

    constructor(private siteService: SiteService) {
        super();
        makeObservable(this);
    }

    public getSummaries() {
        this.site_list = null;
        this.siteService.getSummaries().subscribe((p) => {
            this.site_list = _.sortBy(p, 'name');
        });
    }

    public getSummary(id: number) {
        this.inprogress = true;
        this.siteService.getSummary(id).subscribe((p) => {
            this.site_list = this.site_list.concat(p);
            this.inprogress = false;
        });
    }

    public removeInactiveSites() {
        this.site_list = this.site_list.filter(p => !p.inactive);
    }
}