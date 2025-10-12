import { FormControl, ValidationErrors } from '@angular/forms';

export class CustomValidators {

    static email(c: FormControl): ValidationErrors {
        const regexp = new RegExp(/^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/);
        const isValid = regexp.test(c.value);
        const message = {
            'email': {
                'message': 'Invalid email'
            }
        };
        return isValid ? null : message;
    }

    static phone(c: FormControl): ValidationErrors {
        let isValid = true;
        if (c.value) {
            let length = c.value.length;
            if (length > 0 && length < 12)
                isValid = false;
        }
        const message = {
            'phone': {
                'message': 'Phone number must contain 10 digits'
            }
        };
        return isValid ? null : message;
    }

    static password(c: FormControl): ValidationErrors {
        const regexp = new RegExp('^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])[!-~]{6,}$');
        const isValid = regexp.test(c.value);
        const message = {
            'password': {
                'message': 'The password must contain lower and uppercase letters and a number'
            }
        };
        return isValid ? null : message;
    }

    static username(c: FormControl): ValidationErrors {
        const regexp = new RegExp('^[a-zA-Z0-9]*$');
        const isValid = regexp.test(c.value);
        const message = {
            'password': {
                'message': 'The username can only contain letters and numbers'
            }
        };
        return isValid ? null : message;
    }
}