import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'decodeHtml'
})
export class DecodeHtmlPipe implements PipeTransform {

    constructor() { }

    transform(input: string) {
        var doc = new DOMParser().parseFromString(input, "text/html");
        return doc.documentElement.textContent;
    }
}