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
    newNotification: boolean = false;
    newNotificationCount: number = 0;

    constructor(private authService: AuthenticationService, private toastService: NotificationsService,
        private route: ActivatedRoute, private router: Router, private localStorage: Storage,
        private accountService: AccountService) { }

    ngOnInit() {
        //this.setNewNotifcations(true);
        //this.newNotificationCount = 5;
    }

    // NOTIFICATIONS LOGIC START

    setNewNotification(toggle: boolean) {
        this.newNotification = toggle;
    }

    setNewNotificationCount(count: number) {
        this.newNotificationCount = count;
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
