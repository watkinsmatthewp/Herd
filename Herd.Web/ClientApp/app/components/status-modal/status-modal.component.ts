import { Component, Input } from '@angular/core';

import { MastodonService } from "../../services";
import { Status } from '../../models/mastodon';

@Component({
    selector: 'status-modal',
    templateUrl: './status-modal.component.html',
    styleUrls: ['../status/status.component.css','./status-modal.component.css']
})
export class StatusModalComponent {
    @Input() status: Status;
    @Input() modalId: string;

    constructor() {}
}
