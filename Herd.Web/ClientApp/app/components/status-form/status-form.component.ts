import { Component, Input } from '@angular/core';
import { NgForm } from '@angular/forms';


import { MastodonService } from "../../services";
import { Visibility } from '../../models/mastodon';

@Component({
    selector: 'status-form',
    templateUrl: './status-form.component.html',
    styleUrls: ['./status-form.component.css']
})
export class StatusFormComponent {
    @Input() actionName: string;
    @Input() isReply: boolean;
    @Input() inReplyToId: number;

    maxStatusLength: number = 200;
    Visibility = Visibility;
    visibilityOptions = [
        {
            "option": "Public",
            "context": "Public timelines",
            "value": Visibility.PUBLIC,
            "icon": "globe"
        },
        {
            "option": "Private",
            "context": "Followers only",
            "value": Visibility.PRIVATE,
            "icon": "lock"
        },
        {
            "option": "Direct",
            "context": "Mentioned users only",
            "value": Visibility.DIRECT,
            "icon": "envelope"
        },
    ];

    // Default model options for a status
    model: any = {
        status: "",
        contentWarning: false,
        visibility: Visibility.PUBLIC
    };
    

    constructor(private mastodonService: MastodonService) {}

    toggleContentWarning(): void {
        this.model.contentWarning = !this.model.contentWarning
    }

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
