import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';

import { AlertService } from '../shared/services/alert.service';
import { AuthenticationService } from '../shared/services/authentication.service';

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
        private alertService: AlertService) { }

    ngOnInit() {
        // reset login status
        this.authenticationService.logout();

        // get return url from route parameters or default to '/'
        this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
    }

    login() {
        this.loading = true;
        let user = this.authenticationService.login(this.model.email, this.model.password);
        this.loading = false;
        this.alertService.success("Successfully Logged in as " + user.email, true);
        this.router.navigateByUrl('/home');

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
    }
}
