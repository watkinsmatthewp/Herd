import { Injectable } from '@angular/core';
import { Observable } from "rxjs/Observable";

import { User } from '../../models';
import { Account, Status, UserCard } from '../../models/mastodon';
import { HttpClientService } from '../http-client-service/http-client.service';


@Injectable()
export class AccountService {


    constructor(private httpClient: HttpClientService) { }

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
     * Get the posts that the active user has made
     */
    getUserFeed(userID: string): Observable<Status[]> {
        let queryString = "?authorMastodonUserID=" + userID;
        return this.httpClient.get('api/mastodon-posts/search' + queryString)
            .map(response => response.Posts as Status[]);
    }

    /**
     * Get the followers of a userID
     * @param userID
     */
    getFollowers(userID: string): Observable<Account[]> {
        let queryString = "?followsMastodonUserID=" + userID;
        return this.httpClient.get('api/mastodon-users/search' + queryString)
            .map(response => response.Users as UserCard[]);
    }

    /**
     * Get who the userID is following
     * @param userID
     */
    getFollowing(userID: string): Observable<Account[]> {
        let queryString = "?followedByMastodonUserID=" + userID;
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