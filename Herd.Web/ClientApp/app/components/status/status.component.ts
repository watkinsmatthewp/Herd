﻿import { Component, OnInit, Input } from '@angular/core';
import { Router } from '@angular/router';

import { StatusService, EventAlertService } from "../../services";
import { Status } from '../../models/mastodon';
import { EventAlertEnum } from "../../models/index";

@Component({
    selector: 'status',
    templateUrl: './status.component.html',
    styleUrls: ['./status.component.css']
})
export class StatusComponent implements OnInit {
    @Input() status: Status;
    showBlur: boolean = false;

    constructor(private router: Router, private eventAlertService: EventAlertService) {}

    ngOnInit() {
        if (this.status.IsSensitive === true) {
            this.showBlur = true;
        }
    }

    turnOffBlur(event: any): void {
        this.showBlur = false;
        event.stopPropagation();
    }

    notifyTimelineCommentsClicked(event: any): void {
        this.eventAlertService.addEvent(EventAlertEnum.UPDATE_SPECIFIC_STATUS, { statusID: this.status.Id });
        event.stopPropagation();
    }

    notifyTimelineStatusClicked(event: any): void {
        this.eventAlertService.addEvent(EventAlertEnum.UPDATE_SPECIFIC_STATUS, { statusID: this.status.Id });
        event.stopPropagation();
    }

    retweet() {
        
    }

    like() {
        
    }

    directMessage() {
        
    }
    
}
