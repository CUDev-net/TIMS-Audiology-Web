import { Directive, ElementRef, HostListener } from '@angular/core';

@Directive({
    selector: '[PatientName]'
})
export class PatientNameDirective {
    inputElement: HTMLElement;
    constructor(public el: ElementRef) {
        this.inputElement = el.nativeElement;
    }

    @HostListener('keydown', ['$event'])
    onKeyDown(e: KeyboardEvent) {
        if (this.isRestrictedKey(e.key)) {
            e.preventDefault();
        }
        if (this.isNumeric(e.key)) e.preventDefault();
    }

    @HostListener('paste', ['$event'])
    onPaste(event: ClipboardEvent) {
        event.preventDefault();
        const pastedInput: string = event.clipboardData
            .getData('text/plain');

        for (const c of pastedInput) {
            if (this.isRestrictedKey(c) || this.isNumeric(c))
                return;
        }

        document.execCommand('insertText', false, pastedInput);
    }

    isNumeric(val: any): boolean {
        return !(val instanceof Array) && (val - parseFloat(val) + 1) >= 0;
    }

    isRestrictedKey(key) {
        if (key == '-' || key == ' ' || key == '\\' ||
            key == '&' || key == '(' || key == ')' ||
            key == '+' || key == '/' || key == '?' ||
            key == ',' || key == ';' || key == '.' ||
            key == '!' || key == '#' || key == '"') {
            return true;
        }
        return false;
    }
}