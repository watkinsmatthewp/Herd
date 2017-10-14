import { Component } from '@angular/core';

import { AuthenticationService } from '../../services';
import { NotificationsService } from "angular2-notifications";

@Component({
    selector: 'nav-menu',
    templateUrl: './navmenu.component.html',
    styleUrls: ['./navmenu.component.css']
})
export class NavMenuComponent {
    model: any = {};

    constructor(private authService: AuthenticationService, private toastService: NotificationsService) { }

    isAuthenticated(): boolean {
        return this.authService.isAuthenticated();
    }

    isConnectedToMastodon(): boolean {
        return this.authService.checkIfConnectedToMastodon();
    }

    search() {
        alert(this.model.searchItem);
    }

    logout() {
        this.toastService.remove();
        this.toastService.success("Successfully", "logged out.", { showProgressBar: false, pauseOnHover: false })
    }
}
