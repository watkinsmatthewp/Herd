﻿import { Injectable } from '@angular/core';
import { Observable } from "rxjs/Observable";

import { User } from '../../models';
import { Account, Status, UserCard } from '../../models/mastodon';
import { HttpClientService } from '../http-client-service/http-client.service';


@Injectable()
export class AccountService {


    constructor(private httpClient: HttpClientService) { }


    search(mastodonUserID: string, name: string, followsMastodonUserID: string, followedByMastodonUserID: string, includeFollowers: boolean,
        includeFollowing: boolean, includeFollowsActiveUser: boolean, includeFollowedByActiveUser: boolean, max: number = 30) {
        let queryString = "?mastodonUserID=" + mastodonUserID
                        + "&name=" + name
                        + "&followsMastodonUserID=" + followsMastodonUserID
                        + "&followedByMastodonUserID=" + followedByMastodonUserID
                        + "&includeFollowers=" + includeFollowers
                        + "&includeFollowing=" + includeFollowing
                        + "&includeFollowsActiveUser=" + includeFollowsActiveUser
                        + "&includeFollowedByActiveUser=" + includeFollowedByActiveUser
                        + "&max=" + max;
        return this.httpClient.get('api.mastodon-users/search' + queryString)
            .map((response) => response.Users as Account);

    }

    /**
     * Calls Api to get a user by username
     * @param userID
     */
    getUserByID(userID: string) {
        let queryString = "?mastodonUserID=" + userID;
        return this.httpClient.get('api/mastodon-users/search' + queryString)
            .map((response) => response.Users[0] as Account );
    }

    /**
     * Searches for a user
     * @param name
     */
    searchForUser(name: string) {
        let queryString = "?name=" + name
                        + "&includeFollowedByActiveUser=" + true
                        + "&includeFollowsActiveUser=" + true;
        return this.httpClient.get('api/mastodon-users/search' + queryString)
            .map(response => response.Users as UserCard[]);
    }

    /**
     * Get the followers of a userID
     * @param userID
     */
    getFollowers(userID: string): Observable<Account[]> {
        let queryString = "?followsMastodonUserID=" + userID
                        + "&includeFollowsActiveUser=" + true;
        return this.httpClient.get('api/mastodon-users/search' + queryString)
            .map(response => response.Users as UserCard[]);
    }

    /**
     * Get who the userID is following
     * @param userID
     */
    getFollowing(userID: string): Observable<Account[]> {
        let queryString = "?followedByMastodonUserID=" + userID
                        + "&includeFollowedByActiveUser=" + true
        return this.httpClient.get('api/mastodon-users/search' + queryString)
            .map(response => response.Users as UserCard[]);
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