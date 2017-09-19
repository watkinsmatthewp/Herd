import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { MastodonService } from '../shared/services/mastodon.service';

@Component({
    selector: 'login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.css']
})
export class LoginComponent {
    loginErrorMessage: string = '';
    username: string = '';
    instance: string = '';
    oAuthUrl: string = '';
    showSubmitOAuthTokenForm: boolean = false;

    constructor(private mastodonService: MastodonService, private router: Router) {}

    setShowOauthTokenForm(value: boolean) {
        this.showSubmitOAuthTokenForm = value;
    }

    submitOAuthRequest(form: NgForm) {
        let instanceRequest = form.value.instanceRequest;
        this.username = instanceRequest.split("@")[0];
        this.instance = instanceRequest.split("@")[1];

        this.mastodonService.OAuth_Url(this.username, this.instance).then(response => {
            this.oAuthUrl = response.url;
            window.open(this.oAuthUrl, '_blank').focus();
            this.setShowOauthTokenForm(true);
        });
    }

    submitOauthToken(form: NgForm) {
        this.mastodonService.OAuth_Return(form.value.oauth_token).then(response => {
            localStorage.setItem("session", "true");
            this.router.navigateByUrl('/home');
        }, error => {
            this.showSubmitOAuthTokenForm = false;
            this.loginErrorMessage = "Failure logging in, try again.";
        });
    }

    logout() {
        this.mastodonService.Logout().then(data => {
            localStorage.removeItem("session");
            this.router.navigateByUrl('/login');
        });
    }
}
