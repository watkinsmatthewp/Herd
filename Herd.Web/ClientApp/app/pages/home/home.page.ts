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
        this.mastodonService.getHomeFeed()
            .finally(() => this.loading = false)
            .subscribe(feed => {
                this.homeFeed = feed;
            }, error => {
                this.alertService.error(error.error);
            });
    }

    updateSpecificStatus(statusId: number): void {
        this.loading = true;
        this.renderSpecificModal = false;
        this.mastodonService.getStatus(statusId)
            .finally(() => this.loading = false)
            .subscribe(data => {
                this.specificStatus = data.Status;
                this.specificStatus.Ancestors = data.StatusContext.Ancestors;
                this.specificStatus.Descendants = data.StatusContext.Descendants;
                this.renderSpecificModal = true;
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