import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { NotificationsService } from "angular2-notifications";
import { ActivatedRoute, ParamMap } from '@angular/router';
import { Observable } from "rxjs/Observable";

import { AccountService, EventAlertService, StatusService } from "../../services";
import { Account, Status, UserCard } from '../../models/mastodon';
import { Storage, EventAlertEnum } from '../../models';
import { BsModalComponent } from "ng2-bs3-modal/ng2-bs3-modal";
import { TabsetComponent } from "ngx-bootstrap";
import { Subscription } from "rxjs/Rx";


@Component({
    selector: 'profile',
    templateUrl: './profile.page.html',
    styleUrls: ['./profile.page.css']
})
export class ProfilePage implements OnInit, AfterViewInit {
    @ViewChild('staticTabs') staticTabs: TabsetComponent;
    @ViewChild('specificStatusModal') specificStatusModal: BsModalComponent;
    @ViewChild('replyStatusModal') replyStatusModal: BsModalComponent;
    statusId: number;
    specificStatus: Status;
    replyStatus: Status;

    account: Account;
    userPosts: Status[] = []; // List of posts from this user
    following: UserCard[] = [];
    followers: UserCard[] = [];
    isFollowing: boolean = false;

    loading: boolean = false;

    constructor(
        private accountService: AccountService,
        private eventAlertService: EventAlertService,
        private localStorage: Storage,
        private route: ActivatedRoute,
        private statusService: StatusService,
        private toastService: NotificationsService) {
    }

    /**
     * Given a userID, get that Users account
     * @param userId
     */
    getUserAccount(userID: string) {
        this.loading = true;
        this.accountService.getUserByID(userID)
            .finally(() => this.loading = false)
            .subscribe(account => {
                this.account = account;
            }, error => {
                this.toastService.error("Error", error.error);
            });
    }

    /**
     * Get the posts that this user has made
     * @param userID
     */
    getMostRecentUserPosts(userID: string) {
        this.statusService.getUserFeed(userID)
            .subscribe(feed => {
                this.userPosts = feed;
            }, error => {
                this.toastService.error("Error", error.error);
            });
    }

    /**
     * Get the follows of this user
     * @param userID
     */
    getFollowers(userID: string) {
        this.accountService.getFollowers(userID)
            .subscribe(users => {
                this.followers = users;
                console.log("followers", this.followers);
            });
    }

    /**
     * Get who this user is following
     * @param userID
     */
    getFollowing(userID: string) {
        this.accountService.getFollowing(userID)
            .subscribe(users => {
                this.following = users;
                console.log("following", this.following);
            });
    }

    updateSpecificStatus(statusId: string): void {
        this.loading = true;
        let progress = this.toastService.info("Retrieving", "status info ...");
        this.statusService.getStatus(statusId, true, true)
            .finally(() => this.loading = false)
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
        let progress = this.toastService.info("Retrieving", "status info ...");
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

    /**
     * Set up subscriptions for alerts
     */
    ngOnInit() {
        // Monitor Param map 
        this.route.paramMap
            .switchMap((params: ParamMap) => Observable.of(params.get('id') || "-1"))
            .subscribe(userID => {
                this.getUserAccount(userID);
                this.getFollowing(userID);
                this.getFollowers(userID);
                this.getMostRecentUserPosts(userID);
            });

        // Setup subscription to update modals on status click
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
                case EventAlertEnum.UPDATE_FOLLOWING_AND_FOLLOWERS: {
                    this.getFollowers(this.account.MastodonUserId);
                    this.getFollowing(this.account.MastodonUserId);
                    break;
                }
            }
        });
    }

    /**
     * Update default tab
     */
    ngAfterViewInit() {
        this.route.queryParams
            .subscribe(params => {
                let tabIndex: number = params['tabIndex'] || 0;
                setTimeout(() => this.staticTabs.tabs[tabIndex].active = true);
            });
    }
}
