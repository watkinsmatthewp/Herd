import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
// Import our authentication service
import { AuthenticationService } from './authentication.service';

@Injectable()
export class AuthGuard implements CanActivate {

    constructor(private router: Router, private authService: AuthenticationService) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        if (this.authService.isAuthenticated()) {
            // Were authenticated but we didnt pick our instance yet
            if (!this.authService.checkIfConnectedToMastodon()) {
                this.router.navigateByUrl('/instance-picker');
                return false;
            }
            return true;
        }
        // not logged in so redirect to login page with the return url
        this.router.navigateByUrl('/login', { queryParams: { returnUrl: state.url } });
        return false;
    }

}