import { Component, OnInit, ViewChild, EventEmitter } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';

import { TabsetComponent } from 'ngx-bootstrap';
import { NotificationsService } from "angular2-notifications";
import { BsModalComponent } from "ng2-bs3-modal/ng2-bs3-modal";

import { StatusService, EventAlertService, AccountService } from "../../services";
import { EventAlertEnum, Storage } from '../../models';
import { Status, UserCard } from "../../models/mastodon";

@Component({
    selector: 'home',
    templateUrl: './home.page.html',
    styleUrls: ['./home.page.css']
})
export class HomePage implements OnInit {
    @ViewChild('specificStatusModal') specificStatusModal: BsModalComponent;
    @ViewChild('replyStatusModal') replyStatusModal: BsModalComponent;
    @ViewChild('statusesWrapper') statusesWrapper: any;

    statusId: number;
    specificStatus: Status;
    replyStatus: Status;

    loading: boolean = false;
    homeFeed: Status[] = [];
    newItems: Status[] = [];
    userCard: UserCard;

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

        setInterval(() => { this.checkForNewItems(); }, 10 * 1000);
        this.getMostRecentHomeFeed();
        this.getUserCard();
    }

    getUserCard() {
        let currentUser = JSON.parse(this.localStorage.getItem('currentUser'));
        let userID = currentUser.MastodonConnection.MastodonUserID;
        this.accountService.getUserByID(userID)
            .map(response => response as UserCard)
            .subscribe(usercard => {
                this.userCard = usercard;
            });
    }

    getMostRecentHomeFeed() {
        this.loading = true;
        this.statusService.getHomeFeed()
            .finally(() => this.loading = false)
            .subscribe(feed => {
                this.homeFeed = feed;
            }, error => {
                this.toastService.error("Error", error.error);
            });
    }

    checkForNewItems() {
        this.statusService.checkForNewItems(this.homeFeed[0].Id)
            .finally(() => this.loading = false)
            .subscribe(newItems => {
                this.newItems = newItems;
            });
    }

    getPreviousItems() {
        this.loading = true;
        this.statusService.getPreviousItems(this.homeFeed[this.homeFeed.length - 1].Id)
            .finally(() => this.loading = false)
            .subscribe(new_items => {
                this.appendItems(this.homeFeed, new_items);
                let currentYPosition = this.statusesWrapper.nativeElement.scrollTop;
                this.statusesWrapper.nativeElement.scrollTo(0, currentYPosition);
            });
    }

    updateSpecificStatus(statusId: string): void {
        this.loading = true;
        let progress = this.toastService.info("Retrieving" , "status info ...");
        this.statusService.getStatus(statusId, true, true)
            .finally(() =>  this.loading = false)
            .subscribe(data => {
                this.toastService.remove(progress.id);
                this.specificStatus = data;
                this.specificStatus.Ancestors = data.Ancestors;
                this.specificStatus.Descendants = data.Descendants;
                this.specificStatusModal.open();
                this.toastService.success("Finished", "retrieving status.")
            }, error => {
                this.toastService.error("Error", error.error);
            });
    }

    updateReplyStatusModal(statusId: string): void {
        this.loading = true;
        let progress = this.toastService.info("Retrieving",  "status info ...");
        this.statusService.getStatus(statusId, false, false)
            .finally(() => this.loading = false)
            .subscribe(data => {
                this.toastService.remove(progress.id);
                this.replyStatus = data;
                this.replyStatusModal.open();
                this.toastService.success("Finished", "retrieving status.")
            }, error => {
                this.toastService.error("Error", error.error);
            });
    }

    /**
     * Add the new items to main feed array, scroll to top, empty newItems
     */
    viewNewItems() {
        this.prependItems(this.homeFeed, this.newItems);
        this.scrollToTop();
        this.newItems = [];
    }

    /**
     * Scrolls the status area to the top
     */
    scrollToTop() {
        this.statusesWrapper.nativeElement.scrollTo(0, 0);
    }

    /**
     * Infinite scroll function that is called
     * when scrolling down and near end of view port
     * @param ev
     */
    onScrollDown(ev: any) {
        this.getPreviousItems();
    }

    /** Infinite Scrolling Handling */
    addItems(oldItems: any[], newItems: any[], _method: any) {
        oldItems[_method].apply(oldItems, newItems);
    }

    /**
     * Add items to end of list
     * @param startIndex
     * @param endIndex
     */
    appendItems(oldItems: any[], newItems: any[]) {
        this.addItems(oldItems, newItems, 'push');
    }

    /**
     * Add items to beginning of list
     * @param startIndex
     * @param endIndex
     */
    prependItems(oldItems: any[], newItems: any[]) {
        this.addItems(oldItems, newItems, 'unshift');
    }
}