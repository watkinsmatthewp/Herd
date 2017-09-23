import { Injectable } from '@angular/core';
import { Http, Headers, Response, RequestOptionsArgs, RequestOptions, RequestMethod } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map'

import { HttpClientService } from '../services/http-client.service';
import { StorageService } from '../models/Storage';
import { User } from "../models/User";

@Injectable()
export class AuthenticationService {
    constructor(private http: Http, private httpClient: HttpClientService, private localStorage: StorageService) { }

    checkIfConnectedToMastodon(): boolean {
        if (localStorage.getItem('connectedToMastodon') === "true") {
            return true;
        }
        return false;
    }

    /**
     * Checks if a person is authenticated by checking their currentUser item in local storage
     */
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
        let body = {
            'email': email,
            'password': password
        }
        return this.httpClient.post('api/account/login', body)       
    }

    /**
     * GET /api/account/logout
     */
    logout() {
        // remove user from local storage to log user out & log user out of backend
        this.localStorage.clear();
        return this.http.get('api/account/Logout').toPromise();
    }

    /**
     * Get a oAuth token for the instance
     * GET /api/oauth/url?instance={instance}
     *
     * @returns: { "url": "https://oAuthUrl.com" }
     */
    getOAuthUrl(registrationId: number) {
        let headers = new Headers({ 'Content-Type': 'application/json; charset=UTF-8' });
        let options = new RequestOptions({ headers: headers });
        let queryString = '?registrationID=' + registrationId;

        return this.http.get('api/oauth/url' + queryString, options)
            .map(response => response.json())
            .catch((error: any) => Observable.throw(error.statusText || 'Server error'));
    }

    getRegistrationId(instance: string) {
        let headers = new Headers({ 'Content-Type': 'application/json; charset=UTF-8' });
        let options = new RequestOptions({ headers: headers });
        let queryString = '?instance=' + instance;

        return this.http.get('api/oauth/registration_id' + queryString, options)
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
    submitOAuthToken(token: string, registrationID: number) {
        let headers = new Headers({ 'Content-Type': 'application/json; charset=UTF-8' });
        let options = new RequestOptions({ headers: headers });
        let body = {
            'token': token,
            'app_registration_id': registrationID,
            
        }
        return this.http.post('api/oauth/set_tokens', body, options)
            .catch((error: any) => Observable.throw(error.statusText || 'Server error'));
    }
}