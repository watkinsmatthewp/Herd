import { Component, OnInit, Input } from '@angular/core';
import { Router } from '@angular/router';

import { MastodonService, TimelineAlertService } from "../../services";
import { Status } from '../../models/mastodon';

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
        if (this.status.IsSensitive === true) {
            this.showBlur = true;
        }
    }

    turnOffBlur(event: any): void {
        this.showBlur = false;
        event.stopPropagation();
    }

    notifyTimelineCommentsClicked(event: any): void {
        this.timelineAlert.addMessage("Update reply status", this.status.Id);
        event.stopPropagation();
    }

    notifyTimelineStatusClicked(event: any): void {
        this.timelineAlert.addMessage("Update specific status", this.status.Id);
        event.stopPropagation();
    }

    retweet() {
        
    }

    like() {
        
    }

    directMessage() {
        
    }
    
}
