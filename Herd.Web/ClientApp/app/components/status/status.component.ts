import { Component, OnInit, Input } from '@angular/core';
import { Router } from '@angular/router';

import { Image } from 'angular-modal-gallery';

import { StatusService, EventAlertService } from "../../services";
import { Status } from '../../models/mastodon';
import { EventAlertEnum } from "../../models/index";


@Component({
    selector: 'status',
    templateUrl: './status.component.html',
    styleUrls: ['./status.component.css']
})
export class StatusComponent implements OnInit {
    @Input() status: Status;
    showBlur: boolean = false;
    imagesArray: Array<Image> = [];

    constructor(private router: Router, private statusService: StatusService, private eventAlertService: EventAlertService) {}

    ngOnInit() {
        if (this.status.IsSensitive === true) {
            this.showBlur = true;
        }

        // Add the status images to the imagesArray
        if (this.status.MediaAttachment) {
            this.imagesArray.push(new Image(this.status.MediaAttachment));
        }
    }

    turnOffBlur(event: any): void {
        this.showBlur = false;
        event.stopPropagation();
    }

    notifyTimelineCommentsClicked(event: any): void {
        this.eventAlertService.addEvent(EventAlertEnum.UPDATE_SPECIFIC_STATUS, { statusID: this.status.Id });
        event.stopPropagation();
    }

    notifyTimelineStatusClicked(event: any): void {
        this.eventAlertService.addEvent(EventAlertEnum.UPDATE_SPECIFIC_STATUS, { statusID: this.status.Id });
        event.stopPropagation();
    }

    toggleRepost(event: any) {
        this.statusService.repost(this.status.Id, !this.status.IsReblogged).subscribe(() => {
            this.status.IsReblogged = !this.status.IsReblogged;
            if (this.status.IsFavourited) {
                this.status.ReblogCount++;
            } else {
                this.status.ReblogCount--;
            }
        });
        event.stopPropagation();
    }

    toggleLike(event: any) {
        this.statusService.like(this.status.Id, !this.status.IsFavourited).subscribe(() => {
            this.status.IsFavourited = !this.status.IsFavourited;
            if (this.status.IsFavourited) {
                this.status.FavouritesCount++;
            } else {
                this.status.FavouritesCount--;
            }
        });
        event.stopPropagation();
    }

    /**
     * Set status form text to author of status
     * @param event
     */
    mention(event: any) {
        this.eventAlertService.addEvent(EventAlertEnum.UPDATE_STATUS_FORM_TEXT, { statusText: "@" + this.status.Author.MastodonUserName });
        event.stopPropagation();
    }

}
