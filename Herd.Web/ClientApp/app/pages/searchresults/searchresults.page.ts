import { Component, OnInit, Input } from '@angular/core';

import { AccountService } from '../../services';
import { Account } from "../../models/mastodon";

import { ActivatedRoute, Router } from '@angular/router';
import { NotificationsService } from "angular2-notifications";

@Component({
    selector: 'searchresults',
    templateUrl: './searchResults.page.html'
})
export class SearchResultsPage implements OnInit {

    search: string;
    haveSearchResults: boolean = false;
    finishedSearching: boolean = false;
    userCards: Account[]; // List of users that the search found

    // Keeping  it simple for now
    constructor(private accountService: AccountService, private route: ActivatedRoute, private router: Router, private toastService: NotificationsService) { }

    performSearch() {
        this.haveSearchResults = false;
        this.finishedSearching = false;
        let progress = this.toastService.info("Searching for", this.search + " ...", { timeOut: 0 })
        this.accountService.search({ name: this.search, includeFollowedByActiveUser: true, includeFollowsActiveUser: true })
            .subscribe(users => {
                this.toastService.remove(progress.id);
                if (users.length > 0) {
                    this.haveSearchResults = true;
                }
                this.finishedSearching = true;
                this.userCards = users;
            });
    }

    ngOnInit(): void {
        this.route
            .queryParams
            .subscribe(params => {
                this.search = params['searchString'] || "John";
                this.performSearch();
            });
    }
    
}