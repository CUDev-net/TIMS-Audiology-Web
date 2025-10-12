import { Injectable } from '@angular/core';
import { makeObservable, observable } from 'mobx';
import * as _ from 'underscore';

import { ResourceService } from '@app/services/resource.service';
import { BaseStore } from './base.store';

import { Entities } from '@app/entities/entities';
import Resource = Entities.Resource;


@Injectable()
export class ResourceStore extends BaseStore {
    @observable public resource_list: Resource[] = null;

    constructor(private resourceService: ResourceService) {
        super();
        makeObservable(this);
    }

    public getSummaries() {
        this.resource_list = null;
        this.resourceService.getSummaries().subscribe((p) => {
            this.resource_list = _.sortBy(p, 'name');
        });
    }

    public getResourcesForSite(siteId: number) {
        return this.resource_list.filter((r) => r.siteId == siteId);
    }
}