import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { AlertService, MastodonService } from "../../services";
import { Status } from "../../models/mastodon";


@Component({
    selector: 'home',
    templateUrl: './home.page.html',
})
export class HomePage implements OnInit {
    private sub: any;
    private statusId: number;
    specficiStatus: Status;
    loading: boolean = false;

    // List of posts for the home feed
    homeFeed: Status[];

    constructor(private route: ActivatedRoute, private mastodonService: MastodonService, private alertService: AlertService) {
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
        this.sub = this.route.params.subscribe(params => {
            this.statusId = +params['id']; // (+) converts string 'id' to a number

            if (this.statusId) {
                // get the status and its context
                this.mastodonService.getStatus(this.statusId)
                    .subscribe(data => {
                        console.log(data);
                    }, error => {
                        console.log(error);
                    });
                // TODO: open up the modal for it
            }
        });
    }

    ngOnDestroy() {
        this.sub.unsubscribe();
    }

}