import { Component, OnInit, Input } from '@angular/core';
import { NgForm } from '@angular/forms';


import { MastodonService } from "../../services";

@Component({
    selector: 'status-form',
    templateUrl: './status-form.component.html',
    styleUrls: ['./status-form.component.css']
})
export class StatusFormComponent implements OnInit {
    @Input() actionName: string;
    @Input() isReply: boolean;
    @Input() inReplyToId: number;

    maxStatusLength: number = 200;
    model: any = {
        status: ""
    };

    constructor(private mastodonService: MastodonService) {}

    submitStatus(form: NgForm) {
        if (this.isReply === false) {
            this.mastodonService.makeNewPost(this.model.status).subscribe();
            // on finish reset form models
            form.resetForm();
            this.model.status = "";
        } else {
            this.mastodonService.makeNewReply(this.model.status, this.inReplyToId).subscribe();
            // on finish reset form models
            form.resetForm();
            this.model.status = "";
        }
        
    }

    ngOnInit() {
        
    }
}
