import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptionsArgs, RequestOptions, RequestMethod } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/Rx';

import { HttpClientService } from '../http-client-service/http-client.service';
import { Status, PagedList } from '../../models/mastodon';

/**
    let params = {
        onlyOnActiveUserTimeline: true,
        authorMastodonUserID: "1",
        postID: "1",
        hashtag: "#hello",
        includeAncestors: true,
        includeDescendants: true,
        max: 30,
        maxID: "5000",
        sinceID: "4000"
    }
*/
export interface StatusSearchParams {
    onlyOnActiveUserTimeline?: boolean,
    authorMastodonUserID?: string,
    postID?: string,
    hashtag?: string,
    includeAncestors?: boolean,
    includeDescendants?: boolean,
    max?: number,
    maxID?: string,
    sinceID?: string
}

@Injectable()
export class StatusService {

    constructor(private http: Http, private httpClient: HttpClientService) {}

    search(searchParams: StatusSearchParams): Observable<PagedList<Status>> {
        let queryString = "?"
            
        if (searchParams.onlyOnActiveUserTimeline)
            queryString += "onlyOnActiveUserTimeline=" + searchParams.onlyOnActiveUserTimeline
        if (searchParams.authorMastodonUserID)
            queryString += "&authorMastodonUserID=" + searchParams.authorMastodonUserID
        if (searchParams.postID)
            queryString += "&postID=" + searchParams.postID
        if (searchParams.hashtag)
            queryString += "&hashtag=" + searchParams.hashtag
        if (searchParams.includeAncestors)
            queryString += "&includeAncestors=" + searchParams.includeAncestors
        if (searchParams.includeDescendants)
            queryString += "&includeDescendants=" + searchParams.includeDescendants
        if (searchParams.maxID)
            queryString += "&maxID=" + searchParams.maxID
        if (searchParams.sinceID)
            queryString += "&sinceID=" + searchParams.sinceID
        if (searchParams.max)
            queryString += "&max=" + searchParams.max

        return this.httpClient.get('api/mastodon-posts/search' + queryString)
            .map(response => {
                console.log('status-response', response);
                let pagedList = response as PagedList<Status>;
                console.log('\tStatus-PagedList', pagedList);
                return pagedList;
                //return response.Posts as Status[];
            });
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
    makeNewStatus(message: string, visibility: number, replyStatusId?: string, sensitive?: boolean, spoilerText?: string, attachment?: File) {
        const formData: FormData = new FormData();
        formData.append('message', message);
        formData.append('visibility', String(visibility));
        if (replyStatusId)
            formData.append('replyStatusId', replyStatusId);
        if (sensitive)
            formData.append('sensitive', String(sensitive));
        if (spoilerText)
            formData.append('spoilerText', spoilerText);
        if (attachment)
            formData.append('attachment', attachment);

        return this.httpClient.postForm('api/mastodon-posts/new', formData);
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