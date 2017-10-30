import { Injectable } from '@angular/core';
import { Observable } from "rxjs/Observable";

import { User } from '../../models';
import { Account, Status } from '../../models/mastodon';
import { HttpClientService } from '../http-client-service/http-client.service';

/**
    let params = {
        mastodonUserID: "1",
        name: "1",
        followsMastodonUserID: "1",
        followedByMastodonUserID: "1",
        includeFollowers: true,
        includeFollowing: true,
        includeFollowsActiveUser: true,
        includeFollowedByActiveUser: true,
        max: 30
    }
*/
export interface AccountSearchParams {
    mastodonUserID?: string,
    name?: string,
    followsMastodonUserID?: string,
    followedByMastodonUserID?: string,
    includeFollowers?: boolean,
    includeFollowing?: boolean,
    includeFollowsActiveUser?: boolean,
    includeFollowedByActiveUser?: boolean,
    max?: number,
}

@Injectable()
export class AccountService {


    constructor(private httpClient: HttpClientService) { }

    /**
     * Performs a search across accounts
     * @param searchParams
     */
    search(searchParams: AccountSearchParams): Observable<Account[]> {
        let queryString = "?"

        if (searchParams.mastodonUserID)
            queryString += "mastodonUserID=" + searchParams.mastodonUserID
        if (searchParams.name)
            queryString += "&name=" + searchParams.name
        if (searchParams.followsMastodonUserID)
            queryString += "&followsMastodonUserID=" + searchParams.followsMastodonUserID
        if (searchParams.followedByMastodonUserID)
            queryString += "&followedByMastodonUserID=" + searchParams.followedByMastodonUserID
        if (searchParams.includeFollowers)
            queryString += "&includeFollowers=" + searchParams.includeFollowers
        if (searchParams.includeFollowing)
            queryString += "&includeFollowing=" + searchParams.includeFollowing
        if (searchParams.includeFollowsActiveUser)
            queryString += "&includeFollowsActiveUser=" + searchParams.includeFollowsActiveUser
        if (searchParams.includeFollowedByActiveUser)
            queryString += "&includeFollowedByActiveUser=" + searchParams.includeFollowedByActiveUser
        if (searchParams.max)
            queryString += "&max=" + searchParams.max

        return this.httpClient.get('api/mastodon-users/search' + queryString)
            .map(response => { 
                return response.Users as Account[]
            });

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