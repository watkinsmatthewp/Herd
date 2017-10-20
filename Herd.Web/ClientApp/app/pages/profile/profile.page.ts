import { Component, OnInit, ViewChild } from '@angular/core';
import { NotificationsService } from "angular2-notifications";
import { ActivatedRoute, ParamMap } from '@angular/router';
import { Observable } from "rxjs/Observable";

import { AccountService, TimelineAlertService } from "../../services";
import { Account, Status, UserCard } from '../../models/mastodon';
import { Storage } from '../../models';
import { BsModalComponent } from "ng2-bs3-modal/ng2-bs3-modal";


@Component({
    selector: 'profile',
    templateUrl: './profile.page.html',
    styleUrls: ['./profile.page.css']
})
export class ProfilePage implements OnInit {
    @ViewChild('specificStatusModal')
    specificStatusModal: BsModalComponent;

    @ViewChild('replyStatusModal')
    replyStatusModal: BsModalComponent;

    account: Account;
    loading: boolean = false;
    userPosts: Status[] = []; // List of posts from this user
    following: UserCard[] = [];
    followers: UserCard[] = [];
    isFollowing: boolean = false;

    constructor(
        private accountService: AccountService,
        private localStorage: Storage,
        private alertService: NotificationsService,
        private route: ActivatedRoute,
        private timelineAlert: TimelineAlertService) {
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
                this.alertService.error("Error", error.error);
            });
    }

    // Get the posts from this user *STILL NEEDS SOME AUTHOR CHECK SOMEWHERE*
    getMostRecentUserPosts(userID: string) {
        this.accountService.getUserFeed(userID)
            .subscribe(feed => {
                this.userPosts = feed;
            }, error => {
                this.alertService.error("Error", error.error);
            });
    }

    getFollowers(userID: string) {
        this.accountService.getFollowers(userID)
            .subscribe(users => {
                this.followers = users;
            });
    }

    getFollowing(userID: string) {
        this.accountService.getFollowing(userID)
            .subscribe(users => {
                this.following = users;
            });
    }

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
        this.timelineAlert.getMessage().subscribe(alert => {
            let statusId: string = alert.statusId;
            if (alert.message === "Update specific status") {
                
            } else if (alert.message === "Update reply status") {
                
            }
        });
    }
}
