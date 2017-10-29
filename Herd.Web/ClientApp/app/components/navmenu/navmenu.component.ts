import { ActivatedRoute, Router } from '@angular/router';
import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';

import { AuthenticationService } from '../../services';
import { NotificationsService } from "angular2-notifications";
import { Storage } from '../../models';

@Component({
    selector: 'nav-menu',
    templateUrl: './navmenu.component.html',
    styleUrls: ['./navmenu.component.css']
})
export class NavMenuComponent {
    model: any = {};
    userID: string = "";

    constructor(private authService: AuthenticationService, private toastService: NotificationsService,
        private route: ActivatedRoute, private router: Router, private localStorage: Storage) { }

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
