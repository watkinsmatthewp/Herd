import { Component, OnInit } from '@angular/core';
import { NotificationsService } from "angular2-notifications";

import { AccountService } from "../../services";
import { Account } from '../../models/mastodon';
import { Storage } from '../../models';

@Component({
    selector: 'profile',
    templateUrl: './profile.page.html',
})
export class ProfilePage implements OnInit {
    account: Account;
    loading: boolean = false;
    constructor(private accountService: AccountService,
        private localStorage: Storage,
        private alertService: NotificationsService) {
    }

    ngOnInit() {
        this.loading = true;
        let currentUser = JSON.parse(this.localStorage.getItem('currentUser'));
        let userId = currentUser.MastodonConnection.MastodonUserID;
        console.log("mastodonUserID", userId);
        this.accountService.getUser(userId)
            .finally(() => this.loading = false)
            .subscribe(account => {
                console.log("account", account);
                this.account = account;
            }, error => {
                this.alertService.error("Error", error.error);
            });
    }
}
