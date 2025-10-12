import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { SchedulerHostComponent } from './components/scheduler-host/scheduler-host.component';
import { PatientScheduleComponent } from './components/patient-schedule/patient-schedule.component';
import { PatientScheduleCompleteComponent } from './components/patient-schedule-complete/patient-schedule-complete.component';

const routes: Routes = [
  { path: 'scheduler', component: SchedulerHostComponent },
  { path: 'patient-schedule', component: PatientScheduleComponent },
  { path: 'patient-schedule-complete', component: PatientScheduleCompleteComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
