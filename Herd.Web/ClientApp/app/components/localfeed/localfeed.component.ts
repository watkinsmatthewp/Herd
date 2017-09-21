import { Component, OnInit } from '@angular/core';
import { Post } from '../shared/models/Post';
import { MastodonService } from '../shared/services/mastodon.service';

@Component({
    selector: 'localfeed',
    templateUrl: './localfeed.component.html'
})
export class LocalFeedComponent implements OnInit {

    // List of posts for the localFeed
    posts: Post[];

    constructor(private mastodonService: MastodonService) { }

    ngOnInit(): void {
        throw new Error("Method not implemented.");
    }
}