import { Component, OnInit } from '@angular/core';

import { MastodonService } from "../../services";
import { UserCard } from '../../models/mastodon';


@Component({
    selector: 'usercard',
    templateUrl: './usercard.component.html',
    styleUrls: ['./usercard.component.css']
})
export class UserCardComponent implements OnInit {

    userCard: UserCard;
    isFollowing: boolean;

    constructor() { }

    ngOnInit() {

    }

}