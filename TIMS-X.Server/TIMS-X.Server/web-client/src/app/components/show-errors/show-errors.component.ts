import { Component, Input } from '@angular/core';
import { AbstractControlDirective, AbstractControl } from '@angular/forms';

@Component({
  selector: 'show-errors',
  styles: ['.error-message {display: block;}'],
  template: `
   <div *ngIf="shouldShowErrors()">
     <div *ngFor="let error of listOfErrors()">
        <div class="invalid-feedback error-message" style="color: white;">{{error}}</div>
     </div>
   </div>
 `,
})
export class ShowErrorsComponent {

  private static readonly errorMessages = {
    'required': () => 'required',
    'minlength': (params) => 'The min number of characters is ' + params.requiredLength,
    'maxlength': (params) => 'The max allowed number of characters is ' + params.requiredLength,
    'pattern': (params) => 'The required pattern is: ' + params.requiredPattern,
    'years': (params) => params.message,
    'countryCity': (params) => params.message,
    'uniqueName': (params) => params.message,
    'telephoneNumbers': (params) => params.message,
    'telephoneNumber': (params) => params.message,
    'username': (params) => params.message,
    'password': (params) => params.message,
    'awardCode': (params) => params.message,
    'eventName': (params) => params.message,
    'templateName': (params) => params.message,
    'email': () => "Invalid email address",
    'awardName': (params) => params.message
  };

  @Input()
  public control: AbstractControlDirective | AbstractControl;

  shouldShowErrors(): boolean {
    return this.control &&
      this.control.errors &&
      (this.control.dirty || this.control.touched);
  }

  listOfErrors(): string[] {
    return Object.keys(this.control.errors)
      .map(field => this.getMessage(field, this.control.errors[field]));
  }

  private getMessage(type: string, params: any) {
    return ShowErrorsComponent.errorMessages[type](params);
  }

}