import { Component, OnInit, Input } from '@angular/core';

import { MastodonService, TimelineAlertService } from "../../services";
import { Status } from '../../models/mastodon';
import { Router } from '@angular/router';

@Component({
    selector: 'status',
    templateUrl: './status.component.html',
    styleUrls: ['./status.component.css']
})
export class StatusComponent implements OnInit {
    @Input() status: Status;
    showBlur: boolean = false;

    constructor(private router: Router, private timelineAlert: TimelineAlertService) {}

    ngOnInit() {
        if (this.status.Sensitive === true) {
            this.showBlur = true;
        }
    }

    turnOffBlur(): void {
        this.showBlur = false;
    }

    notifyTimelineCommentsClicked(): void {
        this.timelineAlert.addMessage("Update specific status", this.status.Id);
    }

    retweet() {
        
    }

    like() {
        
    }

    directMessage() {
        
    }
    
}
