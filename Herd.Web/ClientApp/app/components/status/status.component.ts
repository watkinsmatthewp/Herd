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

    constructor() { // MastodonService was here from Dana commit
    }

    ngOnInit() {
        console.info(this.status);
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
