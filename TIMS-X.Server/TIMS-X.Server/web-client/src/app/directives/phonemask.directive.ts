import { Directive, HostListener, Input } from '@angular/core'
import { NgControl } from '@angular/forms';

@Directive({
    selector: '[PhoneMask]'
})
export class PhoneMaskDirective {
    constructor(public ngControl: NgControl) { }
    @Input() locale: string = 'US';

    @HostListener('input', ['$event'])
    onKeyDown(event: KeyboardEvent) {
        const input = event.target as HTMLInputElement;
        let trimmed = input.value.replace(/\s+/g, '');
        let newValue = '';
        if (this.locale == 'NZ') {
            if (trimmed.length > 10) {
                trimmed = trimmed.substr(0, 10);
            }

            trimmed = trimmed.replace(/-/g, '');

            let numbers = [];
            if (trimmed.length == 9) {
                numbers.push(trimmed.substr(0, 2));
                numbers.push(trimmed.substr(2, 3));
                numbers.push(trimmed.substr(5, 4));
            }
            else if (trimmed.length == 10) {
                numbers.push(trimmed.substr(0, 3));
                numbers.push(trimmed.substr(3, 3));
                numbers.push(trimmed.substr(6, 4));
            }
            else {
                numbers.push(trimmed.substr(0, 3));
                if (trimmed.substr(3, 2) !== "")
                    numbers.push(trimmed.substr(3, 3));
                if (trimmed.substr(6, 3) != "")
                    numbers.push(trimmed.substr(6, 4));
            }

            newValue = numbers.join(' ');
        }
        else {
            if (trimmed.length > 12) {
                trimmed = trimmed.substr(0, 12);
            }

            trimmed = trimmed.replace(/-/g, '');

            let numbers = [];

            numbers.push(trimmed.substr(0, 3));
            if (trimmed.substr(3, 2) !== "")
                numbers.push(trimmed.substr(3, 3));
            if (trimmed.substr(6, 3) != "")
                numbers.push(trimmed.substr(6, 4));


            newValue = numbers.join('-');
        }

        this.ngControl.control.patchValue(newValue);
    }
}