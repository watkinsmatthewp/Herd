import { Component, OnInit } from '@angular/core';
import { NotificationsService } from "angular2-notifications";

import { AccountService } from "../../services";
import { Account } from '../../models/mastodon';
import { Status } from "../../models/mastodon";
import { Storage } from '../../models';

@Component({
    selector: 'profile',
    templateUrl: './profile.page.html',
    styleUrls: ['./profile.page.css']
})
export class ProfilePage implements OnInit {
    account: Account;
    loading: boolean = false;
    userPosts: Status[] = []; // List of posts from this user
    isFollowing: boolean = false;

    constructor(private accountService: AccountService,
        private localStorage: Storage,
        private alertService: NotificationsService) {
    }

    // Get the posts from this user *STILL NEEDS SOME AUTHOR CHECK SOMEWHERE*
    getMostRecentUserPosts() {
        this.loading = true;
        let progress = this.alertService.info("Retrieving", "user timeline ...")
        this.accountService.getUserFeed()
            .finally(() => this.loading = false)
            .subscribe(feed => {
                this.alertService.remove(progress.id);
                this.userPosts = feed;
                this.alertService.success("Finished", "retrieving user timeline.");
            }, error => {
                this.alertService.error("Error", error.error);
            });
    }

    ngOnInit() {
        this.loading = true;

        // Load the current user to see the profile
        let currentUser = JSON.parse(this.localStorage.getItem('currentUser'));
        let userId = currentUser.MastodonConnection.MastodonUserID;
        this.accountService.getUserById(userId)
            .finally(() => this.loading = false)
            .subscribe(account => {
                this.account = account;
            }, error => {
                this.alertService.error("Error", error.error);
            });

        // Get the posts from this user
        this.getMostRecentUserPosts();
    }
}
