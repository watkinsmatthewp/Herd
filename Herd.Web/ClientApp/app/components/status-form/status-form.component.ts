import { Component, Input, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Observable } from "rxjs/Observable";

import { Image } from 'angular-modal-gallery';
import { NotificationsService } from "angular2-notifications";

import { StatusService, EventAlertService } from "../../services";
import { Visibility } from '../../models/mastodon';
import { EventAlertEnum } from "../../models/index";


@Component({
    selector: 'status-form',
    templateUrl: './status-form.component.html',
    styleUrls: ['./status-form.component.css']
})
export class StatusFormComponent {
    @ViewChild('fileInput') fileInput: any;
    @Input() actionName: string;
    @Input() isReply: boolean;
    @Input() inReplyToId: string

    loading = false;
    ImagePreview: any;
    imagesArray: Array<Image> = [];
    maxStatusLength: number = 200;
    Visibility = Visibility;
    visibilityOptions = [
        {
            "option": "Public",
            "context": "Public timelines",
            "value": Visibility.PUBLIC,
            "icon": "globe"
        },
        // Currently not using Mastodon as a federation so no need for private for now.
        //{
        //    "option": "Private",
        //    "context": "Followers only",
        //    "value": Visibility.PRIVATE,
        //    "icon": "lock"
        //},
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

    onFileChange(event: any) {
        let fileList: FileList = event.target.files;
        if (fileList.length > 0) {
            let file = fileList[0];
            this.model.file = file;
            this.model.filename = file.name;

            let reader = new FileReader();
            reader.onload = (e: any) => {
                this.ImagePreview = e.target.result;
                this.imagesArray.push(new Image(this.ImagePreview));
            }
            reader.readAsDataURL(event.target.files[0]);
        }
    }

    clearFile() {
        this.fileInput.nativeElement.value = ""; // this completely resets file input
        this.model.filename = null;
        this.model.file = null;
    }

    toggleContentWarning(): void {
        this.model.contentWarning = !this.model.contentWarning
    }

    updateStatusText(text: string): void {
        this.model.status = text + " ";
    }

    submitStatus(form: NgForm) {

        this.loading = true;
        this.statusService.makeNewStatus(this.model.status, this.model.visibility, this.inReplyToId, this.model.contentWarning, this.model.spoilerText, this.model.file)
            .finally(() => {
                this.loading = false;
                this.resetFormDefaults(form);
            })
            .subscribe(response => {
                this.loading = true;
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
        this.clearFile();
    }
}
