import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { TabsetComponent } from 'ngx-bootstrap';

import { StatusService, EventAlertService, AccountService } from "../../services";
import { EventAlertEnum, Storage } from '../../models';
import { Status, UserCard } from "../../models/mastodon";
import { NotificationsService } from "angular2-notifications";
import { BsModalComponent } from "ng2-bs3-modal/ng2-bs3-modal";

@Component({
    selector: 'home',
    templateUrl: './home.page.html',
    styleUrls: ['./home.page.css']
})
export class HomePage implements OnInit {
    @ViewChild('specificStatusModal') 
    specificStatusModal: BsModalComponent;
    @ViewChild('replyStatusModal')
    replyStatusModal: BsModalComponent;
    statusId: number;
    specificStatus: Status;
    replyStatus: Status;

    loading: boolean = false;
    homeFeed: Status[] = []; // List of posts for the home feed
    userCard: UserCard;

    constructor(private activatedRoute: ActivatedRoute, private eventAlertService: EventAlertService, private toastService: NotificationsService,
        private statusService: StatusService, private accountService: AccountService, private localStorage: Storage) { }

    onScroll() {
        console.log('scrolled!!')
    }

    getUserCard() {
        let currentUser = JSON.parse(this.localStorage.getItem('currentUser'));
        let userID = currentUser.MastodonConnection.MastodonUserID;
        this.accountService.getUserByID(userID)
            .map(response => response as UserCard)
            .subscribe(usercard => {
                this.userCard = usercard;
                console.log("getUserCard", usercard);
            });
    }

    getMostRecentHomeFeed() {
        this.loading = true;
        let progress = this.toastService.info("Retrieving", "home timeline ...")
        this.statusService.getHomeFeed()
            .finally(() => this.loading = false)
            .subscribe(feed => {
                this.toastService.remove(progress.id);
                this.homeFeed = feed;
                this.toastService.success("Finished",  "retrieving home timeline.");
            }, error => {
                this.toastService.error("Error", error.error);
            });
    }

    updateSpecificStatus(statusId: string): void {
        this.loading = true;
        let progress = this.toastService.info("Retrieving" , "status info ...");
        this.statusService.getStatus(statusId, true, true)
            .finally(() =>  this.loading = false)
            .subscribe(data => {
                this.toastService.remove(progress.id);
                this.specificStatus = data;
                this.specificStatus.Ancestors = data.Ancestors;
                this.specificStatus.Descendants = data.Descendants;
                this.specificStatusModal.open();
                this.toastService.success("Finished", "retrieving status.")
            }, error => {
                this.toastService.error("Error", error.error);
            });
    }

    updateReplyStatusModal(statusId: string): void {
        this.loading = true;
        let progress = this.toastService.info("Retrieving",  "status info ...");
        this.statusService.getStatus(statusId, false, false)
            .finally(() => this.loading = false)
            .subscribe(data => {
                this.toastService.remove(progress.id);
                this.replyStatus = data;
                this.replyStatusModal.open();
                this.toastService.success("Finished", "retrieving status.")
            }, error => {
                this.toastService.error("Error", error.error);
            });
    }

    ngOnInit() {
        this.eventAlertService.getMessage().subscribe(event => {
            switch (event.eventType) {
                case EventAlertEnum.UPDATE_SPECIFIC_STATUS: {
                    let statusID: string = event.statusID;
                    this.updateSpecificStatus(statusID);
                    break;
                }
                case EventAlertEnum.UPDATE_REPLY_STATUS: {
                    let statusID: string = event.statusID;
                    this.updateReplyStatusModal(statusID);
                    break;
                }
            }
        });

        this.getMostRecentHomeFeed();
        this.getUserCard();
    }
}