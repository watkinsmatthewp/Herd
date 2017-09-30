import { Component } from '@angular/core';

@Component({
    selector: 'status-form',
    templateUrl: './status-form.component.html',
    styleUrls: ['./status-form.component.css']
})
export class StatusFormComponent {
    model: any = {
        status: ""
    };

    constructor() {
    }

    submitStatus() {

    }
}
