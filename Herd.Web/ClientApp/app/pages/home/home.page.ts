import { Component, OnInit, ViewChild, EventEmitter } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';

import { TabsetComponent } from 'ngx-bootstrap';
import { NotificationsService } from "angular2-notifications";
import { BsModalComponent } from "ng2-bs3-modal/ng2-bs3-modal";

import { StatusService, EventAlertService, AccountService } from "../../services";
import { EventAlertEnum, ListTypeEnum, Storage } from '../../models';
import { Account, Hashtag, PagedList, Status } from "../../models/mastodon";

@Component({
    selector: 'home',
    templateUrl: './home.page.html',
    styleUrls: ['./home.page.css']
})
export class HomePage implements OnInit {
    public listTypeEnum = ListTypeEnum;
    @ViewChild('specificStatusModal') specificStatusModal: BsModalComponent;
    @ViewChild('replyStatusModal') replyStatusModal: BsModalComponent;

    account: Account = new Account();
    loading: boolean = false;
    // Modal Variables
    statusId: number;
    specificStatus: Status;
    replyStatus: Status;
    

    constructor(private activatedRoute: ActivatedRoute, private eventAlertService: EventAlertService,
                private toastService: NotificationsService, private statusService: StatusService,
                private accountService: AccountService, private localStorage: Storage) { }

    ngOnInit() {
        this.eventAlertService.getMessage().subscribe(event => {
            switch (event.eventType) {
                case EventAlertEnum.UPDATE_SPECIFIC_STATUS: {
                    let statusID: string = event.statusID;
                    this.updateSpecificStatus(statusID);
                    break;
                }
                case EventAlertEnum.UPDATE_REPLY_STATUS: {
                    let statusID: string = event.statusID;
                    this.updateReplyStatusModal(statusID);
                    break;
                }
            }
        });

        this.getAccount();
    }

    getAccount() {
        let currentUser = JSON.parse(this.localStorage.getItem('currentUser'));
        let userID = currentUser.MastodonConnection.MastodonUserID;
        this.accountService.search({ mastodonUserID: userID, includeFollowedByActiveUser: true, includeFollowsActiveUser: true })
            .map(response => response.Items[0] as Account)
            .subscribe(account => {
                this.account = account;
            });
    }

    updateSpecificStatus(statusId: string): void {
        this.loading = true;
        //let progress = this.toastService.info("Retrieving" , "status info ...");
        this.statusService.search({ postID: statusId, includeAncestors: true, includeDescendants: true })
            .map(postList => postList.Items[0] as Status)
            .finally(() =>  this.loading = false)
            .subscribe(data => {
               // this.toastService.remove(progress.id);
                this.specificStatus = data;
                this.specificStatus.Ancestors = data.Ancestors;
                this.specificStatus.Descendants = data.Descendants;
                this.specificStatusModal.open();
                //this.toastService.success("Finished", "retrieving status.")
            }, error => {
                this.toastService.error("Error", error.error);
            });
    }

    updateReplyStatusModal(statusId: string): void {
        this.loading = true;
        //let progress = this.toastService.info("Retrieving",  "status info ...");
        this.statusService.search({ postID: statusId, includeAncestors: false, includeDescendants: false })
            .map(postList => postList.Items[0] as Status)
            .finally(() => this.loading = false)
            .subscribe(data => {
                //this.toastService.remove(progress.id);
                this.replyStatus = data;
                this.replyStatusModal.open();
                //this.toastService.success("Finished", "retrieving status.")
            }, error => {
                this.toastService.error("Error", error.error);
            });
    }

}