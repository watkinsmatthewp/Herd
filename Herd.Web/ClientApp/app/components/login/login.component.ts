import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MastodonService } from '../shared/services/mastodon.service';

@Component({
    selector: 'login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
    oAuthUrl = null;

    constructor(private mastodonService: MastodonService) {
    }

    login(f: NgForm) {
        console.log('form-value', f.value);
        console.log("f.value.instance", f.value.instance);

        this.mastodonService.connectToMastodon(f.value.instance).then(result => {
            this.oAuthUrl = result['oAuthUrl'];
            window.open(result['oAuthUrl'], "_blank"); // this may be blocked my the browser
        }).catch((ex) => {
            console.error('Error connecting to mastodon', ex);
        });
    }

    submitOauthToken(f: NgForm) {
        console.log('form-value', f.value);
        console.log("f.value.oauth_token", f.value.oauth_token);

        // call service to submit access token
        this.mastodonService.submitOAuthToken(f.value.oauth_token).then(result => {
            console.log("oauth result", result);
        });
    }

    ngOnInit() {
        
    }
}
