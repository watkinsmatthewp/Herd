import { Component, OnInit, Input } from '@angular/core';

import { MastodonService } from "../../services";
import { Status } from '../../models/mastodon';

@Component({
    selector: 'status',
    templateUrl: './status.component.html',
    styleUrls: ['./status.component.css']
})
export class StatusComponent implements OnInit {
    @Input() status: Status;
    showBlur: boolean = false;

    constructor() { // MastodonService was here from Dana commit
    }

    ngOnInit() {
        //console.info(this.status);
        if (this.status.Sensitive === true) {
            this.showBlur = true;
        }
    }

    turnOffBlur(): void {
        this.showBlur = false;
    }

    reply() {
        
    }

    retweet() {
        
    }

    like() {
        
    }

    directMessage() {
        
    }
    
}
