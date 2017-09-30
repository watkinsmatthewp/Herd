import { Component, OnInit } from '@angular/core';
import { Status } from '../shared/models/mastodon/Status';
import { MastodonService } from '../shared/services/mastodon.service';

@Component({
    selector: 'localfeed',
    templateUrl: './localfeed.component.html'
})
export class LocalFeedComponent implements OnInit {

    // List of posts for the localFeed
    localFeed: Status[];

    constructor(private mastodonService: MastodonService) { }

    ngOnInit(): void {
    }
}