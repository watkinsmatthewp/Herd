import { ActivatedRoute, Router } from '@angular/router';
import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';

import { AuthenticationService } from '../../services';
import { NotificationsService } from "angular2-notifications";

@Component({
    selector: 'nav-menu',
    templateUrl: './navmenu.component.html',
    styleUrls: ['./navmenu.component.css']
})
export class NavMenuComponent {
    model: any = {};

    constructor(private authService: AuthenticationService, private toastService: NotificationsService,
        private route: ActivatedRoute, private router: Router) { }

    isAuthenticated(): boolean {
        return this.authService.isAuthenticated();
    }

    isConnectedToMastodon(): boolean {
        return this.authService.checkIfConnectedToMastodon();
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
