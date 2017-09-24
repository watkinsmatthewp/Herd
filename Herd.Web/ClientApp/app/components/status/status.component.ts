import { Component, Input } from '@angular/core';

import { Status } from '../shared/models/mastodon/status';

@Component({
    selector: 'status',
    templateUrl: './status.component.html',
    styleUrls: ['./status.component.css']
})
export class StatusComponent {
    @Input() status: Status;

    constructor() {
    }

}
