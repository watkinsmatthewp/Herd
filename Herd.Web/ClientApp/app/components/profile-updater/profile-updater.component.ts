import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';

import { AccountService, EventAlertService } from "../../services";
import { NotificationsService } from "angular2-notifications";

@Component({
    selector: 'profile-updater',
    templateUrl: './profile-updater.component.html',
    styleUrls: ['./profile-updater.component.css']
})
export class ProfileUpdaterComponent implements OnInit {

    model: any = {
        display: "",
        bio: "",
        avatarBase64: "",
        headerBase64: "",
    };

    constructor(private accountService: AccountService, private eventAlertService: EventAlertService,
        private toastService: NotificationsService) { }

    ngOnInit() {
        
    }

    onHeaderFileChange(event: any) {
        let fileList: FileList = event.target.files;
        if (fileList.length > 0) {
            let file = fileList[0];
            this.model.headerFile = file;
            this.model.headerFilename = file.name;

            let reader = new FileReader();
            reader.readAsDataURL(event.target.files[0]);
            reader.onload = (e: any) => {
                this.model.headerBase64 = reader.result.split("base64,")[1];
                
            }
        }
    }

    onAvatarFileChange(event: any) {
        let fileList: FileList = event.target.files;
        console.log("Made it here");
        if (fileList.length > 0) {
            let file = fileList[0];
            this.model.avatarFile = file;
            this.model.avatarFilename = file.name;

            let reader = new FileReader();
            reader.readAsDataURL(event.target.files[0]);
            reader.onload = (e: any) => {
                //console.log(reader.result);
                this.model.avatarBase64 = reader.result.split("base64,")[1];
                console.log(this.model.avatarBase64);
            }
        }
    }

    submitUpdate(form: NgForm) {
        if (this.model.diplay === "")
            this.model.diplay = null;
        if (this.model.bio === "")
            this.model.bio = null;
        if (this.model.avatarBase64 === "")
            this.model.avatarBase64 = null;
        if (this.model.headerBase64 === "")
            this.model.headerBase64 = null;

        this.accountService.updateUserMastodonAccount(this.model.display, this.model.bio,
            this.model.avatarBase64, this.model.headerBase64)
            .finally(() => {

            })
            .subscribe(response => {
                this.toastService.success("Successfully", "Updated Profile");
            }, error => {
                this.toastService.error(error.error);
            });

    }

}