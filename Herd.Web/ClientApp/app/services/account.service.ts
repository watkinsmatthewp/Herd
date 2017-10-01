import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions, Response } from '@angular/http';
import { Observable } from "rxjs/Observable";

import { User } from '../models/User';

 
@Injectable()
export class AccountService {
    constructor(private http: Http) { }



}