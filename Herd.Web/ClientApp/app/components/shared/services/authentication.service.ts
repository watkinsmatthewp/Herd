import { Injectable } from '@angular/core';
import { Http, Headers, Response, RequestOptionsArgs, RequestOptions, RequestMethod } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map'

import { StorageService } from '../models/Storage';

@Injectable()
export class AuthenticationService {
    constructor(private http: Http, private localStorage: StorageService) { }

    isAuthenticated() {
        if (this.localStorage.getItem('currentUser')) {
            return true; //logged in
        }
        return false; // not logged in
    }

    login(username: string, password: string) {
        return this.http.post('/api/authenticate', JSON.stringify({ username: username, password: password }))
            .map((response: Response) => {
                // login successful if there's a jwt token in the response
                let user = response.json();
                if (user && user.token) {
                    // store user details and jwt token in local storage to keep user logged in between page refreshes
                    localStorage.setItem('currentUser', JSON.stringify(user));
                }
                return user;
            });
    }

    logout() {
        // remove user from local storage to log user out & log user out of backend
        localStorage.removeItem('currentUser');
        return this.http.get('/api/auth/Logout').toPromise()
    }

    // Get the OAuth Token
    getOAuthUrl(username: string, instance: string) {
        let headers = new Headers({ 'Content-Type': 'application/json; charset=UTF-8' });
        let options = new RequestOptions({ headers: headers });
        let body = JSON.stringify({
            username: username,
            instance: instance
        });

        return this.http.post('api/auth/OAuth_Url', body, options)
            .map(response => response.json())
            .toPromise();
    }

    // Submit the OAuth Token
    submitOAuthUrl(OAuthToken: string) {
        let headers = new Headers({ 'Content-Type': 'application/json; charset=UTF-8' });
        let options = new RequestOptions({ headers: headers });
        return this.http.get('api/auth/OAuth_Return/' + OAuthToken, options).toPromise();
    }
}