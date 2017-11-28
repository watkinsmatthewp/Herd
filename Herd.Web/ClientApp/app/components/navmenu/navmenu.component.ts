import { ActivatedRoute, Router } from '@angular/router';
import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';

import { MastodonNotification, Status, Account, PagedList } from '../../models/mastodon';
import { AuthenticationService, AccountService } from '../../services';
import { NotificationsService } from "angular2-notifications";
import { Storage } from '../../models';

@Component({
    selector: 'nav-menu',
    templateUrl: './navmenu.component.html',
    styleUrls: ['./navmenu.component.css']
})
export class NavMenuComponent implements OnInit {
    @ViewChild('notificationsWrapper') notificationsWrapper: any;

    notificationList: PagedList<MastodonNotification> = new PagedList<MastodonNotification>();

    model: any = {};
    userID: string = "";

    notificationsLoading: boolean = false;

    constructor(private authService: AuthenticationService, private toastService: NotificationsService,
        private route: ActivatedRoute, private router: Router, private localStorage: Storage,
        private accountService: AccountService) { }

    // NOTIFICATIONS LOGIC START

    ngOnInit() {
        this.notificationList.Items = [];
        this.getInitialNotifications();
    }

    getInitialNotifications() {
        this.notificationsLoading = true;

        this.accountService.getHerdNotifications({ includeAncestors: true, includeDescendants: true })
            .finally(() => this.notificationsLoading = false)
            .subscribe(notificationList => {
                this.notificationList = notificationList;
                this.toastService.success("Successfully", "Pulled Notifications.");
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

    // NOTIFICATIONS LOGIC END

    isAuthenticated(): boolean {
        return this.authService.isAuthenticated();
    }

    isConnectedToMastodon(): boolean {
        if (this.authService.checkIfConnectedToMastodon()) {
            let currentUser = JSON.parse(this.localStorage.getItem('currentUser'));
            this.userID = currentUser.MastodonConnection.MastodonUserID; 
            return true;
        }
        return false;
    }

    search(form: NgForm) {
        this.router.navigate(['/searchresults'], { queryParams: { searchString: this.model.searchItem } });
        this.model.searchItem = "";
        form.resetForm();
    }

    logout() {
        this.toastService.remove();
        this.toastService.success("Successfully", "logged out.", { showProgressBar: false, pauseOnHover: false })
    }
}
