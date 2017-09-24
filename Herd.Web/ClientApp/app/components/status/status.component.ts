import { Component, Input } from '@angular/core';

import { Status } from '../shared/models/Status';

@Component({
    selector: 'status',
    templateUrl: './status.component.html',
    styleUrls: ['./status.component.css']
})
export class PostComponent {
    @Input() status: Status;

    constructor() {
    }

}
