import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptionsArgs, RequestOptions, RequestMethod } from '@angular/http';
import 'rxjs/Rx';

import { Status } from '../models/mastodon';
import { HttpClientService } from './http-client.service';

@Injectable()
export class MastodonService {

    constructor(private http: Http, private httpClient: HttpClientService) {}

    // Get a list of posts for the home feed
    getHomeFeed() {
        return this.httpClient.get('api/feed/new_items')
            .map(response => response.RecentFeedItems as Status[])
    }

}