import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router, NavigationEnd } from '@angular/router';
import { Title } from "@angular/platform-browser";

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

    constructor(private router: Router, private titleService: Title, private authenticationService: AuthenticationService, private toastService: NotificationsService) {
        router.events.subscribe(event => {
            if (event instanceof NavigationEnd) {
                var title = this.getTitle(router.routerState, router.routerState.root).join('-');
                titleService.setTitle(title);
            }
        });
    }

    private getTitle(state: any, parent: any): any {
        var data = [];
        if (parent && parent.snapshot.data && parent.snapshot.data.title) {
            data.push(parent.snapshot.data.title);
        }

        if (state && parent) {
            data.push(... this.getTitle(state, state.firstChild(parent)));
        }
        return data;
    }

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
