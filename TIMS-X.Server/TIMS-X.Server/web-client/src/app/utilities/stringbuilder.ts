import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class StringBuilder {
    /// <summary>Utility for appending text without all the ifs and thens.</summary>

    parts: string[] = [];

    append(value: any, appendWithPrefix?: string, appendIfNull?: boolean);
    append(value: any) {
        /// <summary>Appends text to the string builder.</summary>
        /// <param name="value">The text to append.</param>

        var appendIfNull = false,
            appendWithPrefix = null,
            parts = this.parts;

        // Get args for this function call. Expected args:
        // value: string (always first)
        // appendIfNull: bool
        // appendWithPrefix: string

        for (var i = 1; i < arguments.length; i++) {
            if (typeof arguments[i] === 'boolean') appendIfNull = arguments[i];
            if (typeof arguments[i] === 'string') appendWithPrefix = arguments[i];
        }

        if (appendIfNull || value) {
            if (appendWithPrefix && parts.length > 0) {
                parts.push(appendWithPrefix);
            }
            parts.push(value);
        }
    }

    toString() {
        /// <summary>Render the string builder output.</summary>

        return this.parts.join('');
    }
}
