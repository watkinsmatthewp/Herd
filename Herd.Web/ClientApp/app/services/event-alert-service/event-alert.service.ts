import { Injectable } from '@angular/core';
import { Router, NavigationStart } from '@angular/router';
import { Observable } from 'rxjs';
import { Subject } from 'rxjs/Subject'; 

import { EventAlertEnum } from '../../models'

@Injectable()
export class EventAlertService {
    private subject = new Subject<any>();

    constructor() { }

    addEvent(event: EventAlertEnum, details: any = {}) {
        let eventDetails = Object.assign({ eventType: event }, details);
        this.subject.next(eventDetails);
    }

    getMessage(): Observable<any> {
        return this.subject.asObservable();
    }
}