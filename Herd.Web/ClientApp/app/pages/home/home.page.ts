import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';

import { MastodonService, TimelineAlertService } from "../../services";
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

    constructor(private activatedRoute: ActivatedRoute, private toastService: NotificationsService,
                private mastodonService: MastodonService, private timelineAlert: TimelineAlertService) { }

    getMostRecentHomeFeed() {
        this.loading = true;
        let progress = this.toastService.info("Retrieving", "home timeline ...")
        this.mastodonService.getHomeFeed()
            .finally(() => this.loading = false)
            .subscribe(feed => {
                this.toastService.remove(progress.id);
                this.homeFeed = feed;
                this.toastService.success("Finished",  "retrieving home timeline.");
            }, error => {
                this.toastService.error("Error", error.error);
            });
    }

    updateSpecificStatus(statusId: number): void {
        this.loading = true;
        this.renderSpecificModal = false;
        let progress = this.toastService.info("Retrieving" , "status info ...");
        this.mastodonService.getStatus(statusId, true, true)
            .finally(() =>  this.loading = false)
            .subscribe(data => {
                this.toastService.remove(progress.id);
                this.specificStatus = data;
                this.specificStatus.Ancestors = data.Ancestors;
                this.specificStatus.Descendants = data.Descendants;
                this.renderSpecificModal = true;
                this.toastService.success("Finished", "retrieving status.")
            }, error => {
                this.toastService.error("Error", error.error);
            });
    }

    updateReplyStatusModal(statusId: number): void {
        this.loading = true;
        this.renderReplyModal = false;
        let progress = this.toastService.info("Retrieving",  "status info ...");
        this.mastodonService.getStatus(statusId, false, false)
            .finally(() => this.loading = false)
            .subscribe(data => {
                this.toastService.remove(progress.id);
                this.replyStatus = data;
                this.renderReplyModal = true;
                this.toastService.success("Finished", "retrieving status.")
            }, error => {
                this.toastService.error("Error", error.error);
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