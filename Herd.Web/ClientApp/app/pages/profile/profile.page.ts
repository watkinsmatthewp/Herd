import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { NotificationsService } from "angular2-notifications";
import { ActivatedRoute, ParamMap, NavigationEnd, Router } from '@angular/router';
import { Observable } from "rxjs/Observable";
import { Title } from "@angular/platform-browser";

import { AccountService, EventAlertService, StatusService } from "../../services";
import { Account, Status, PagedList } from '../../models/mastodon';
import { Storage, EventAlertEnum, ListTypeEnum } from '../../models';
import { BsModalComponent } from "ng2-bs3-modal/ng2-bs3-modal";
import { TabsetComponent } from "ngx-bootstrap";
import { Subscription } from "rxjs/Rx";
import { AccountListComponent, StatusTimelineComponent } from "../../components/index";



@Component({
    selector: 'profile',
    templateUrl: './profile.page.html',
    styleUrls: ['./profile.page.css'],
})
export class ProfilePage implements OnInit, AfterViewInit {
    public listTypeEnum = ListTypeEnum;
    @ViewChild('staticTabs') staticTabs: TabsetComponent;
    @ViewChild('statusList') statusList: StatusTimelineComponent;
    @ViewChild('followingList') followingList: AccountListComponent;
    @ViewChild('followerList') followerList: AccountListComponent;
    @ViewChild('specificStatusModal') specificStatusModal: BsModalComponent;
    @ViewChild('replyStatusModal') replyStatusModal: BsModalComponent;

    account: Account;
    isFollowing: boolean = false;
    followUnfollowText: string = "Following";
    showFullBanner: boolean = true;
    loading: boolean = false;
    // Modal Variables
    statusId: number;
    specificStatus: Status;
    replyStatus: Status;

    constructor(
        private accountService: AccountService, private router: Router, private titleService: Title, private eventAlertService: EventAlertService,
        private localStorage: Storage, private route: ActivatedRoute,
        private statusService: StatusService, private toastService: NotificationsService) {
        router.events.subscribe(event => {
            if (event instanceof NavigationEnd) {
                var title = this.getTitle(router.routerState, router.routerState.root).join('-');
                titleService.setTitle(title);
            }
        });
    }

    private getTitle(state: any, parent: any): any {
        var data = [];
        if (parent && parent.snapshot.data && parent.snapshot.data.title) {
            data.push(parent.snapshot.data.title);
        }

        if (state && parent) {
            data.push(... this.getTitle(state, state.firstChild(parent)));
        }
        return data;
    }

    /**
     * Set up subscriptions for alerts
     */
    ngOnInit() {
        // Monitor Param map to update user id 
        this.route.paramMap
            .switchMap((params: ParamMap) => Observable.of(params.get('id') || "-1"))
            .subscribe(userID => {
                // if id switches we need to update the entire page again
                this.getUserAccount(userID);
                if (this.followingList && this.followerList && this.statusList) {
                    this.followerList.userID = userID;
                    this.followingList.userID = userID;
                    this.statusList.userID = userID;
                }
            });

        // Setup subscription to update modals on status click
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
                case EventAlertEnum.UPDATE_FOLLOWING_AND_FOLLOWERS: {
                    this.followerList.getInitialItems();
                    this.followingList.getInitialItems();
                    break;
                }
            }
        });
    }

    /**
     * Update selected tab
     */
    ngAfterViewInit() {
        this.route.queryParams
            .subscribe(params => {
                let tabIndex: number = params['tabIndex'] || 0;
                setTimeout(() => this.staticTabs.tabs[tabIndex].active = true);
            });
    }

    isCurrentUser(): boolean {
        let currentUser = JSON.parse(this.localStorage.getItem('currentUser'));
        let userID = currentUser.MastodonConnection.MastodonUserID;
        if (userID === this.account.MastodonUserId) {
            return true;
        }
        return false;
    }

    toggleShowBanner(): void {
        this.showFullBanner = !this.showFullBanner;
    }

    /**
     * Toggle following a user
     */
    togglefollow(): void {
        this.accountService.followUser(String(this.account.MastodonUserId), !this.isFollowing)
            .subscribe(response => {
                this.isFollowing = !this.isFollowing;
                this.eventAlertService.addEvent(EventAlertEnum.UPDATE_FOLLOWING_AND_FOLLOWERS);
                this.toastService.success("Successfully", "changed relationship.");
            }, error => {
                this.toastService.error(error.error);
            });
    }

    /**
     * Given a userID, get that Users account
     * @param userId
     */
    getUserAccount(userID: string) {
        this.loading = true;
        let progress = this.toastService.info("Retrieving", "account information ...", { timeOut: 0 });
        this.accountService.search({ mastodonUserID: userID, includeFollowedByActiveUser: true })
            .map(response => response.Items[0] as Account)
            .finally(() => this.loading = false)
            .subscribe(account => {
                this.toastService.remove(progress.id);
                this.account = account;
                if (this.account.IsFollowedByActiveUser) {
                    this.isFollowing = true;
                }
            }, error => {
                this.toastService.error("Error", error.error);
            });
    }

    /** ----------------------------------------------------------- Modal Actions ----------------------------------------------------------- */

    updateSpecificStatus(statusId: string): void {
        this.loading = true;
        //let progress = this.toastService.info("Retrieving", "status info ...", { showProgressBar: false, pauseOnHover: false });
        this.statusService.search({ postID: statusId, includeAncestors: true, includeDescendants: true })
            .map(postList => postList.Items[0] as Status)
            .finally(() => this.loading = false)
            .subscribe(data => {
                //this.toastService.remove(progress.id);
                this.specificStatus = data;
                this.specificStatus.Ancestors = data.Ancestors;
                this.specificStatus.Descendants = data.Descendants;
                this.specificStatusModal.open();
               // this.toastService.success("Finished", "retrieving status.")
            }, error => {
                this.toastService.error("Error", error.error);
            });
    }

    updateReplyStatusModal(statusId: string): void {
        this.loading = true;
        //let progress = this.toastService.info("Retrieving", "status info ...", { showProgressBar: false, pauseOnHover: false });
        this.statusService.search({ postID: statusId, includeAncestors: false, includeDescendants: false })
            .map(postList => postList.Items[0] as Status)
            .finally(() => this.loading = false)
            .subscribe(data => {
               // this.toastService.remove(progress.id);
                this.replyStatus = data;
                this.replyStatusModal.open();
               // this.toastService.success("Finished", "retrieving status.")
            }, error => {
                this.toastService.error("Error", error.error);
            });
    }

}
