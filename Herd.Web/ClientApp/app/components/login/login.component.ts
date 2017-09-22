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
        this.authenticationService.logout();

        // if coming from register default the email
        this.model.email = this.route.snapshot.queryParams['email'] || ''; 

        // get return url from route parameters or default to '/'
        this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
    }

    login() {
        this.loading = true;
        this.authenticationService.login(this.model.email, this.model.password)
        .finally(() => this.loading = false)
        .subscribe(user => {
            if (user && user.email && !user.instance) {
                this.localStorage.setItem('currentUser', JSON.stringify(user));
                this.alertService.success("Successfully Logged", true);
                this.router.navigateByUrl('/instance-picker');
            }
        }, error => {
            this.alertService.error(error);
        });
    }
}
