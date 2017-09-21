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

    login(email: string, password: string) {
        let user = {
            id: 0,
            email: email,
            password: password,
        };
        this.localStorage.setItem('currentUser', JSON.stringify(user));
        return user;
        /**
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
        */
    }

    logout() {
        // remove user from local storage to log user out & log user out of backend
        localStorage.removeItem('currentUser');
        return this.http.get('/api/auth/Logout').toPromise()
    }

    /**
     * Get the OAuth Token
     * GET /oauth/oauth_url?username={username}&instance={instance}
     * Return: { "url": "https://mastodon.xyz/oauth/whatever?client_id={client_id}...&redirect_url={redirect_url}" }
     */
    getOAuthUrl(instance: string) {
        let headers = new Headers({ 'Content-Type': 'application/json; charset=UTF-8' });
        let options = new RequestOptions({ headers: headers });
        let queryString = '?instance=' + instance;

        return this.http.get('oauth/oauth_url' + queryString, options)
            .map(response => response.json().url as string)
    }

    // Submit the OAuth Token
    submitOAuthUrl(OAuthToken: string) {
        let headers = new Headers({ 'Content-Type': 'application/json; charset=UTF-8' });
        let options = new RequestOptions({ headers: headers });
        return this.http.get('api/auth/OAuth_Return/' + OAuthToken, options).toPromise();
    }
}