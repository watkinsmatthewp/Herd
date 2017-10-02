import { Component, OnInit } from '@angular/core';


import { MastodonService } from "../../services";

@Component({
    selector: 'status-form',
    templateUrl: './status-form.component.html',
    styleUrls: ['./status-form.component.css']
})
export class StatusFormComponent implements OnInit {
    maxStatusLength: number = 200;
    model: any = {
        status: ""
    };

    constructor(private mastodonService: MastodonService) { }

    submitStatus() {
        console.log(this.model.status);
    }

    ngOnInit() {
        
    }
}
