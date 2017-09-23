import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptionsArgs, RequestOptions, RequestMethod } from '@angular/http';
import 'rxjs/Rx';

import { NumberObject } from '../models/NumberObject';
import { Post } from '../models/Post';
import { HttpClientService } from "./http-client.service";

@Injectable()
export class MastodonService {

    constructor(private http: Http, private httpClient: HttpClientService) {}

    getRandomNumber(): Promise<NumberObject> {
        return this.http.get('/api/RandomNumberApi/GetRandomNumber')
            .map(response => response.json() as NumberObject)
            .toPromise(); 
    }

    // Get a list of posts for the home feed
    getHomeFeed() {
        return this.httpClient.get('api/feed/new_items')
            .map(response => response.RecentFeedItems as Post[])
    }

}