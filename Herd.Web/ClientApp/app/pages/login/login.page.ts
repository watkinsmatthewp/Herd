import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';

import { AlertService, AuthenticationService } from '../../services';
import { Storage } from '../../models';

@Component({
    selector: 'login',
    templateUrl: './login.page.html',
    styleUrls: []
})
export class LoginPage implements OnInit {
    model: any = {};
    loading = false;
    returnUrl: string;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private authenticationService: AuthenticationService,
        private alertService: AlertService,
        private localStorage: Storage) { }

    ngOnInit() {
        // reset login status
        this.authenticationService.logout().subscribe();
        // if coming from register default the email
        this.model.email = this.route.snapshot.queryParams['email'] || ''; 
    }

    login(form: NgForm) {
        this.loading = true;
        this.authenticationService.login(this.model.email, this.model.password)
            .finally(() => {
                this.loading = false
                form.resetForm();
                this.model.email = "";
                this.model.password = "";
            })
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
