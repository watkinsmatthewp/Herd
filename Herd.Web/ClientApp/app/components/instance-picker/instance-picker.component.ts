import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';

import { AlertService } from '../shared/services/alert.service';
import { AuthenticationService } from '../shared/services/authentication.service';

@Component({
    selector: 'instance-picker',
    templateUrl: './instance-picker.component.html',
    styleUrls: []
})
export class InstancePickerComponent {
    model: any = {};
    loading = false;
    oAuthUrl: string;
    instance: string;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private authenticationService: AuthenticationService,
        private alertService: AlertService) { }

    getOAuthToken() {
        this.loading = true;

        this.oAuthUrl = "https://google.com";
        /**
        this.authenticationService.login(this.model.username, this.model.password)
            .subscribe(
            data => {
                this.router.navigate([this.returnUrl]);
            },
            error => {
                this.alertService.error(error);
                this.loading = false;
            });
        */
        this.loading = false;
    }

    submitOAuthToken() {
        this.loading = true;
        this.alertService.success("Successfully linked with " + this.model.instance, true);
        this.router.navigate(['/login']); // TODO: this should be home
    }
}
