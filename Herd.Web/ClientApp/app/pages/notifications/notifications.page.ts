import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { NotificationsService } from "angular2-notifications";
import { EventAlertEnum, Storage } from '../../models';
import { MastodonNotification, Status, Account, PagedList } from '../../models/mastodon';
import { StatusService, EventAlertService, AccountService } from "../../services";

@Component({
    selector: 'notifications',
    templateUrl: './notifications.page.html',
})
export class NotificationsPage implements OnInit {
    @ViewChild('notificationsWrapper') notificationsWrapper: any;

    //testNotification: Notification;

    notificationList: PagedList<MastodonNotification> = new PagedList<MastodonNotification>();

    loading: boolean = false;

    constructor(private activatedRoute: ActivatedRoute, private eventAlertService: EventAlertService,
        private toastService: NotificationsService, private accountService: AccountService,
        private localStorage: Storage) { }

    ngOnInit() {
        this.notificationList.Items = [];
        this.getInitialNotifications();
    }

    getInitialNotifications() {
        this.loading = true;

        this.accountService.getHerdNotifications({ includeAncestors: true, includeDescendants: true })
            .finally(() => this.loading = false)
            .subscribe(notificationList => {
                this.notificationList = notificationList;
            }, error => {
                this.toastService.error("Error", error.error);
            });

    }

    getPreviousNotifications() {
        this.accountService.getHerdNotifications({ includeAncestors: true, includeDescendants: true, maxID: this.notificationList.PageInformation.EarlierPageMaxID })
            .subscribe(newNotificationList => {
                this.appendItems(this.notificationList.Items, newNotificationList.Items);
                this.notificationList.PageInformation = newNotificationList.PageInformation;
                this.notificationsWrapper.nativeElement.scrollTo(0, this.notificationsWrapper.nativeElement.scrollTop);
            }, error => {
                this.toastService.error("Error", error.error);
            });
    }

    /**
     * Scrolls the list area to the top
     */
    scrollToTop(tab: string) {
        this.notificationsWrapper.nativeElement.scrollTo(0, 0);
    }

    /**
     * Infinite scroll function that is called
     * when scrolling down and near end of view port
     * @param ev
     */
    onScrollDown(ev: any) {
            this.getPreviousNotifications();
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
