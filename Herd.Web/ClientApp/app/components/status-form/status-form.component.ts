import { Component, Input } from '@angular/core';
import { NgForm } from '@angular/forms';


import { MastodonService } from "../../services";

@Component({
    selector: 'status-form',
    templateUrl: './status-form.component.html'
})
export class StatusFormComponent {
    @Input() actionName: string;
    @Input() isReply: boolean;
    @Input() inReplyToId: number;

    maxStatusLength: number = 200;
    model: any = {
        status: "",
        visibility: 0.
    };

    constructor(private mastodonService: MastodonService) {}

    submitStatus(form: NgForm) {
        if (this.isReply) { // its a reply
            this.mastodonService.makeNewPost(this.model.status, this.model.visibility, this.inReplyToId).subscribe();
        } else { // its a new status being made
            this.mastodonService.makeNewPost(this.model.status, this.model.visibility).subscribe();
        }
        // on finish reset form models
        form.resetForm();
        this.model.status = "";
        
    }
}
