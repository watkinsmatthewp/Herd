import { Component, OnInit } from '@angular/core';

import { AlertService } from '../shared/services/alert.service';
import { MastodonService } from '../shared/services/mastodon.service';
import { Status } from '../shared/models/mastodon/Status';

@Component({
    selector: 'home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
    randomInt: number;
    loading: boolean = false;

    // List of posts for the home feed
    homeFeed: Status[];

    constructor(private mastodonService: MastodonService, private alertService: AlertService) {
    }

    getMostRecentHomeFeed() {
        this.loading = true;
        this.mastodonService.getHomeFeed()
            .finally(() => this.loading = false)
            .subscribe(feed => {
                this.homeFeed = feed;
            }, error => {
                this.alertService.error(error.error);
            });
    }

    ngOnInit() {
        this.getMostRecentHomeFeed();
    }
}