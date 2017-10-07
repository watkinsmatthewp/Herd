import { Component, OnInit, Input } from '@angular/core';

import { MastodonService } from "../../services";
import { Status } from '../../models/mastodon';

@Component({
    selector: 'status-modal',
    templateUrl: './status-modal.component.html',
    styleUrls: ['./status-modal.component.css']
})
export class StatusModalComponent implements OnInit {
    @Input() status: Status;
    @Input() modalId: string;

    constructor() {}

    ngOnInit() {
        console.log(this.status.Id);
    }

}
