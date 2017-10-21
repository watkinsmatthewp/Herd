import { Component, OnInit, Input, ViewEncapsulation } from '@angular/core';

import { TimelineAlertService, AccountService } from "../../services";
import { NotificationsService } from "angular2-notifications";
import { UserCard } from '../../models/mastodon';

@Component({
    selector: 'usercard',
    templateUrl: './usercard.component.html',
    styleUrls: ['./usercard.component.css']
})
export class UserCardComponent implements OnInit {
    @Input() userCard: UserCard;
    @Input() fillWidth: boolean = false;
    @Input() showBio: boolean = false;
    @Input() showActions: boolean = false;
    @Input() showFollowButton: boolean = false;
    @Input() showQuickInfo: boolean = false;

    isFollowing: boolean = false;
    followUnfollowText: string = "Following";

    constructor(private timelineAlert: TimelineAlertService,
        private accountService: AccountService, private toastService: NotificationsService) { }

    ngOnInit() {
        if (this.userCard.IsFollowedByActiveUser) {
            this.isFollowing = true;
        }
    }

    togglefollow(): void {
        this.accountService.followUser(String(this.userCard.MastodonUserId), !this.isFollowing)
            .subscribe(response => {
                this.isFollowing = !this.isFollowing;
                this.toastService.success("Successfully", "changed relationship.");
            }, error => {
                this.toastService.error(error.error);
            });
    }

    mouseEnter(div: string) {
        console.log("mouse enter : " + div);
    }

    mouseLeave(div: string) {
        console.log('mouse leave :' + div);
    }

}