import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-address-formatter',
  templateUrl: './address-formatter.component.html',
  styleUrl: './address-formatter.component.scss'
})
export class AddressFormatterComponent {
  @Input() addressLine1: string;
  @Input() addressLine2: string;
  @Input() city: string;
  @Input() state: string;
  @Input() zipCode: string;

}
