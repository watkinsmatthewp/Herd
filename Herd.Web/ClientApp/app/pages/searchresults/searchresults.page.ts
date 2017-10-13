import { Component, OnInit, Input } from '@angular/core';

import { MastodonService } from '../../services';
import { UserCard } from "../../models/mastodon";

@Component({
    selector: 'searchresults',
    templateUrl: './searchresults.page.html'
})
export class SearchResultsPage implements OnInit {

    search: string;
    userCards: UserCard[]; // List of users that the search found

    // Keeping  it simple for now
    constructor(private mastodonService: MastodonService) { }

    ngOnInit(): void {
        // hard coding string here for testing
        this.mastodonService.searchUser("Thomas Ortiz")
            .subscribe(users => {
                this.userCards = users;
            });
    }

    
}