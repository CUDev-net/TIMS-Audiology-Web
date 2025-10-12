import { Component, Input, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import * as _ from 'underscore';

@Component({
  selector: 'app-name-entry',
  templateUrl: './name-entry.component.html',
  styleUrls: ['./name-entry.component.scss']
})
export class NameEntryComponent implements OnInit {
  @Input() nameform: FormGroup;

  constructor() { }

  get f() {
    return this.nameform.controls;
  }

  ngOnInit(): void {
  }

  public disable() {
    if (this.nameform) this.nameform.disable();
  }

  public enable() {
    if (this.nameform) this.nameform.enable();
  }

}
