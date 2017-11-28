import { Component, OnInit, Input, ViewEncapsulation } from '@angular/core';

import { AccountService, EventAlertService } from "../../services";
import { NotificationsService } from "angular2-notifications";
import { Account } from '../../models/mastodon';
import { EventAlertEnum } from '../../models'

@Component({
    selector: 'usercard',
    templateUrl: './usercard.component.html',
    styleUrls: ['./usercard.component.css']
})
export class UserCardComponent implements OnInit {
    @Input() userCard: Account;
    @Input() fillWidth: boolean = false;
    @Input() size: string;
    @Input() showBio: boolean = false;
    @Input() showActions: boolean = false;
    @Input() showFollowButton: boolean = false;
    @Input() showQuickInfo: boolean = false;

    isFollowing: boolean = false;
    followUnfollowText: string = "Following";

    constructor(private accountService: AccountService, private eventAlertService: EventAlertService, private toastService: NotificationsService) { }

    ngOnInit() {
        if (this.userCard.IsFollowedByActiveUser) {
            this.isFollowing = true;
        }
    }

    togglefollow(): void {
        this.accountService.followUser(String(this.userCard.MastodonUserId), !this.isFollowing)
            .subscribe(response => { 
                this.isFollowing = !this.isFollowing;
                this.eventAlertService.addEvent(EventAlertEnum.UPDATE_FOLLOWING_AND_FOLLOWERS);
                this.toastService.success("Successfully", "changed relationship.");
            }, error => {
                this.toastService.error(error.error);
            });
    }
}