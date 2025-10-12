import { Component, Input, OnInit } from '@angular/core';

import { Entities } from '@app/entities/entities';
import ValidationResult = Entities.ValidationResult;
import Severity = Entities.Severity;

@Component({
  selector: 'app-validation-results',
  templateUrl: './validation-results.component.html',
  styleUrls: ['./validation-results.component.scss']
})
export class ValidationResultsComponent implements OnInit {
  @Input() results: ValidationResult[];

  constructor() { }

  ngOnInit(): void {
  }

  getTextColor(v: ValidationResult) {
    if (v.severity == Severity.error)
      return '#FF0000';
    else
      return '#FF7700';
  }
}
