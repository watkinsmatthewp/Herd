import { Component, Input } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Observable } from "rxjs/Observable";

import { StatusService, EventAlertService } from "../../services";
import { NotificationsService } from "angular2-notifications";
import { Visibility } from '../../models/mastodon';
import { EventAlertEnum } from "../../models/index";

@Component({
    selector: 'status-form',
    templateUrl: './status-form.component.html',
    styleUrls: ['./status-form.component.css']
})
export class StatusFormComponent {
    @Input() actionName: string;
    @Input() isReply: boolean;
    @Input() inReplyToId: string;

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
        visibility: Visibility.PUBLIC,
        spoilerText: ""
    };
    

    constructor(private statusService: StatusService, private eventAlertService: EventAlertService, private toastService: NotificationsService) { }

    ngOnInit() {
        this.eventAlertService.getMessage().subscribe(event => {
            switch (event.eventType) {
                case EventAlertEnum.UPDATE_STATUS_FORM_TEXT: {
                    let statusText: string = event.statusText;
                    this.updateStatusText(statusText);
                }
            }
        });
    }

    toggleContentWarning(): void {
        this.model.contentWarning = !this.model.contentWarning
    }

    updateStatusText(text: string): void {
        this.model.status = text + " ";
    }

    submitStatus(form: NgForm) {
        this.statusService.makeNewStatus(this.model.status, this.model.visibility, this.inReplyToId, this.model.contentWarning, this.model.spoilerText)
            .finally(() => {
                this.resetFormDefaults(form);
            })
            .subscribe(response => {
                this.toastService.success("Successfully", "posted a status.");
            }, error => {
                this.toastService.error(error.error);
            });
    }

    resetFormDefaults(form: NgForm): void {
        form.resetForm();
        this.model.status = "";
        this.model.contentWarning = false;
        this.model.visibility = Visibility.PUBLIC;
        form.controls.visibility.setValue(0); // have to manually set the select value for some reason
        this.model.spoilerText = "";
    }
}
