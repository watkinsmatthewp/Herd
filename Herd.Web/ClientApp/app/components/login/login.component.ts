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
    showSubmitOAuthTokenForm: boolean = false;
    oAuthUrl: string = '';

    constructor(private mastodonService: MastodonService, private router: Router) {
		
    }

    setShowOauthTokenForm(value: boolean) {
        this.showSubmitOAuthTokenForm = value;
    }

    getMastodonOAuthURL() {        
        this.mastodonService.getMastodonOAuthURL().then(url => {
            this.oAuthUrl = url;
        });
    }

    submitOauthToken(form: NgForm) {
        this.mastodonService.saveOAuthToken(form.value.oauth_token).then(data => {
            this.router.navigateByUrl('/home');
        }, this.handleError);
    }

    private handleError(error: any): Promise<any> {
        this.router.navigateByUrl('/login');
        return Promise.reject(error.message || error);
    }

    ngOnInit() {
        this.getMastodonOAuthURL();
    }
}
