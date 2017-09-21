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

    /**
     * GET /api/auth/Login
     * @param email
     * @param password
     */
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

    /**
     * GET /api/auth/Logout
     */
    logout() {
        // remove user from local storage to log user out & log user out of backend
        localStorage.removeItem('currentUser');
        return this.http.get('/api/auth/Logout').toPromise()
    }

    /**
     * Get a oAuth token for the instance
     * GET /api/oauth/url?instance={instance}
     *
     * @returns: { "url": "https://oAuthUrl.com" }
     */
    getOAuthUrl(instance: string) {
        let headers = new Headers({ 'Content-Type': 'application/json; charset=UTF-8' });
        let options = new RequestOptions({ headers: headers });
        let queryString = '?instance=' + instance;

        return this.http.get('api/oauth/url' + queryString, options)
            .map(response => response.json())
            .catch((error: any) => Observable.throw(error.statusText || 'Server error'));
    }

    /**
     * Submit the OAuth Token
     * POST /api/oauth/set_tokens
     *
     * @param instance
     * @param token
     */
    submitOAuthToken(instance: string, token: string) {
        let headers = new Headers({ 'Content-Type': 'application/json; charset=UTF-8' });
        let options = new RequestOptions({ headers: headers });
        let body = {
            'instance': instance,
            'token': token
        }
        return this.http.post('api/oauth/set_tokens', body, options)
            .map(response => response.json())
            .catch((error: any) => Observable.throw(error.statusText || 'Server error'));
    }
}