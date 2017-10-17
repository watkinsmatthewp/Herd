import { Component, OnInit, Input } from '@angular/core';

import { AccountService } from '../../services';
import { UserCard } from "../../models/mastodon";

import { ActivatedRoute, Router } from '@angular/router';
import { NotificationsService } from "angular2-notifications";

@Component({
    selector: 'searchresults',
    templateUrl: './searchResults.page.html'
})
export class SearchResultsPage implements OnInit {

    search: string;
    userCards: UserCard[]; // List of users that the search found

    // Keeping  it simple for now
    constructor(private accountService: AccountService, private route: ActivatedRoute, private router: Router, private toastService: NotificationsService) { }

    performSearch() {
        let progress = this.toastService.info("Searching for", this.search + " ...")
        this.accountService.searchUser(this.search)
            .subscribe(users => {
                this.toastService.remove(progress.id);
                this.userCards = users;
                this.toastService.success("Finished", "search for " + this.search + ".");
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