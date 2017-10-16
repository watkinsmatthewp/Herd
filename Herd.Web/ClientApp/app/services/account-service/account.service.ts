import { Injectable } from '@angular/core';
import { Observable } from "rxjs/Observable";

import { User } from '../../models';
import { Account } from '../../models/mastodon';
import { HttpClientService } from '../http-client-service/http-client.service';


@Injectable()
export class AccountService {


    constructor(private httpClient: HttpClientService) { }

    /**
     * Calls Api to get a user by username
     * @param userID
     */
    getUserById(userID: string) {
        let queryString = "?mastodonUserID=" + userID;
        return this.httpClient.get('api/mastodon-users/search' + queryString)
            .map((response) => response.Users[0] as Account );
    }

    /**
     * Calls api to follow/unfollow a user by id
     * @param userID
     * @param followUser
     */
    followUser(userID: string, followUser: boolean) {
        let body = {
            'mastodonUserID': userID,
            'followUser': followUser,
        }
        return this.httpClient.post('api/mastodon-users/follow', body);
    }

}