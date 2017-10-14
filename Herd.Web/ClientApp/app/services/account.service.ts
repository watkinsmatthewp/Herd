import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { Observable } from "rxjs/Observable";

import { User } from '../models';
import { Account } from '../models/mastodon';
import { HttpClientService } from "./http-client.service";

 
@Injectable()
export class AccountService {
    constructor(private http: Http, private httpClient: HttpClientService) { }

    // Calls Api to get a user by username
    getUser(userId: string) {
        let queryString = "?userID=" + userId;
        return this.httpClient.get('api/mastodon-users/search' + queryString)
            .map((response) => {
                console.log("Response", response);
                return response.Users[0] as Account;
            });
    }

}