import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';

import { NotificationsService } from "angular2-notifications";
import { Router } from '@angular/router';

import { AccountService, EventAlertService } from "../../services";
import { Account } from "../../models/mastodon";
import { Storage } from '../../models';

@Component({
    selector: 'profile-updater',
    templateUrl: './profile-updater.component.html',
    styleUrls: ['./profile-updater.component.css']
})
export class ProfileUpdaterComponent implements OnInit {
    account: Account;
    userID: string;
    model: any = {
        display: "",
        bio: "",
    };

    constructor(private router: Router, private accountService: AccountService, private eventAlertService: EventAlertService,
                private toastService: NotificationsService,  private localStorage: Storage) { }

    ngOnInit() {
        let currentUser = JSON.parse(this.localStorage.getItem('currentUser'));
        let userID = currentUser.MastodonConnection.MastodonUserID;
        this.accountService.search({ mastodonUserID: userID, includeFollowedByActiveUser: true })
            .map(response => response.Items[0] as Account)
            .subscribe(account => {
                this.account = account;
                this.model.display = this.account.MastodonDisplayName;

                // Strip HTML tags
                this.model.bio = String(this.account.MastodonShortBio).replace(/<[^>]+>/gm, '');

            }, error => {
                this.toastService.error("Error", error.error);
            });
    }

    onHeaderFileChange(event: any) {
        let fileList: FileList = event.target.files;
        if (fileList.length > 0) {
            let file = fileList[0];
            this.model.headerFile = file;
            this.model.headerFilename = file.name;

            let reader = new FileReader();
            reader.readAsDataURL(event.target.files[0]);
            reader.onload = (e: any) => {
                //this.model.headerBase64 = reader.result.split("base64,")[1];
            }
        }
    }

    onAvatarFileChange(event: any) {
        let fileList: FileList = event.target.files;
        if (fileList.length > 0) {
            let file = fileList[0];
            this.model.avatarFile = file;
            this.model.avatarFilename = file.name;

            let reader = new FileReader();
            reader.readAsDataURL(event.target.files[0]);
            reader.onload = (e: any) => {
                //this.model.avatarBase64 = reader.result.split("base64,")[1];
            }
        }
    }

    submitUpdate(form: NgForm) {
        if (this.model.diplay === "")
            this.model.diplay = null;
        if (this.model.bio === "")
            this.model.bio = null;

        this.accountService.updateUserMastodonAccount(this.model.display, this.model.bio,
            this.model.avatarFile, this.model.headerFile)
            .finally(() => {

            })
            .subscribe(response => {
                this.toastService.success("Successfully", "Updated Profile");
                this.router.navigateByUrl('/profile/' + this.account.MastodonUserId);
            }, error => {
                this.toastService.error(error.error);
            });

    }

}