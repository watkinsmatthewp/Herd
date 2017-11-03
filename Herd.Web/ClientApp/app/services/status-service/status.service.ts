import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptionsArgs, RequestOptions, RequestMethod } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/Rx';

import { HttpClientService } from '../http-client-service/http-client.service';
import { Status, UserCard } from '../../models/mastodon';

@Injectable()
export class StatusService {

    constructor(private http: Http, private httpClient: HttpClientService) {}

    // Get a list of posts for the home feed
    getHomeFeed(): Observable<Status[]> {
        let queryString = '?onlyOnActiveUserTimeline=true' 
        return this.httpClient.get('api/mastodon-posts/search' + queryString)
            .map(response => response.Posts as Status[]);
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
     * Returns a status with optional context.
     * @param statusId of status
     * @param includeAncestors optional include ancestor statuses of this status
     * @param includeDescendants optional include descendants statuses of this status
     */
    getStatus(statusId: string, includeAncestors: boolean, includeDescendants: boolean): Observable<Status> {
        let queryString = '?postID=' + statusId
                        + '&includeAncestors=' + includeAncestors
                        + '&includeDescendants=' + includeDescendants;
        return this.httpClient.get('api/mastodon-posts/search' + queryString)
            .map(response => response.Posts[0] as Status);
    }

    /**
     * Make a new status on the home feed
     *
     * @param message
     * @param visibility
     *      Direct = 3
     *      Private = 2
     *      Unlisted = 1
     *      Public = 0
     * @param replayStatusId
     * @param sensitive
     * @param spoilerText
     */
    makeNewStatus(message: string, visibility: number, replyStatusId?: string, sensitive?: boolean, spoilerText?: string, mediaAttachment?: FormData) {
        let body = {
            'message': message,
            'visibility': visibility,
            'replyStatusId': replyStatusId || null,
            'sensitive': sensitive || false,
            'spoilerText': spoilerText || null,
            'mediaAttachment': mediaAttachment || null,
        }
        console.log("Message: " + message);
        console.log("Media: " + mediaAttachment);
        return this.httpClient.post('api/mastodon-posts/new', body);
    }

    /**
     * Reposts the status
     * @param statusID
     * @param repost
     */
    repost(statusID: string, repost: boolean) {
        let body = {
            'statusID': statusID,
            'repost': repost,
        }
        return this.httpClient.post('api/mastodon-posts/repost', body);
    }

    /**
     * Likes the status
     * @param statusID
     * @param like
     */
    like(statusID: string, like: boolean) {
        let body = {
            'statusID': statusID,
            'like': like,
        }
        return this.httpClient.post('api/mastodon-posts/like', body);
    }
}