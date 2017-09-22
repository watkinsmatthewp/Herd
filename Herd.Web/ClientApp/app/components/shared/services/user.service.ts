import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { Observable } from "rxjs/Observable";

import { User } from '../models/User';

 
@Injectable()
export class UserService {
    constructor(private http: Http) { }


}