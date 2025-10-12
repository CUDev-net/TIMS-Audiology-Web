import { Component, OnInit } from '@angular/core';

import { PracticeStore } from './stores/practice.store';
import { UserStore } from './stores/user.store';
import { AccessHelper } from './utilities/access-helper';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  public get isProviderLoggedIn() { return this.accessHelper.requiresAuthentication; }

  constructor(public practiceStore: PracticeStore, private userStore: UserStore, private accessHelper: AccessHelper) {
  }

  ngOnInit(): void {
  }

  get practiceName() {
    return this.practiceStore.practiceSummary == null ? null : this.practiceStore.practiceSummary.officeCode;
  }

  get userName() {
    return this.userStore.inprogress || this.userStore.currentUser == null ? null : this.userStore.currentUser.user.name;
  }
}
