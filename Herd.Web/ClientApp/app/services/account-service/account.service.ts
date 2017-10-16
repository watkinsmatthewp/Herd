import { Injectable } from '@angular/core';
import { Observable } from "rxjs/Observable";

import { User } from '../../models';
import { Account, Status } from '../../models/mastodon';
import { HttpClientService } from '../http-client-service/http-client.service';


@Injectable()
export class AccountService {

    constructor(private httpClient: HttpClientService) { }

    // Calls Api to get a user by username 
    getUserById(userId: string) {
        let queryString = "?mastodonUserID=" + userId;
        return this.httpClient.get('api/mastodon-users/search' + queryString)
            .map((response) => response.Users[0] as Account );
    }

    getUserFeed(): Observable<Status[]> {
        return this.httpClient.get('api/feed/user_items')
            .map(response => response.RecentPosts as Status[]);
    }

}