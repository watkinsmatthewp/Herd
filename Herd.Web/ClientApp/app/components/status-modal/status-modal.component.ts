import { AfterViewInit, Component, OnInit, Input, OnDestroy } from '@angular/core';

import { Status } from '../../models/mastodon';
import { StatusService } from "../../services";
import { NotificationsService } from "angular2-notifications";

@Component({
    selector: 'status-modal',
    templateUrl: './status-modal.component.html',
    styleUrls: ['../status/status.component.css']
})
export class StatusModalComponent implements OnInit, AfterViewInit, OnDestroy {
    @Input() status: Status;
    @Input() statusId: string;
    @Input() modalId: string;
    @Input() autoOpen: boolean; // after initalization should we auto open

    constructor(private statusService: StatusService, private toastService: NotificationsService) { }

    ngOnInit(): void {
        if (this.statusId) {
            this.statusService.getStatus(this.statusId, true, true)
                .subscribe(data => {
                    this.status = data;
                    this.status.Ancestors = data.Ancestors;
                    this.status.Descendants = data.Descendants;
                }, error => {
                    this.toastService.error(error);
                });
        }
    }

    ngAfterViewInit(): void {
        if (this.autoOpen) {
            var openButton = document.getElementById("openButton-" + this.modalId);
            if (openButton) {
                openButton.click();
            }
        }
    }

    ngOnDestroy(): void {
        var closeButton = document.getElementById("closeButton-" + this.modalId);
        if (closeButton) {
            closeButton.click();
        }
    }
}
