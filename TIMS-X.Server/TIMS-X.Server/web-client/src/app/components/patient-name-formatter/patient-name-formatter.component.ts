import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-patient-name-formatter',
  templateUrl: './patient-name-formatter.component.html',
  styleUrls: ['./patient-name-formatter.component.scss']
})
export class PatientNameFormatterComponent implements OnInit {
  @Input() firstname: string;
  @Input() lastname: string;

  constructor() { }

  ngOnInit(): void {
  }

}
