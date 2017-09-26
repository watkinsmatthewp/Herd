import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';

import { AlertService } from '../shared/services/alert.service';
import { AuthenticationService } from '../shared/services/authentication.service';
import { StorageService } from '../shared/models/Storage';

@Component({
    selector: 'login',
    templateUrl: './login.component.html',
    styleUrls: []
})
export class LoginComponent implements OnInit {
    model: any = {};
    loading = false;
    returnUrl: string;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private authenticationService: AuthenticationService,
        private alertService: AlertService,
        private localStorage: StorageService) { }

    ngOnInit() {
        // reset login status
        this.authenticationService.logout().subscribe();;
        // if coming from register default the email
        this.model.email = this.route.snapshot.queryParams['email'] || ''; 
    }

    login() {
        this.loading = true;
        this.authenticationService.login(this.model.email, this.model.password)
            .finally(() => this.loading = false)
            .subscribe(response => {
                let user = response.User;
                this.localStorage.setItem('currentUser', JSON.stringify(user));
                this.alertService.success("Successfully Logged In", true);

                // Reroute user depending on if they picked a mastodon instance yet.
                if (user && user.ID && !user.MastodonConnection) {
                    this.router.navigateByUrl('/instance-picker');
                } else {
                    this.localStorage.setItem('connectedToMastodon', true);
                    this.router.navigateByUrl('/home');
                }
            }, error => {
                this.alertService.error(error.error);
            });
    }
}
