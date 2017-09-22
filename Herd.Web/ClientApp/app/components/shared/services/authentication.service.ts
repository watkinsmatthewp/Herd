import { Injectable } from '@angular/core';
import { Http, Headers, Response, RequestOptionsArgs, RequestOptions, RequestMethod } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map'

import { StorageService } from '../models/Storage';
import { User } from "../models/User";

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
     * POST /api/account/register
     */
    register(user: User) {
        let headers = new Headers({ 'Content-Type': 'application/json; charset=UTF-8' });
        let options = new RequestOptions({ headers: headers });

        return this.http.post('api/account/register', JSON.stringify(user), options)
            .map(response => response.json())
            .catch((error: any) => Observable.throw(error.statusText || 'Server error'));
    }

    /**
     * POST /api/account/login
     *
     * @returns : {AppUser Obj}
     */
    login(email: string, password: string) {
        let headers = new Headers({ 'Content-Type': 'application/json; charset=UTF-8' });
        let options = new RequestOptions({ headers: headers });
        let body = {
            'email': email,
            'password': password
        }
        return this.http.post('api/account/login', body, options)
            .map(response => response.json())
            .catch((error: any) => Observable.throw(error.statusText || 'Server error'));
    }

    /**
     * GET /api/account/logout
     */
    logout() {
        // remove user from local storage to log user out & log user out of backend
        localStorage.removeItem('currentUser');
        return this.http.get('api/account/Logout').toPromise()
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