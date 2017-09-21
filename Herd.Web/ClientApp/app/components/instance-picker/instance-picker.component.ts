import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';

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
        private route: ActivatedRoute,
        private router: Router,
        private authenticationService: AuthenticationService,
        private alertService: AlertService) { }

    getOAuthToken() {
        this.loading = true;
        this.authenticationService.getOAuthUrl(this.model.instance).subscribe(returnedOAuthUrl => {
            this.oAuthUrl = returnedOAuthUrl;
            window.open(returnedOAuthUrl, '_blank').focus();
        }, error => {
            this.alertService.error(error);
        });
        this.loading = false;
    }

    submitOAuthToken() {
        this.loading = true;
        this.authenticationService.submitOAuthToken(this.model.instance, this.model.oAuthToken).subscribe(data => {
            this.alertService.success("Successfully linked with " + this.model.instance, true);
            this.router.navigate(['/home']);
        }, error => {
            this.alertService.error(error);
        });
    }
}
