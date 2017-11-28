import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { Title } from "@angular/platform-browser";

import { AuthenticationService } from '../../services';
import { NotificationsService } from "angular2-notifications";
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

    constructor(private route: ActivatedRoute, private router: Router, private titleService: Title,
                private authenticationService: AuthenticationService, private toastService: NotificationsService,
                private localStorage: Storage) {
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
                this.toastService.success("Successfully", "Logged In.", { showProgressBar: false, pauseOnHover: false });

                // Reroute user depending on if they picked a mastodon instance yet.
                if (user && user.ID && !user.MastodonConnection) {
                    this.router.navigateByUrl('/instance-picker');
                } else {
                    this.localStorage.setItem('connectedToMastodon', true);
                    this.router.navigateByUrl('/home');
                }
            }, error => {
                this.toastService.error("Error", error.error);
            });
    }
}
