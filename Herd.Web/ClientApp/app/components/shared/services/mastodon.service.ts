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

    // Get the OAuth Token
    OAuth_Url(username: string, instance: string) {
        let headers = new Headers({ 'Content-Type': 'application/json; charset=UTF-8' });
        let options = new RequestOptions({ headers: headers });
        let body = JSON.stringify({
            username: username,
            instance: instance
        });

        return this.http.post('api/auth/OAuth_Url', body, options)
            .map(response => response.json())
            .toPromise();
    }

    // Submit the OAuth Token
    OAuth_Return(OAuthToken: string) {
        let headers = new Headers({ 'Content-Type': 'application/json; charset=UTF-8' });
        let options = new RequestOptions({ headers: headers });
        return this.http.get('api/auth/OAuth_Return/' + OAuthToken, options).toPromise();
    }

    // Logout
    Logout() { return this.http.get('/api/auth/Logout').toPromise(); }

    // Get a list of posts for the home feed
    getHomeFeed() {
        return this.http.get('api/feed/new_items')
            .map(response => response.json() as Post[]) 
            .toPromise();
    }

}