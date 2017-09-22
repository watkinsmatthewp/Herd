import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';

import { AlertService } from '../shared/services/alert.service';
import { UserService } from '../shared/services/user.service';
import { AuthenticationService } from '../shared/services/authentication.service';
import { User } from '../shared/models/User';

@Component({
    selector: 'register',
    templateUrl: './register.component.html'
})
export class RegisterComponent {
    model: any = {};
    loading = false;

    constructor(private router: Router, private authenticationService: AuthenticationService, private alertService: AlertService) { }

    register() {
        this.loading = true;
        let user: User = new User(this.model.email, this.model.password, this.model.firstName, this.model.lastName);

        this.authenticationService.register(user)
            .finally(() => this.loading = false)
            .subscribe(user => {
                this.alertService.success('Registration successful', true);
                this.router.navigateByUrl('/login', { queryParams: { email: this.model.email } });
            }, error => {
                this.alertService.error(error);
            });
    }
}
