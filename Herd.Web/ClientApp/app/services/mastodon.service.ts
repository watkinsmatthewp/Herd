import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptionsArgs, RequestOptions, RequestMethod } from '@angular/http';
import 'rxjs/Rx';

import { HttpClientService } from './http-client.service';
import { Status } from '../models/mastodon';

@Injectable()
export class MastodonService {

    constructor(private http: Http, private httpClient: HttpClientService) {}

    // Get a list of posts for the home feed
    getHomeFeed() {
        return this.httpClient.get('api/feed/new_items')
            .map(response => response.RecentFeedItems as Status[])
    }

    // Make a new post on the home feed
    makeNewPost(message: string) {
        let body = {
            'message': message
        }
        return this.httpClient.post('api/feed/new_post', body);
    }

}