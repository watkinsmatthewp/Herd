import { Component, Input } from '@angular/core';

import { StatusService } from "../../../services";
import { Status } from '../../../models/mastodon';

@Component({
    selector: 'status-form-modal',
    templateUrl: './status-form-modal.component.html',
    styleUrls: ['../../status/status.component.css']
})
export class StatusFormModalComponent {
    @Input() modalId: string;

    constructor() { }
}
