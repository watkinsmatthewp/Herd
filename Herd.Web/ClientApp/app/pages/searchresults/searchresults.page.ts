import { Component, OnInit, Input, ViewChild } from '@angular/core';

import { AccountService, StatusService } from '../../services';
import { Account, Status } from "../../models/mastodon";

import { ActivatedRoute, Router } from '@angular/router';
import { NotificationsService } from "angular2-notifications";

@Component({
    selector: 'searchresults',
    templateUrl: './searchResults.page.html',
    styleUrls: ['./searchResults.page.css']
})
export class SearchResultsPage implements OnInit {
    @ViewChild('usersWrapper') usersWrapper: any;
    @ViewChild('statusesWrapper') statusesWrapper: any;

    search: string;
    finishedSearching: boolean = false;
    userCards: Account[] = []; // List of users that the search found
    statuses: Status[] = [];
    new_statuses: Status[] = [];
    loading: boolean = false;

    // Keeping  it simple for now
    constructor(private accountService: AccountService, private route: ActivatedRoute, private router: Router,
                private statusService: StatusService, private toastService: NotificationsService) { }

    ngOnInit(): void {
        this.route
            .queryParams
            .subscribe(params => {
                this.search = params['searchString'] || "John";
                if (this.search.indexOf("#") >= 0) {
                    this.search = this.search.replace("#", "");
                }
                this.performSearch();
            });
        // setInterval(() => { this.checkForNewStatuses(); }, 10 * 1000);
    }

    performSearch() {
        this.userCards = [];
        this.statuses = [];
        this.finishedSearching = false;
        this.getInitialStatuses();
        this.getInitialUsers();
    }

    getInitialStatuses() {
        this.statusService.search({ hashtag: this.search })
            .subscribe(statuses => {
                this.statuses = statuses;
            });
    }

    getInitialUsers() {
        this.accountService.search({ name: this.search, includeFollowedByActiveUser: true, includeFollowsActiveUser: true })
            .subscribe(users => {
                this.finishedSearching = true;
                this.userCards = users;
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


    checkForNewStatuses() {
        if (this.statuses.length > 0) {
            this.statusService.search({ hashtag: this.search, sinceID: this.statuses[0].Id })
                .finally(() => this.loading = false)
                .subscribe(newItems => {
                    this.statuses = newItems;
                });
        } else {
            this.getInitialStatuses();
        }
    }

    getPreviousStatuses() {
        this.loading = true;
        this.statusService.search({ hashtag: this.search, maxID: this.statuses[this.statuses.length - 1].Id })
            .finally(() => this.loading = false)
            .subscribe(new_items => {
                this.appendItems(this.statuses, new_items);
                let currentYPosition = this.statusesWrapper.nativeElement.scrollTop;
                this.statusesWrapper.nativeElement.scrollTo(0, currentYPosition);
            });
    }

    /**
     * Add the new items to main statuses array, scroll to top, empty newItems
     */
    viewNewStatuses() {
        this.prependItems(this.statuses, this.new_statuses);
        this.scrollToTop('statuses');
        this.new_statuses = [];
    }








    /**
     * Scrolls the status area to the top
     */
    scrollToTop(tab: string) {
        if (tab === 'users')
            this.usersWrapper.nativeElement.scrollTo(0, 0);
        else if (tab === 'statuses')
            this.statusesWrapper.nativeElement.scrollTo(0, 0);
    }

    /**
     * Infinite scroll function that is called
     * when scrolling down and near end of view port
     * @param ev
     */
    onScrollDown(ev: any, tab: string) {
        if (tab === 'users')
            this.getMoreUsers();
        else if (tab === 'statuses')
            this.getPreviousStatuses();
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