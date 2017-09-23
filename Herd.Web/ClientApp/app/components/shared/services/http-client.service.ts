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

    private defaultHeaders: Headers = new Headers({ 'Content-Type': 'application/json; charset=UTF-8' });
    protected baseUrl = '';
    private responseInterceptors: Array<ResponseInterceptor> = [];
    private requestInterceptors: Array<RequestInterceptor> = [];

    constructor(private http: Http) { }

    /**
     * Sets header for all requests.
     * @param key      A header key.
     * @param value    A value for the key.
     */
    setHeader(key: string, value: string) {
        this.defaultHeaders.append(key, value);
    }

    /**
     * Gets header by key for all requests.
     * @param key      A header key.
     * @returns value
     */
    getHeaderByKey(key: string) {
        return this.defaultHeaders.get(key);
    }

    /**
     * Removes header for all requests.
     * @param key      A header key.
     */
    removeHeader(key: string) {
        this.defaultHeaders.delete(key);
    } 

    /**
     * Handler which generate options for all requests from headers.
     * @param options   Request options arguments
     * @returns         Request options arguments
     */
    protected generateOptions(options: RequestOptionsArgs = {}): RequestOptionsArgs {
        if (!options.headers) {
            options.headers = new Headers();
        }
        let headerJson = this.defaultHeaders.toJSON();
        let optionsJson = options.headers.toJSON();

        let mergedHeaders: Headers = new Headers();
        options.headers.forEach((value, name) => {
            name = name || "";
            if(!mergedHeaders.has(name))
                mergedHeaders.append(name, value[0]);
        });

        this.defaultHeaders.forEach((value, name) => {
            name = name || "";
            if (!mergedHeaders.has(name))
                mergedHeaders.append(name || "", value[0]);
        });

        options.headers = mergedHeaders;
        
        let mergedJson = mergedHeaders.toJSON();
        return options;
    }

    get<T>(url: string, options?: RequestOptionsArgs): Observable<T> {
        let request = options != null ? this.http.get(url, this.generateOptions(options)) : this.http.get(url);
        return request.map(this.mapRequest)
    }

    post<T>(url: string, data: Object, options?: RequestOptionsArgs): Observable<any> {
        const newData = this.prepareData(data);
        let request = options != null ? this.http.post(url, newData, this.generateOptions(options)) : this.http.post(url, newData);
        return request.map(this.mapRequest)
    }

    /**
     * Returns data if no error, otherwise return exception
     * @param res
     */
    mapRequest(res: Response) {
        let json = res.json();
        if (json.Success == false) {
            let errors: string = "";
            let arr = json.Errors;
            // aggregate the errors if multiple
            for (var i = 0, len = arr.length; i < len; i++) {
                errors += arr[i].Message;
            }
            throw Observable.throw(errors);
        } else {
            return json.Data;
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
}