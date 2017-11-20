import { Component, OnInit, Input, ViewChild } from '@angular/core';

import { AccountService, StatusService } from '../../services';
import { Account, Status, PagedList } from "../../models/mastodon";
import { Storage, TimelineTypeEnum } from '../../models';

import { ActivatedRoute, Router } from '@angular/router';
import { NotificationsService } from "angular2-notifications";
import { StatusTimelineComponent } from "../../components/index";

@Component({
    selector: 'searchresults',
    templateUrl: './searchResults.page.html',
    styleUrls: ['./searchResults.page.css']
})
export class SearchResultsPage implements OnInit {
    @ViewChild('usersWrapper') usersWrapper: any;
    @ViewChild('statusTimeline') statusTimeline: StatusTimelineComponent;

    search: string;
    finishedSearching: boolean = false;
    loading: boolean = false;
    timelineType: TimelineTypeEnum = TimelineTypeEnum.SEARCH;

    userList: PagedList<Account> = new PagedList<Account>();

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
                this.statusTimeline.search = this.search;
            });
    }

    performSearch() {
        this.userList.Items = [];
        this.finishedSearching = false;
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

    getInitialUsers() {
        this.accountService.search({ name: this.search, includeFollowedByActiveUser: true, includeFollowsActiveUser: true })
            .subscribe(userList => {
                this.finishedSearching = true;
                this.userList = userList;
            });
    }









    /**
     * Scrolls the status area to the top
     */
    scrollToTop(tab: string) {
        if (tab === 'users')
            this.usersWrapper.nativeElement.scrollTo(0, 0);
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