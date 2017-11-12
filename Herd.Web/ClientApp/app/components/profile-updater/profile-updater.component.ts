import { Component, OnInit } from '@angular/core';

@Component({
    selector: 'profile-updater',
    templateUrl: './profile-updater.component.html',
    styleUrls: ['./profile-updater.component.css']
})
export class ProfileUpdaterComponent implements OnInit {

    model: any = {
        display: "",
    };

    constructor() { }

    ngOnInit() {
        
    }

    onHeaderFileChange(event: any) {
        let fileList: FileList = event.target.files;
        if (fileList.length > 0) {
            let file = fileList[0];
            this.model.headerFile = file;
            this.model.headerFilename = file.name;

            let reader = new FileReader();
            reader.onload = (e: any) => {
                
            }
            reader.readAsDataURL(event.target.files[0]);
        }
    }

    onAvatarFileChange(event: any) {
        let fileList: FileList = event.target.files;
        if (fileList.length > 0) {
            let file = fileList[0];
            this.model.avatarFile = file;
            this.model.avatarFilename = file.name;

            let reader = new FileReader();
            reader.onload = (e: any) => {
                
            }
            reader.readAsDataURL(event.target.files[0]);
        }
    }

}