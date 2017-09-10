import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import 'rxjs/Rx';

import { NumberObject } from '../model/NumberObject';

@Injectable()
export class MastodonService {
    constructor(private http: Http) {

    }

    getRandomNumber() {
        return this.http.get('api/Mastodon/GetRandomNumber')
            .map(response => response.json() as NumberObject)
            .toPromise();
    }

}
