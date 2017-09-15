import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptionsArgs, RequestOptions, RequestMethod } from '@angular/http';
import 'rxjs/Rx';

import { NumberObject } from '../model/NumberObject';

@Injectable()
export class MastodonService {

    /**
        We can add "@Inject('BASE_URL') baseUrl: string" here to get the base url of the web app
        We can pass this on in the below api calls to our server BUT as of now it is working.
        Maybe this is something to worry about when going to production?

        (Below Ex) this.http.get(baseUrl + 'api/Mastodon/ConnectToMastodon') 
    */
    constructor(private http: Http) {

    }

    getRandomNumber() {
        return this.http.get('/api/RandomNumberApi/GetRandomNumber')
            .map(response => response.json() as NumberObject)
            .toPromise();
    }

    getMastodonOAuthURL() {
        return this.http.get('/api/AuthApi/GetMastodonOAuthURL')
            .map(response => response.json().url as string)
            .toPromise();
    }

    saveOAuthToken(oAuthToken: string) {        
        let headers = new Headers({ 'Content-Type': 'application/json; charset=UTF-8' });
        let options = new RequestOptions({ headers: headers });

        let body = JSON.stringify({
            oAuthToken: oAuthToken,
            testToken: 'testing',
            one: 'one',
        });

        return this.http.post('api/AuthApi/SaveOAuthToken/', body, options)
            .map(respone => respone.json().success as boolean)
            .toPromise();
    }

}

/**
We can do this

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