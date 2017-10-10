import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';

import { AlertService, MastodonService } from "../../services";
import { Status } from "../../models/mastodon";


@Component({
    selector: 'home',
    templateUrl: './home.page.html',
})
export class HomePage implements OnInit {
    sub: any;
    statusId: number;
    specificStatus: Status;
    renderSpecificModal: boolean = false;
    loading: boolean = false;
    homeFeed: Status[]; // List of posts for the home feed

    constructor(private activatedRoute: ActivatedRoute, private mastodonService: MastodonService, private alertService: AlertService) {
        // Detects changes in the url and grab new data
        this.activatedRoute.params.subscribe(params => {
            const statusId = params['id'];
            this.renderSpecificModal = false;
            this.checkForStatusId();
        }); 
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

    checkForStatusId() {
        this.sub = this.activatedRoute.params.subscribe(params => {
            this.statusId = +params['id']; // (+) converts string 'id' to a number

            // If a status id was passed in then show that status
            if (this.statusId) {
                this.mastodonService.getStatus(this.statusId)
                    .subscribe(data => {
                        this.specificStatus = data.Status;
                        this.specificStatus.Ancestors = data.StatusContext.Ancestors;
                        this.specificStatus.Descendants = data.StatusContext.Descendants;
                        this.renderSpecificModal = true;
                    }, error => {
                        this.alertService.error(error);
                    });
            }
        });
    }

    ngOnInit() {
        this.checkForStatusId();
        this.getMostRecentHomeFeed();
    }

    ngOnDestroy() {
        this.sub.unsubscribe();
    }

}