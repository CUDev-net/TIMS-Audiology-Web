import { Injectable } from '@angular/core';
import { makeObservable, observable } from 'mobx';
import * as _ from 'underscore';

import { UserService } from '../services/user.service';
import { BaseStore } from './base.store';

import { Entities } from '@app/entities/entities';
import UserDto = Entities.UserDto;
import User = Entities.User;

@Injectable()
export class UserStore extends BaseStore {
    @observable public currentUser: UserDto = null;
    @observable public calendarInterval: number = null;
    private currentUserPermissions: Entities.SettingEnum[] = null;

    constructor(private userService: UserService) {
        super();
        makeObservable(this);
    }

    public doesUserHaveSetting(setting: Entities.SettingEnum): boolean {
        return _.contains(this.currentUserPermissions, setting);
    }

    public getCurrentUser() {
        this.inprogress = true;
        this.userService.getCurrentUser().subscribe((p) => {
            this.currentUser = p;
            if (p.user.calendarInterval <= 15)
                this.calendarInterval = 15;
            else
                this.calendarInterval = 30;
            this.inprogress = false;
            this.getCurrentUserPermissions();
        });
    }

    public getCurrentUserPermissions() {
        this.inprogress = true;
        this.userService.getCurrentUserPermissions(this.currentUser.user.id).subscribe((p) => {
            this.currentUserPermissions = p;
            this.inprogress = false;
        });
    }

    public getUserPatientList() {
        this.userService.getUserPatientList().subscribe((p) => {
            this.currentUser.lastPatientSummaries = p;
        });
    }

    public update(user: UserDto) {
        this.inprogress = true;
        this.userService.update(user).subscribe((p: User) => {
            this.currentUser.user = p;
            this.inprogress = false;
        });
    }
}
