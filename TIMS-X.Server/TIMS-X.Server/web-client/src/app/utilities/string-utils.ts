
export module StringUtils {

    export function toTitleCase(str: string): string {
        /// <summary>Converts a string to title case.</summary>
        /// <param name="str">The string to convert.</param>
        /// <returns>The string in title case.</returns>

        return str
            .toLowerCase()
            .split(' ')
            .map(word => word.charAt(0).toUpperCase() + word.slice(1))
            .join(' ');
    }
}