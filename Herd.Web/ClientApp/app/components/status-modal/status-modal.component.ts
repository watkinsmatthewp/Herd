import { AfterViewInit, Component, OnInit, Input, OnDestroy } from '@angular/core';

import { Status } from '../../models/mastodon';
import { AlertService, MastodonService } from "../../services";

@Component({
    selector: 'status-modal',
    templateUrl: './status-modal.component.html',
    styleUrls: ['../status/status.component.css']
})
export class StatusModalComponent implements OnInit, AfterViewInit, OnDestroy {
    @Input() status: Status;
    @Input() statusId: number;
    @Input() modalId: string;
    @Input() autoOpen: boolean; // after initalization should we auto open

    constructor(private mastodonService: MastodonService, private alertService: AlertService) { }

    ngOnInit(): void {
        if (this.statusId) {
            this.mastodonService.getStatus(this.statusId, true, true)
                .subscribe(data => {
                    this.status = data;
                    this.status.Ancestors = data.Ancestors;
                    this.status.Descendants = data.Descendants;
                }, error => {
                    this.alertService.error(error);
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
