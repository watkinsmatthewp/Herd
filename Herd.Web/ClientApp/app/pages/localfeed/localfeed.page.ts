import { Component, OnInit } from '@angular/core';

import { StatusService } from '../../services';
import { Status } from '../../models/mastodon';

@Component({
    selector: 'localfeed',
    templateUrl: './localfeed.page.html'
})
export class LocalFeedPage implements OnInit {

    // List of posts for the localFeed
    localFeed: Status[];

    constructor(private statusService: StatusService) { }

    ngOnInit(): void {
    }
}