import { Injectable } from '@angular/core';
import { Router, NavigationStart } from '@angular/router';
import { Observable } from 'rxjs';
import { Subject } from 'rxjs/Subject';

@Injectable()
export class TimelineAlertService {
    private subject = new Subject<any>();

    constructor() {}

    addMessage(message: string, statusId: string) {
        this.subject.next({ message: message, statusId: statusId });
    }

    getMessage(): Observable<any> {
        return this.subject.asObservable();
    }
}