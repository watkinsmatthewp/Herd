import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'stripHtmlTags'
})
export class StripHtmlTagsPipe implements PipeTransform {

    constructor() { }

    transform(input: string) {
        return String(input).replace(/<[^>]+>/gm, '');
    }
}