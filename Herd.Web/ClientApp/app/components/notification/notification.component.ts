import { Component, OnInit, Input } from '@angular/core';
import { Router } from '@angular/router';

import { Notification } from '../../models/mastodon';
import { StatusService, EventAlertService } from "../../services";
import { Status } from '../../models/mastodon';
import { EventAlertEnum } from "../../models/index";

@Component({
    selector: 'notification',
    templateUrl: './notification.component.html',
    styleUrls: ['./notification.component.css']
})
export class NotificationComponent implements OnInit {

    @Input() notification: Notification;

    constructor(private router: Router, private statusService: StatusService, private eventAlertService: EventAlertService) { }

    ngOnInit() {

    }

}