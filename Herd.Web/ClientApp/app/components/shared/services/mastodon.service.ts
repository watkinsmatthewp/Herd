import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptionsArgs, RequestOptions, RequestMethod } from '@angular/http';
import 'rxjs/Rx';

import { NumberObject } from '../models/NumberObject';
import { Post } from '../models/Post';

@Injectable()
export class MastodonService {

    constructor(private http: Http) {}

    getRandomNumber(): Promise<NumberObject> {
        return this.http.get('/api/RandomNumberApi/GetRandomNumber')
            .map(response => response.json() as NumberObject)
            .toPromise();
    }

    // Get a list of posts for the home feed
    getHomeFeed() {
        return this.http.get('api/feed/new_items')
            .map(response => response.json() as Post[])
    }

}