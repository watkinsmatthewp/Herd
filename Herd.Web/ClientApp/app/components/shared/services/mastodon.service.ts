import { Injectable } from '@angular/core';
import { Http, RequestOptionsArgs, RequestOptions, RequestMethod } from '@angular/http';
import 'rxjs/Rx';

import { NumberObject } from '../model/NumberObject';
import { MastodonAuthenticationObject } from '../model/mastodonAuthentication/MastodonAuthenticationObject'

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
        return this.http.get('api/AuthApi/GetRandomNumber')
            .map(response => response.json() as NumberObject)
            .toPromise();
    }

    connectToMastodon(instanceName: string) {
        console.log("connectToMastodon", instanceName);

        return this.http.get('api/AuthApi/ConnectToMastodon/' + instanceName)
            .map(response => response.json())
            .toPromise();
    }

    submitOAuthToken(oAuthToken: string) {
        console.log("oauth token", oAuthToken);

        return this.http.get('api/AuthApi/SubmitOAuthToken/' + oAuthToken)
            .map(respone => respone.json() as MastodonAuthenticationObject)
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