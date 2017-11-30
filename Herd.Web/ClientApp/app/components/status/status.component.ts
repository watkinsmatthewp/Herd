import { Component, OnInit, Input, TemplateRef } from '@angular/core';
import { Router, UrlSerializer } from '@angular/router';

import { BsModalService } from 'ngx-bootstrap/modal';
import { BsModalRef } from 'ngx-bootstrap';
import { Image } from 'angular-modal-gallery';

import { StatusService, EventAlertService } from "../../services";
import { Status } from '../../models/mastodon';
import { EventAlertEnum, Storage } from "../../models";


@Component({
    selector: 'status',
    templateUrl: './status.component.html',
    styleUrls: ['./status.component.css']
})
export class StatusComponent implements OnInit {
    @Input() status: Status;
    modalRef: BsModalRef;
    showBlur: boolean = false;
    copyText: string = "";
    imagesArray: Array<Image> = [];

    constructor(private router: Router, private urlSerializer: UrlSerializer,
                private statusService: StatusService, private localStorage: Storage,
                private eventAlertService: EventAlertService, private modalService: BsModalService) { }

    ngOnInit() {
        if (this.status.IsSensitive === true) {
            this.showBlur = true;
        }

        // Add the status images to the imagesArray
        if (this.status.MediaAttachment) {
            this.imagesArray.push(new Image(this.status.MediaAttachment));
        }
        
        // Set copy url text
        let tree = this.router.createUrlTree(['status', this.status.Id]);
        let fullURL = window.location.origin + this.urlSerializer.serialize(tree);
        this.copyText = fullURL;
    }

    isCurrentUser(): boolean {
        let currentUser = JSON.parse(this.localStorage.getItem('currentUser'));
        let userID = currentUser.MastodonConnection.MastodonUserID;
        if (userID === this.status.Author.MastodonUserId) {
            return true;
        }
        return false;
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

    openDeleteModal(event: any, template: TemplateRef<any>) {
        this.modalRef = this.modalService.show(template, { class: 'modal-sm' });
        event.stopPropagation();
    }

    answerDeleteConfirmation(answer: boolean): void {
        if (answer) {
            this.statusService.delete(this.status.Id).subscribe();
            this.eventAlertService.addEvent(EventAlertEnum.REMOVE_STATUS, { statusID: this.status.Id });
        }
        this.modalRef.hide();
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
