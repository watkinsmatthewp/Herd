import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { CanActivate } from '@angular/router';
// Import our authentication service
import { MastodonService } from './mastodon.service';

@Injectable()
export class AuthGuard implements CanActivate {

    constructor(private mastodonService: MastodonService, private router: Router) { }

    canActivate() {
        // If the user is not logged in we'll send them back to the home page
        if (!this.mastodonService.isAuthenticated()) {
            this.router.navigateByUrl('/login');
            return false;
        }
        return true;
    }

}