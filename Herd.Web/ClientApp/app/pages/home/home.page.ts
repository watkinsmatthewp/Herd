import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';

import { AlertService, MastodonService, TimelineAlertService } from "../../services";
import { Status } from "../../models/mastodon";


@Component({
    selector: 'home',
    templateUrl: './home.page.html',
})
export class HomePage implements OnInit {
    statusId: number;
    specificStatus: Status;
    renderSpecificModal: boolean = false;
    loading: boolean = false;
    homeFeed: Status[]; // List of posts for the home feed

    constructor(private activatedRoute: ActivatedRoute, private mastodonService: MastodonService, private alertService: AlertService, private timelineAlert: TimelineAlertService) {}

    getMostRecentHomeFeed() {
        this.loading = true;
        this.alertService.info("Retrieving home timeline ...");
        this.mastodonService.getHomeFeed()
            .finally(() => {
                this.loading = false;
            })
            .subscribe(feed => {
                this.homeFeed = feed;
                this.alertService.success("Finished retrieving home timeline.");
            }, error => {
                this.alertService.error(error.error);
            });
    }

    updateSpecificStatus(statusId: number): void {
        this.loading = true;
        this.renderSpecificModal = false;
        this.alertService.info("Retreiving status info ...");
        this.mastodonService.getStatus(statusId)
            .finally(() => {
                this.loading = false
            })
            .subscribe(data => {
                this.specificStatus = data.Status;
                this.specificStatus.Ancestors = data.StatusContext.Ancestors;
                this.specificStatus.Descendants = data.StatusContext.Descendants;
                this.renderSpecificModal = true;
                this.alertService.success("Retrieved status.")
            }, error => {
                this.alertService.error(error);
            });
    }

    ngOnInit() {
        this.timelineAlert.getMessage().subscribe(alert => {
            let statusId = alert.statusId;
            this.updateSpecificStatus(statusId);
        });
        this.getMostRecentHomeFeed();
    }
}