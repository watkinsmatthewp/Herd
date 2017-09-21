import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';

import { AlertService } from '../shared/services/alert.service';
import { UserService } from '../shared/services/user.service';

@Component({
    selector: 'register',
    templateUrl: './register.component.html'
})
export class RegisterComponent {
    model: any = {};
    loading = false;

    constructor(private router: Router, private userService: UserService, private alertService: AlertService) { }

    register() {
        this.loading = true;

        this.userService.create(this.model).subscribe(data => {
            // set success message and pass true paramater to persist the message after redirecting
            this.alertService.success('Registration successful', true);
            this.router.navigate(['/login']);
        }, error => {
            this.alertService.error(error);
            this.loading = false;
        });
    }
}
