import { Component, OnInit, ViewChild, EventEmitter } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';

import { TabsetComponent } from 'ngx-bootstrap';
import { NotificationsService } from "angular2-notifications";
import { BsModalComponent } from "ng2-bs3-modal/ng2-bs3-modal";

import { StatusService, EventAlertService, AccountService } from "../../services";
import { EventAlertEnum, Storage } from '../../models';
import { Status, Account, PagedList } from "../../models/mastodon";

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

    account: Account = new Account();
    statusList: PagedList<Status> = new PagedList<Status>();
    newStatusList: PagedList<Status> = new PagedList<Status>();
    //homeFeed: Status[] = [];
    //newItems: Status[] = [];
    
    hashtags: string[] = [];

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

        setInterval(() => { this.checkForNewStatuses(); }, 10 * 1000);
        this.getMostRecentHomeFeed();
        this.getaccount();
        this.getPopularHashtags();
    }

    getaccount() {
        let currentUser = JSON.parse(this.localStorage.getItem('currentUser'));
        let userID = currentUser.MastodonConnection.MastodonUserID;
        this.accountService.search({ mastodonUserID: userID, includeFollowedByActiveUser: true, includeFollowsActiveUser: true })
            .map(response => response.Items[0] as Account)
            .subscribe(account => {
                this.account = account;
            });
    }

    getMostRecentHomeFeed() {
        this.loading = true;
        let progress = this.toastService.info("Retrieving", "home timeline ...", { timeOut: 0 });

        this.statusService.search({ onlyOnActiveUserTimeline: true })
            .finally(() => this.loading = false)
            .subscribe(statusList => {
                this.toastService.remove(progress.id);
                this.statusList = statusList;
            }, error => {
                this.toastService.error("Error", error.error);
            });
    }

    getPopularHashtags() {
        // call service to get hashtags, for now just mocked.
        this.hashtags.push("lunch", "ipreo", "Avengers", "IronMan", "BlackWidow", "CaptainAmerica", "TheHulk", "NickFury", "DrStrange", "ClintBarton");
    }

    checkForNewStatuses() {
        this.statusService.search({ onlyOnActiveUserTimeline: true, sinceID: this.statusList.Items[0].Id })
            .finally(() => this.loading = false)
            .subscribe(newStatusList => {
                this.newStatusList = newStatusList;
            });
    }

    getPreviousStatuses() {
        this.loading = true;
        this.statusService.search({ onlyOnActiveUserTimeline: true, maxID: this.statusList.PageInformation.EarlierPageMaxID })
            .finally(() => this.loading = false)
            .subscribe(newStatusList => {
                this.appendItems(this.statusList.Items, newStatusList.Items);
                this.statusList.PageInformation = newStatusList.PageInformation;
                this.statusesWrapper.nativeElement.scrollTo(0, this.statusesWrapper.nativeElement.scrollTop);
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

    /**
     * Add the new items to main feed array, scroll to top, empty newItems
     */
    viewNewItems() {
        this.prependItems(this.statusList.Items, this.newStatusList.Items);
        this.scrollToTop();
        this.newStatusList = new PagedList<Status>();
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
        this.getPreviousStatuses();
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