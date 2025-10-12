import { Injectable } from '@angular/core';
import { ActivationStart, Router } from '@angular/router';
import { TimsStore } from '@app/stores/tims.store';

@Injectable({
    providedIn: 'root'
})
export class AccessHelper {

    public requiresAuthentication: boolean = true;

    constructor(private router: Router, private timsStore: TimsStore) {
        this.router.events.subscribe(data => {
            if (data instanceof ActivationStart) {
                if (this.urlRequiresAuthentication(data.snapshot.url[0].path)) {
                    this.requiresAuthentication = false;
                    this.timsStore.patientScheduleStore.officeCode = data.snapshot.queryParams.officeCode;
                }
                console.log(`URL path `, data.snapshot.url[0].path);
                if (this.requiresAuthentication) {
                    this.timsStore.initializeWebScehdulerStores();
                }
                else {
                    this.timsStore.initializePatientScheduleStores();
                }
            }
        });
    }

    private urlRequiresAuthentication(path: string) {
        return path == 'patient-schedule' || path == 'patient-schedule-complete';
    }
}