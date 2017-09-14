import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MastodonService } from '../shared/services/mastodon.service';

@Component({
    selector: 'login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
    constructor(private mastodonService: MastodonService) {
		
    }

    getMastodonOAuthURL() {
		// TODO: This returns a promise. How in the blazes can we get the value into the UI?
		// return this.mastodonService.getMastodonOAuthURL();
		return 'https://mastodon.xyz';
    }

    ngOnInit() {
        
    }
}
