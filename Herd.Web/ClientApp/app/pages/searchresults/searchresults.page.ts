import { Component, OnInit, Input, ViewChild } from '@angular/core';

import { AccountService } from '../../services';
import { Account } from "../../models/mastodon";

import { ActivatedRoute, Router } from '@angular/router';
import { NotificationsService } from "angular2-notifications";

@Component({
    selector: 'searchresults',
    templateUrl: './searchResults.page.html',
    styleUrls: ['./searchResults.page.css']
})
export class SearchResultsPage implements OnInit {
    @ViewChild('userWrapper') usersWrapper: any;

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


    getMoreUsers() {
        let lastID = this.userCards[this.userCards.length-1].MastodonUserId;
        this.accountService.search({ name: this.search, includeFollowedByActiveUser: true, includeFollowsActiveUser: true, sinceID: lastID })
            .subscribe(new_users => {
                this.appendItems(this.userCards, new_users);
                let currentYPosition = this.usersWrapper.nativeElement.scrollTop;
                this.usersWrapper.nativeElement.scrollTo(0, currentYPosition);
            });
    }

    /**
     * Scrolls the status area to the top
     */
    scrollToTop(tab: string) {
        if (tab === 'users')
            this.usersWrapper.nativeElement.scrollTo(0, 0);

    }

    /**
     * Infinite scroll function that is called
     * when scrolling down and near end of view port
     * @param ev
     */
    onScrollDown(ev: any, tab: string) {
        if (tab === 'users')
            this.getMoreUsers();
    }

    /** Infinite Scrolling Handling */
    addItems(oldItems: any[], newItems: any[], _method: any) {
        oldItems[_method].apply(oldItems, newItems);
    }

    /**
     * Add items to end of list
     * @param startIndex
     * @param endIndex
     */
    appendItems(oldItems: any[], newItems: any[]) {
        this.addItems(oldItems, newItems, 'push');
    }

    /**
     * Add items to beginning of list
     * @param startIndex
     * @param endIndex
     */
    prependItems(oldItems: any[], newItems: any[]) {
        this.addItems(oldItems, newItems, 'unshift');
    }

    
}