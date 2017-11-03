import { Component, Input, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Observable } from "rxjs/Observable";

import { StatusService } from "../../services";
import { NotificationsService } from "angular2-notifications";
import { Visibility } from '../../models/mastodon';

@Component({
    selector: 'status-form',
    templateUrl: './status-form.component.html',
    styleUrls: ['./status-form.component.css']
})
export class StatusFormComponent {
    @Input() actionName: string;
    @Input() isReply: boolean;
    @Input() inReplyToId: string
    ImagePreview: any;
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
        spoilerText: "",
    };
    

    constructor(private statusService: StatusService, private toastService: NotificationsService) {}

    onFileChange(event: any) {
        let fileList: FileList = event.target.files;
        if (fileList.length > 0) {
            let file = fileList[0];
            this.model.file = file;
            this.model.filename = file.name;
            let formData: FormData = new FormData();
            formData.append('uploadFile', file, file.name);
            this.model.formData = formData;
            console.log("File", file);

            let reader = new FileReader();
            reader.onload = (e: any) => {
                this.ImagePreview = e.target.result;
            }
            reader.readAsDataURL(event.target.files[0]);
        }
    }

    clearFile() {
        this.model.filename = null;
        this.model.file = null;
        this.model.formData = null;
        console.log("File", this.model.file);
    }

    toggleContentWarning(): void {
        this.model.contentWarning = !this.model.contentWarning
    }

    submitStatus(form: NgForm) {
        this.statusService.makeNewStatus(this.model.status, this.model.visibility, this.inReplyToId, this.model.contentWarning, this.model.spoilerText, this.model.formData)
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
