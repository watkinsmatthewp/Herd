import { Component, OnInit } from '@angular/core';
import { Status } from '../../models/mastodon/Status';
import { MastodonService } from "../../services/mastodon.service";

@Component({
    selector: 'localfeed',
    templateUrl: './localfeed.page.html'
})
export class LocalFeedPage implements OnInit {

    // List of posts for the localFeed
    localFeed: Status[];

    constructor(private mastodonService: MastodonService) { }

    ngOnInit(): void {
    }
}