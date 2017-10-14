import { Component, OnInit, Input } from '@angular/core';

import { MastodonService, TimelineAlertService } from "../../services";
import { UserCard } from '../../models/mastodon';

@Component({
    selector: 'usercard',
    templateUrl: './usercard.component.html',
    styleUrls: ['./usercard.component.css']
})
export class UserCardComponent implements OnInit {
    @Input() userCard: UserCard;
    isFollowing: boolean = false;

    constructor(private timelineAlert: TimelineAlertService) {  }

    ngOnInit() {
        if (this.userCard.FollowsCurrentUser === true) {
            this.isFollowing = true;
        }
    }

    follow() {

    }

}