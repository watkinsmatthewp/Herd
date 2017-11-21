import { Component, OnInit, Input, Output, ViewChild, OnChanges, SimpleChanges, EventEmitter } from '@angular/core';

import { ListTypeEnum, Storage, EventAlertEnum } from "../../models";
import { AccountService, EventAlertService } from "../../services";
import { Account, PagedList } from "../../models/mastodon";
import { NotificationsService } from "angular2-notifications";

@Component({
    selector: 'account-list',
    templateUrl: './account-list.component.html',
    styleUrls: ['./account-list.component.css']
})
export class AccountListComponent implements OnInit, OnChanges {
    public listTypeEnum = ListTypeEnum;
    @Input() listType: ListTypeEnum;
    @Input() search: string;
    @Input() userID: string;
    @Output() finishedSearching: EventEmitter<boolean> = new EventEmitter<boolean>();

    @ViewChild('ps') scrollBar: any;
    // Account Lists 
    accountList: PagedList<Account> = new PagedList<Account>();
    // Functions to call 
    getInitialItems: Function;
    getPreviousItems: Function;
    // Loading variable
    loading: boolean = false;


    constructor(private accountService: AccountService, private eventAlertService: EventAlertService,
                private toastService: NotificationsService, private localStorage: Storage) { }

    ngOnInit() {
        // Check for required input - we AT LEAST need to know which types of statuses to get
        if (this.listType < 0) throw new Error("listType is required");

        // Setup subscription to update modals on status click
        this.eventAlertService.getMessage().subscribe(event => {
            switch (event.eventType) {
                case EventAlertEnum.UPDATE_FOLLOWING_AND_FOLLOWERS: {
                    this.getInitialItems();
                    break;
                }
            }
        });

        this.setupFunctions();
        this.getInitialItems();
    }

    /**
     * On state changes do stuff
     * @param changes shows old value vs new value of state change
     */
    ngOnChanges(changes: SimpleChanges): void {
        if ((changes.search && changes.search.previousValue) || (changes.userID && changes.userID.previousValue)) {
            this.getInitialItems();
        }
    }

    private setupFunctions() {
        switch (+this.listType) {
            case ListTypeEnum.PROFILEFOLLOWERS: {
                // Set getInitialItems 
                this.getInitialItems = function (): void {
                    this.accountList.Items = [];
                    this.loading = true;
                    this.accountService.search({ followsMastodonUserID: this.userID, includeFollowsActiveUser: true })
                        .finally(() => this.loading = false)
                        .subscribe(followerList => {
                            this.accountList = followerList;
                        });
                }

                // Set getPreviousItems
                this.getPreviousItems = function (): void {
                    this.loading = true;
                    this.accountService.search({ followsMastodonUserID: this.userID, includeFollowsActiveUser: true, maxID: this.accountList.PageInformation.EarlierPageMaxID })
                        .finally(() => this.loading = false)
                        .subscribe(newUsersList => {
                            if (newUsersList.Items.length > 0) {
                                this.appendItems(this.accountList.Items, newUsersList.Items);
                                this.accountList.PageInformation = newUsersList.PageInformation;
                                this.triggerScroll();
                            }
                        });
                }
                break;
            }
            case ListTypeEnum.PROFILEFOLLOWING: {
                // Set getInitialItems 
                this.getInitialItems = function (): void {
                    this.accountList.Items = [];
                    this.loading = true;
                    this.accountService.search({ followedByMastodonUserID: this.userID, includeFollowedByActiveUser: true })
                        .finally(() => this.loading = false)
                        .subscribe(followingList => {
                            this.accountList = followingList;
                        });
                }

                // Set getPreviousItems
                this.getPreviousItems = function (): void {
                    this.loading = true;
                    this.accountService.search({ followedByMastodonUserID: this.userID, includeFollowedByActiveUser: true, maxID: this.accountList.PageInformation.EarlierPageMaxID })
                        .finally(() => this.loading = false)
                        .subscribe(newUsersList => {
                            if (newUsersList.Items.length > 0) {
                                this.appendItems(this.accountList.Items, newUsersList.Items);
                                this.accountList.PageInformation = newUsersList.PageInformation;
                                this.triggerScroll();
                            }
                        });
                }
                break;
            }
            case ListTypeEnum.SEARCH: {
                // Set getInitialItems 
                this.getInitialItems = function (): void {
                    this.accountList.Items = [];
                    this.loading = true;
                    this.accountService.search({ name: this.search, includeFollowedByActiveUser: true, includeFollowsActiveUser: true })
                        .finally(() => {
                            this.loading = false;
                            this.finishedSearching.emit(true);
                        })
                        .subscribe(userList => {
                            this.accountList = userList;
                        });
                }

                // Set getPreviousItems --> No such thing as infinite scrolling on search
                this.getPreviousItems = function (): void { }
                break;
            }
        }
    }

    /** ----------------------------------------------------------- Button Actions ----------------------------------------------------------- */

    /**
     * Add the new items to main feed array, scroll to top, empty newItems
     */
    viewNewItems() {
        //this.prependItems(this.accountList.Items, this.newaccountList.Items);
        //this.scrollToTop();
        //this.newaccountList = new PagedList<Status>();
    }

    /**
     * Scrolls the status area to the top
     */
    scrollToTop() {
        this.scrollBar.directiveRef.scrollToTop(0, 500);
    }

    /** ----------------------------------------------------------- Infinite Scrolling ----------------------------------------------------------- */

    triggerScroll() {
        this.scrollBar.directiveRef.scrollToY(this.scrollBar.directiveRef.position(true).y - 1);
        this.scrollBar.directiveRef.scrollToY(this.scrollBar.directiveRef.position(true).y + 1);
    }

    /**
     * When reach end of page, load next
     * @param event
     */
    reachEnd(event: any) {
        console.log("event", event);
        // This check prevents this from being called prematurely on page load
        if (event.target.getAttribute('class').indexOf("ps--scrolling-y") >= 0) {
            this.getPreviousItems();
        }
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
