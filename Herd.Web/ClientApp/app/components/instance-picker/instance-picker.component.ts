import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';

import { AlertService } from '../shared/services/alert.service';
import { AuthenticationService } from '../shared/services/authentication.service';

@Component({
    selector: 'instance-picker',
    templateUrl: './instance-picker.component.html',
})
export class InstancePickerComponent {
    model: any = {};
    loading = false;
    oAuthUrl: string;

    constructor(
        private router: Router,
        private authenticationService: AuthenticationService,
        private alertService: AlertService) { }

    getOAuthToken() {
        this.loading = true;
        this.authenticationService.getOAuthUrl(this.model.instance)
        .finally(() => this.loading = false)
        .subscribe(returnedOAuthUrl => {
            this.oAuthUrl = returnedOAuthUrl;
            window.open(returnedOAuthUrl, '_blank').focus();
        }, error => {
            this.alertService.error(error);
        });
    }

    submitOAuthToken() {
        this.loading = true;
        this.authenticationService.submitOAuthToken(this.model.instance, this.model.oAuthToken)
        .finally(() => this.loading = false)
        .subscribe(data => {
            this.alertService.success("Successfully linked with " + this.model.instance, true);
            this.router.navigate(['/home']);
        }, error => {
            this.alertService.error(error);
        });
    }
}
