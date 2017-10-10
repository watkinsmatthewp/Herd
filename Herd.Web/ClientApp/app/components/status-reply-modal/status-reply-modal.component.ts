import { AfterViewInit, Component, OnInit, Input, OnDestroy } from '@angular/core';

import { Status } from '../../models/mastodon';

@Component({
    selector: 'status-reply-modal',
    templateUrl: './status-reply-modal.component.html'
})
export class StatusReplyModalComponent implements OnInit, AfterViewInit, OnDestroy {
    @Input() status: Status;
    @Input() statusId: number;
    @Input() modalId: string;
    @Input() autoOpen: boolean; // after initalization should we auto open

    constructor() { }

    ngOnInit(): void { }

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
