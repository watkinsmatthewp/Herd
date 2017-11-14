import { Component, OnInit, Input, ViewChild } from '@angular/core';

import { AccountService, StatusService } from '../../services';
import { Account, Status, PagedList } from "../../models/mastodon";
import { Storage } from '../../models';

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
    loading: boolean = false;

    userList: PagedList<Account> = new PagedList<Account>();
    statusList: PagedList<Status> = new PagedList<Status>();
    newStatusList: PagedList<Status> = new PagedList<Status>(); // only used if we poll statuses

    constructor(private accountService: AccountService, private route: ActivatedRoute, private router: Router,
                private statusService: StatusService, private toastService: NotificationsService,
                private localStorage: Storage) { }

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
        // Only used if we want to continually check for new statuses with a hashtag search
        // setInterval(() => { this.checkForNewStatuses(); }, 10 * 1000);
    }

    performSearch() {
        this.userList.Items = [];
        this.statusList.Items = [];
        this.finishedSearching = false;
        this.getInitialStatuses();
        this.getInitialUsers();
    }

    isCurrentUser(checkID: string): boolean {
        let currentUser = JSON.parse(this.localStorage.getItem('currentUser'));
        let userID: string = currentUser.MastodonConnection.MastodonUserID;
        if (userID === checkID) {
            return true;
        }
        return false;
    }

    getInitialStatuses() {
        this.statusService.search({ hashtag: this.search })
            .subscribe(statusList => {
                this.statusList = statusList;
            });
    }

    getInitialUsers() {
        this.accountService.search({ name: this.search, includeFollowedByActiveUser: true, includeFollowsActiveUser: true })
            .subscribe(userList => {
                this.finishedSearching = true;
                this.userList = userList;
            });
    }

    getPreviousStatuses() {
        this.loading = true;
        this.statusService.search({ hashtag: this.search, maxID: this.statusList.PageInformation.EarlierPageMaxID })
            .finally(() => this.loading = false)
            .subscribe(newStatusList => {
                this.appendItems(this.statusList.Items, newStatusList.Items);
                this.statusList.PageInformation = newStatusList.PageInformation;
                this.statusesWrapper.nativeElement.scrollTo(0, this.statusesWrapper.nativeElement.scrollTop);
            });
    }

    // Only used if we want to continually check for new statuses with a hashtag search
    //checkForNewStatuses() {
    //    if (this.statuses.length > 0) {
    //        this.statusService.search({ hashtag: this.search, sinceID: this.statuses[0].Id })
    //            .finally(() => this.loading = false)
    //            .subscribe(newItemsList => {
    //                // TODO: Update pagination
    //                this.statuses = newItemsList.Items;
    //            });
    //    } else {
    //        this.getInitialStatuses();
    //    }
    //}

    ///**
    // * Add the new items to main statuses array, scroll to top, empty newItems
    // */
    //viewNewStatuses() {
    //    this.prependItems(this.statusList.Items, this.newStatusList.Items);
    //    this.scrollToTop('statuses');
    //    this.newStatusList = new PagedList<Status>();
    //}








    /**
     * Scrolls the status area to the top
     */
    scrollToTop(tab: string) {
        if (tab === 'statuses')
            this.statusesWrapper.nativeElement.scrollTo(0, 0);
        else if (tab === 'users')
            this.usersWrapper.nativeElement.scrollTo(0, 0);
    }

    /**
     * Infinite scroll function that is called
     * when scrolling down and near end of view port
     * @param ev
     */
    onScrollDown(ev: any, tab: string) {
        if (tab === 'statuses')
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