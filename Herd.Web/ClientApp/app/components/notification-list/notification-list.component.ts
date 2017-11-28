import { Component, OnInit, Input, Output, ViewChild, OnChanges, SimpleChanges, EventEmitter } from '@angular/core';

import { ListTypeEnum, Storage, EventAlertEnum } from "../../models";
import { AccountService, EventAlertService } from "../../services";
import { Account, PagedList, MastodonNotification } from "../../models/mastodon";
import { NotificationsService } from "angular2-notifications";

@Component({
    selector: 'notification-list',
    templateUrl: './notification-list.component.html',
    styleUrls: ['./notification-list.component.css']
})
export class NotificationListComponent implements OnInit, OnChanges {


    @ViewChild('ps') private scrollBar: any;

    // Notifications List
    notificationList: PagedList<MastodonNotification> = new PagedList<MastodonNotification>();

    // Loading Variable
    private loading: boolean = false;

    constructor(private accountService: AccountService, private eventAlertService: EventAlertService,
        private toastService: NotificationsService, private localStorage: Storage) { }

    ngOnInit() {
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


    private getInitialItems() {
        this.notificationList.Items = [];
        this.loading = true;
        this.accountService.getHerdNotifications({ includeAncestors: true, includeDescendants: true })
            .finally(() => this.loading = false)
            .subscribe(notificationList => {
                this.notificationList = notificationList;
            }, error => {
                this.toastService.error("Error", error.error);
            });
    }

    private getPreviousItems() {
        this.loading = true;
        this.accountService.getHerdNotifications({ includeAncestors: true, includeDescendants: true, maxID: this.notificationList.PageInformation.EarlierPageMaxID })
            .finally(() => this.loading = false)
            .subscribe(newNotificationList => {
                if (newNotificationList.Items.length > 0) {
                    this.appendItems(this.notificationList.Items, newNotificationList.Items);
                    this.notificationList.PageInformation = newNotificationList.PageInformation;
                    this.triggerScroll();
                }
            }, error => {
                this.toastService.error("Error", error.error);
            });
    }


    /** ----------------------------------------------------------- Button Actions ----------------------------------------------------------- */

    /**
     * Add the new items to main feed array, scroll to top, empty newItems
     */
    private viewNewItems() {
        
    }

    /**
     * Scrolls the status area to the top
     */
    private scrollToTop() {
        this.scrollBar.directiveRef.scrollToTop(0, 500);
    }

    /** ----------------------------------------------------------- Infinite Scrolling ----------------------------------------------------------- */

    private triggerScroll() {
        this.scrollBar.directiveRef.scrollToY(this.scrollBar.directiveRef.position(true).y - 1);
        this.scrollBar.directiveRef.scrollToY(this.scrollBar.directiveRef.position(true).y + 1);
    }

    /**
     * When reach end of page, load next
     * @param event
     */
    private reachEnd(event: any) {
        console.log("event", event);
        // This check prevents this from being called prematurely on page load
        if (event.target.getAttribute('class').indexOf("ps--scrolling-y") >= 0) {
            this.getPreviousItems();
        }
    }

    /** Infinite Scrolling Handling */
    private addItems(oldItems: any[], newItems: any[], _method: any) {
        oldItems[_method].apply(oldItems, newItems);
    }

    /**
     * Add items to end of list
     * @param startIndex
     * @param endIndex
     */
    private appendItems(oldItems: any[], newItems: any[]) {
        this.addItems(oldItems, newItems, 'push');
    }

    /**
     * Add items to beginning of list
     * @param startIndex
     * @param endIndex
     */
    private prependItems(oldItems: any[], newItems: any[]) {
        this.addItems(oldItems, newItems, 'unshift');
    }

}