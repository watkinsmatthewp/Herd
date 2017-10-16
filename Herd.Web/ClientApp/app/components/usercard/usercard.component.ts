import { Component, OnInit, Input } from '@angular/core';

import { MastodonService, TimelineAlertService, AccountService } from "../../services";
import { NotificationsService } from "angular2-notifications";
import { UserCard } from '../../models/mastodon';

@Component({
    selector: 'usercard',
    templateUrl: './usercard.component.html',
    styleUrls: ['./usercard.component.css']
})
export class UserCardComponent implements OnInit {
    @Input() userCard: UserCard;
    isFollowing: boolean = false;

    constructor(private timelineAlert: TimelineAlertService, private mastodonService: MastodonService,
        private accountService: AccountService, private toastService: NotificationsService) { }

    ngOnInit() {
        if (this.userCard.IsFollowedByActiveUser === true) {
            this.isFollowing = true;
        }
    }

    togglefollow(): void {
        var action = false;
        if (this.userCard.IsFollowedByActiveUser === false)
            action = true;
        this.accountService.followUser(String(this.userCard.MastodonUserId), action)
            .subscribe(response => {
                this.toastService.success("Successfully", "changed following relationship.");
            }, error => {
                this.toastService.error(error.error);
            });
    }

}