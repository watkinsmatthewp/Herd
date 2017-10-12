import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';

import { AlertService, MastodonService, TimelineAlertService } from "../../services";
import { Status } from "../../models/mastodon";
import { NotificationsService } from "angular2-notifications";


@Component({
    selector: 'home',
    templateUrl: './home.page.html',
})
export class HomePage implements OnInit {
    statusId: number;
    specificStatus: Status;
    replyStatus: Status;
    renderSpecificModal: boolean = false;
    renderReplyModal: boolean = false;
    loading: boolean = false;
    homeFeed: Status[]; // List of posts for the home feed

    constructor(private activatedRoute: ActivatedRoute, private mastodonService: MastodonService, private timelineAlert: TimelineAlertService, private alertService: NotificationsService) {}

    getMostRecentHomeFeed() {
        this.loading = true;
        let progress = this.alertService.info("Retrieving", "home timeline ...")
        this.mastodonService.getHomeFeed()
            .finally(() => this.loading = false)
            .subscribe(feed => {
                this.alertService.remove(progress.id);
                this.homeFeed = feed;
                this.alertService.success("Finished",  "retrieving home timeline.");
            }, error => {
                this.alertService.error("Error", error.error);
            });
    }

    updateSpecificStatus(statusId: number): void {
        this.loading = true;
        this.renderSpecificModal = false;
        let progress = this.alertService.info("Retrieving" , "status info ...");
        this.mastodonService.getStatus(statusId, true, true)
            .finally(() =>  this.loading = false)
            .subscribe(data => {
                this.alertService.remove(progress.id);
                this.specificStatus = data;
                this.specificStatus.Ancestors = data.Ancestors;
                this.specificStatus.Descendants = data.Descendants;
                this.renderSpecificModal = true;
                this.alertService.success("Finished", "retrieving status.")
            }, error => {
                this.alertService.error("Error", error.error);
            });
    }

    updateReplyStatusModal(statusId: number): void {
        this.loading = true;
        this.renderReplyModal = false;
        let progress = this.alertService.info("Retrieving",  "status info ...");
        this.mastodonService.getStatus(statusId, false, false)
            .finally(() => this.loading = false)
            .subscribe(data => {
                this.alertService.remove(progress.id);
                this.replyStatus = data;
                this.renderReplyModal = true;
                this.alertService.success("Finished", "retrieving status.")
            }, error => {
                this.alertService.error("Error", error.error);
            });
    }

    ngOnInit() {
        this.timelineAlert.getMessage().subscribe(alert => {
            let statusId = alert.statusId;
            if (alert.message === "Update specific status") {
                this.updateSpecificStatus(statusId);
            } else if (alert.message === "Update reply status") {
                this.updateReplyStatusModal(statusId);
            }
        });
        this.getMostRecentHomeFeed();
    }
}