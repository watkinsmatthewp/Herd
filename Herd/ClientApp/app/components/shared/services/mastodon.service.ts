import { Injectable } from '@angular/core';
import { Http, RequestOptionsArgs, RequestOptions, RequestMethod } from '@angular/http';
import 'rxjs/Rx';

import { NumberObject } from '../model/NumberObject';
import { MastodonAuthenticationObject } from '../model/mastodonAuthentication/MastodonAuthenticationObject'

@Injectable()
export class MastodonService {
    constructor(private http: Http) {

    }

    getRandomNumber() {
        return this.http.get('api/Mastodon/GetRandomNumber')
            .map(response => response.json() as NumberObject)
            .toPromise();
    }

    connectToMastodon(instanceName: string) {
        console.log("connectToMastodon", instanceName);

        return this.http.get('api/Mastodon/ConnectToMastodon/' + instanceName)
            .map(response => response.json())
            .toPromise();
    }

    submitOAuthToken(oAuthToken: string) {
        console.log("oauth token", oAuthToken);

        return this.http.get('api/Mastodon/SubmitOAuthToken/' + oAuthToken)
            .map(respone => respone.json() as MastodonAuthenticationObject)
            .toPromise();
    }

}
