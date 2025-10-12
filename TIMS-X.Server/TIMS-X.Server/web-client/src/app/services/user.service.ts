import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { EnvironmentUrlService } from '../services/environment-url.service';

import { Entities } from '@app/entities/entities';
import UserDto = Entities.UserDto;
import User = Entities.User;
import PatientSummary = Entities.PatientSummary;

@Injectable({
    providedIn: 'root'
})
export class UserService {
    constructor(private http: HttpClient, private envUrl: EnvironmentUrlService) { }

    public getCurrentUser = () => {
        let route = `/web/user/get-current-user`;
        return this.http.get<UserDto>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public getCurrentUserPermissions = (userId: number) => {
        let route = `/web/user/get-permissions?userId=${userId}`;
        return this.http.get<Entities.SettingEnum[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public getUserPatientList = () => {
        let route = `/web/user/get-patient-list`;
        return this.http.get<PatientSummary[]>(this.createCompleteRoute(route, this.envUrl.urlAddress));
    }

    public update = (user: UserDto) => {
        let route = `/web/user/${user.user.id}`;
        return this.http.put<User>(this.createCompleteRoute(route, this.envUrl.urlAddress), user);
    }

    private createCompleteRoute = (route: string, envAddress: string) => {
        return `${envAddress}${route}`;
    }
}