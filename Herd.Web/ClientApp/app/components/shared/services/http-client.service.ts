import { Injectable } from '@angular/core';
import { Http, Response, RequestOptionsArgs, Headers } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';

type ResponseInterceptor = (response: any) => any;
type RequestInterceptor = (request: any) => any;
type ErrorInterceptor = (error: any) => any;
const absoluteURLPattern = /^((?:https:\/\/)|(?:http:\/\/)|(?:www))/;

@Injectable()
export class HttpClientService {

    private headers: any = {};
    protected baseUrl = '';
    private responseInterceptors: Array<ResponseInterceptor> = [];
    private requestInterceptors: Array<RequestInterceptor> = [];
    private errorInterceptors: Array<ErrorInterceptor> = [];

    constructor(private http: Http) { }

    get<T>(url: string, options?: RequestOptionsArgs): Observable<T> {
        let request = options != null ? this.http.get(url, options) : this.http.get(url);
        return request
            .map(this.mapRequest)
            .catch((error: any) => Observable.throw(error.statusText || 'Server error'));
    }

    post<T>(url: string, data: Object, options?: RequestOptionsArgs): Observable<any> {
        const newData = this.prepareData(data);
        let request = options != null ? this.http.post(url, newData, options) : this.http.post(url, newData);
        return request
            .map(this.mapRequest)
            .catch((error: any) => Observable.throw(error.statusText || 'Server error'));
    }

    /**
     * Returns data if no error otherwise return exception
     * @param res
     */
    mapRequest(res: Response) {
        if (res) {
            let json = res.json();
            if (json.Success == false) {
                return Observable.throw(new Error(json.errors));
            } else {
                return json.Data;
            }
        }
    }

    /**
   * Prepare data for request with interceptors
   * @param data     any
   * @returns        string
   */
    protected prepareData(data: any): string {
        return this.requestInterceptors.reduce((acc, interceptor) => interceptor(acc), data);
    }

    /**
     * Handler which transform response to JavaScript format if response exists.
     * @param resp     Http response
     * @returns        any
     */
    protected responseHandler(resp: Response): any {
        return this.responseInterceptors.reduce((acc: any, interceptor: any) => interceptor(acc), resp);
    }

    protected errorHandler(error: Response): Observable<any> {
        return Observable.throw(
            this.errorInterceptors.reduce((acc: any, interceptor: any) => interceptor(acc), error)
        );
    }
}