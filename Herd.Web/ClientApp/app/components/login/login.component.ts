import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { MastodonService } from '../shared/services/mastodon.service';

@Component({
    selector: 'login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
    loginErrorMessage: string = '';
    username: string = '';
    instance: string = '';
    oAuthUrl: string = '';
    showSubmitOAuthTokenForm: boolean = false;

    constructor(private mastodonService: MastodonService, private router: Router) {
		
    }

    setShowOauthTokenForm(value: boolean) {
        this.showSubmitOAuthTokenForm = value;
    }

    submitInstaceRequest(form: NgForm) {
        let instanceRequest = form.value.instanceRequest;
        this.username = instanceRequest.split("@")[0];
        this.instance = instanceRequest.split("@")[1];

        this.mastodonService.login(this.username, this.instance).then(response => {
            console.log("response", response);
            if (response.loginSuccessful === true) {
                this.router.navigateByUrl('/home');
            } else {
                this.oAuthUrl = response.url;
            }
        });
    }

    submitOauthToken(form: NgForm) {
        this.mastodonService.loginWithOAuthToken(this.instance, form.value.oauth_token).then(data => {
            this.router.navigateByUrl('/home');
        }, error => {
            this.showSubmitOAuthTokenForm = false;
            this.loginErrorMessage = "Failure logging in, try again.";
        });
    }

    ngOnInit() {}
}
