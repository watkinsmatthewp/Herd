import { Component, OnInit, Input } from '@angular/core';

import { MastodonService } from '../../services';
import { UserCard } from "../../models/mastodon";

@Component({
    selector: 'searchresults',
    templateUrl: './searchresults.page.html'
})
export class SearchResultsPage implements OnInit {

    @Input() search: string;
    userCards: UserCard[]; // List of users that the search found

    // Keeping simple for now
    constructor(private mastodonService: MastodonService) { }

    ngOnInit(): void {
        //this.mastodonService.searchUser(search)
            //.subscribe
    }
}