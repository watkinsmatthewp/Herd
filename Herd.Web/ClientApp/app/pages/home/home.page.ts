import { Component, OnInit } from '@angular/core';

import { AlertService, MastodonService } from "../../services";
import { Status } from "../../models/mastodon";

@Component({
    selector: 'home',
    templateUrl: './home.page.html',
})
export class HomePage implements OnInit {
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