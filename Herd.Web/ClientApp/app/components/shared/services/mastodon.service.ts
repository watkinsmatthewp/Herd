import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptionsArgs, RequestOptions, RequestMethod } from '@angular/http';
import 'rxjs/Rx';

import { NumberObject } from '../model/NumberObject';

@Injectable()
export class MastodonService {

    authHeader: string;

    /**
        We can add "@Inject('BASE_URL') baseUrl: string" here to get the base url of the web app
        We can pass this on in the below api calls to our server BUT as of now it is working.
        Maybe this is something to worry about when going to production?

        (Below Ex) this.http.get(baseUrl + 'api/Mastodon/ConnectToMastodon') 
    */
    constructor(private http: Http) {}

    getRandomNumber(): Promise<NumberObject> {
        return this.http.get('/api/RandomNumberApi/GetRandomNumber')
            .map(response => response.json() as NumberObject)
            .toPromise();
    }

    // TODO
    checkWebAppAccessToken() {
        let headers = new Headers({ 'Content-Type': 'application/json; charset=UTF-8' });
        let options = new RequestOptions({ headers: headers });

        return this.http.get('/api/', options)
            .map(response => response.json())
            .toPromise();
    }

    login(username: string, instance: string) {
        let headers = new Headers({ 'Content-Type': 'application/json; charset=UTF-8'});
        let options = new RequestOptions({ headers: headers });

        let body = JSON.stringify({
            username: username,
            instance: instance
        });

        return this.http.post('api/AuthApi/LoginToApp', body, options)
            .map(respone => respone.json())
            .toPromise();
    }

    loginWithOAuthToken(instance: string, oAuthToken: string): Promise<boolean> {        
        let headers = new Headers({ 'Content-Type': 'application/json; charset=UTF-8' });
        let options = new RequestOptions({ headers: headers });

        let body = JSON.stringify({
            instance: instance,
            oAuthToken: oAuthToken,
        });

        return this.http.post('api/AuthApi/LoginWithOAuthToken/', body, options)
            .map(respone => respone.json())
            .toPromise();
    }
}

/**
Example of parsing result as a WeatherForecast array

http.get(baseUrl + 'api/SampleData/WeatherForecasts').subscribe(result => {
            this.forecasts = result.json() as WeatherForecast[];
        }, error => console.error(error));

interface WeatherForecast {
    dateFormatted: string;
    temperatureC: number;
    temperatureF: number;
    summary: string;
}
*/