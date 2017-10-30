import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';

import { AuthenticationService } from '../../services';
import { NotificationsService } from "angular2-notifications";
import { User } from '../../models';

@Component({
    selector: 'register',
    templateUrl: './register.page.html'
})
export class RegisterPage {
    model: any = {};
    loading = false;

    constructor(private router: Router, private authenticationService: AuthenticationService, private toastService: NotificationsService) { }

    register() {
        this.loading = true;
        let user: User = {
            email: this.model.email, password: this.model.password,
            firstName: this.model.firstName, lastName: this.model.lastName
        } as User;

        this.authenticationService.register(user)
            .finally(() => this.loading = false)
            .subscribe(response => {
                this.toastService.success('Successful',  'registration');
                this.router.navigate(['/login'], { queryParams: { email: this.model.email } });
            }, error => {
                this.toastService.error("Error", error.error);
            });
    }
}
