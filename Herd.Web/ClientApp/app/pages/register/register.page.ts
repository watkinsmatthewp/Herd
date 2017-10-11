import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';

import { AlertService, AuthenticationService } from '../../services';
import { User } from '../../models';
import { NotificationsService } from "angular2-notifications";

@Component({
    selector: 'register',
    templateUrl: './register.page.html'
})
export class RegisterPage {
    model: any = {};
    loading = false;

    constructor(private router: Router, private authenticationService: AuthenticationService, private alertService: NotificationsService) { }

    register() {
        this.loading = true;
        let user: User = new User(this.model.email, this.model.password, this.model.firstName, this.model.lastName);

        this.authenticationService.register(user)
            .finally(() => this.loading = false)
            .subscribe(response => {
                this.alertService.success('Successful',  'registration');
                this.router.navigate(['/login'], { queryParams: { email: this.model.email } });
            }, error => {
                this.alertService.error(error.error);
            });
    }
}
