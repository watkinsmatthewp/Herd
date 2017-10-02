import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';


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

    submitStatus(form: NgForm) {
        console.log(this.model.status);

        // on finish reset form models
        form.resetForm();
        this.model.status = "";
    }

    ngOnInit() {
        
    }
}
