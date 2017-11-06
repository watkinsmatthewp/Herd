import { Injectable } from '@angular/core';
import { Http, Response, RequestOptionsArgs, Headers, RequestOptions } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';

type ResponseInterceptor = (response: any) => any;
type RequestInterceptor = (request: any) => any;
type ErrorInterceptor = (error: any) => any;

@Injectable()
export class HttpClientService {

    private defaultHeaders: Headers = new Headers({ 'Content-Type': 'application/json; charset=UTF-8' });
    private defaultFormHeaders: Headers = new Headers({ 'Content-Type': 'multipart/form-data' });
    private responseInterceptors: Array<ResponseInterceptor> = [];
    private requestInterceptors: Array<RequestInterceptor> = [];

    constructor(private http: Http) { }

    /**
     * Create a Get request
     * @param url to send request
     * @param options request options
     */
    get<T>(url: string, options?: RequestOptionsArgs): Observable<any> {
        let request = options != null ? this.http.get(url, this.generateOptions(options)) :
            this.http.get(url, new RequestOptions({ headers: this.defaultHeaders }));
        return request.map(this.mapRequest)
    }

    /**
     * Create a Post request
     * @param url to send request
     * @param data request body
     * @param options request options
     */
    post<T>(url: string, data: Object, options?: RequestOptionsArgs): Observable<any> {
        const newData = this.prepareData(data);
        let request = options != null ? this.http.post(url, newData, this.generateOptions(options)) :
                                        this.http.post(url, newData, new RequestOptions({ headers: this.defaultHeaders }));
        return request.map(this.mapRequest)
    }

    /**
     * Create a Post request in form parameters
     * @param url to send request
     * @param data request body
     * @param options request options
     */
    postForm<T>(url: string, data: Object, options?: RequestOptionsArgs): Observable<any> {
        const newData = this.prepareData(data);
        //var paramFormatData = this.paramify(newData);
        let request = options != null ? this.http.post(url, newData, this.generateOptions(options)) :
            this.http.post(url, newData, new RequestOptions({ headers: this.defaultFormHeaders }));
        return request.map(this.mapRequest)
    }

    /**
     * Turns an object into its parameterized form
     */
    paramify(data: any): any {
        // If this is not an object, defer to native stringification.
        if (typeof data === 'object') {
            return ((data == null) ? "" : data.toString());
        }

        var buffer = [];

        // Serialize each key in the object.
        for (var name in data) {
            if (!data.hasOwnProperty(name)) {
                continue;
            }

            var value = data[name];

            buffer.push(
                encodeURIComponent(name) + "=" + encodeURIComponent((value == null) ? "" : value)
            );
        }

        // Serialize the buffer and clean it up for transportation.
        var source = buffer.join("&").replace(/%20/g, "+");
        return (source); 
    }

    /**
     * Sets header for all requests.
     * @param key      A header key.
     * @param value    A value for the key.
     */
    setHeader(key: string, value: string) {
        this.defaultHeaders.set(key, value);
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
    protected generateOptions(options: RequestOptionsArgs): RequestOptionsArgs {
        if (!options.headers) {
            options.headers = new Headers();
        }
        let headerJson = this.defaultHeaders.toJSON();
        let optionsJson = options.headers.toJSON();

        let mergedHeaders: Headers = new Headers();
        options.headers.forEach((value, name) => {
            name = name || "";
            if (!mergedHeaders.has(name))
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


    /**
     * Returns data if no error, otherwise return exception
     * @param res
     */
    protected mapRequest(res: Response) {
        let json = res.text() ? res.json() : {};
        if (json.Success == false) {
            let errors: string = "";
            let systemErrors = json.SystemErrors;
            let userErrors = json.UserErrors;
            // aggregate User Errors
            if (userErrors.length > 0) {
                errors += "User Errors: "
                for (var i = 0, len = userErrors.length; i < len; i++) {
                    errors += userErrors[i].Message
                    if (i < systemErrors.length - 1) {
                        errors += ",\n";
                    }
                }
            }

            // aggregate System Errors
            if (systemErrors.length > 0) {
                errors += "System Error Codes: [";
                for (var i = 0, len = systemErrors.length; i < len; i++) {
                    errors += systemErrors[i].Id
                    if (i < systemErrors.length-1) {
                        errors += ", ";
                    }
                }
                errors += "]\n";
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