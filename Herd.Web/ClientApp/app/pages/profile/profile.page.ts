import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { NotificationsService } from "angular2-notifications";
import { ActivatedRoute, ParamMap } from '@angular/router';
import { Observable } from "rxjs/Observable";

import { AccountService, EventAlertService, StatusService } from "../../services";
import { Account, Status, PagedList } from '../../models/mastodon';
import { Storage, EventAlertEnum } from '../../models';
import { BsModalComponent } from "ng2-bs3-modal/ng2-bs3-modal";
import { TabsetComponent } from "ngx-bootstrap";
import { Subscription } from "rxjs/Rx";


@Component({
    selector: 'profile',
    templateUrl: './profile.page.html',
    styleUrls: ['./profile.page.css']
})
export class ProfilePage implements OnInit, AfterViewInit {
    @ViewChild('staticTabs') staticTabs: TabsetComponent;
    @ViewChild('specificStatusModal') specificStatusModal: BsModalComponent;
    @ViewChild('replyStatusModal') replyStatusModal: BsModalComponent;
    @ViewChild('statusesWrapper') statusesWrapper: any;
    @ViewChild('followingWrapper') followingWrapper: any;
    @ViewChild('followersWrapper') followersWrapper: any;

    // Modal Variables
    statusId: number;
    specificStatus: Status;
    replyStatus: Status;

    account: Account;
    followingList: PagedList<Account> = new PagedList<Account>();
    followerList: PagedList<Account> = new PagedList<Account>();
    statusList: PagedList<Status> = new PagedList<Status>();
    newStatusList: PagedList<Status> = new PagedList<Status>();

    isFollowing: boolean = false;
    followUnfollowText: string = "Following";
    loading: boolean = false;

    constructor(
        private accountService: AccountService,
        private eventAlertService: EventAlertService,
        private localStorage: Storage,
        private route: ActivatedRoute,
        private statusService: StatusService,
        private toastService: NotificationsService) {
    }

    /**
     * Set up subscriptions for alerts
     */
    ngOnInit() {
        // Monitor Param map to update user id 
        this.route.paramMap
            .switchMap((params: ParamMap) => Observable.of(params.get('id') || "-1"))
            .subscribe(userID => {
                this.getUserAccount(userID);
                this.getFollowing(userID);
                this.getFollowers(userID);
                this.getMostRecentUserPosts(userID);
            });

        setInterval(() => { this.checkForNewStatuses(); }, 10 * 1000);

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
                    this.getFollowers(this.account.MastodonUserId);
                    this.getFollowing(this.account.MastodonUserId);
                    break;
                }
            }
        });
    }

    /**
     * Update default tab
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

    /**
     * Get the posts that this user has made
     * @param userID
     */
    getMostRecentUserPosts(userID: string) {
        this.statusService.search({ authorMastodonUserID: userID })
            .subscribe(statusList => {
                this.statusList = statusList;
            }, error => {
                this.toastService.error("Error", error.error);
            });
    }

    /**
     * Get the follows of this user
     * @param userID
     */
    getFollowers(userID: string) {
        this.accountService.search({ followsMastodonUserID: userID, includeFollowsActiveUser: true })
            .subscribe(followerList => {
                this.followerList = followerList;
            });
    }

    /**
     * Get who this user is following
     * @param userID
     */
    getFollowing(userID: string) {
        this.accountService.search({ followedByMastodonUserID: userID, includeFollowedByActiveUser: true })
            .subscribe(followingList => {
                this.followingList = followingList;
            });
    }

    updateSpecificStatus(statusId: string): void {
        this.loading = true;
        //let progress = this.toastService.info("Retrieving", "status info ...");
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
        //let progress = this.toastService.info("Retrieving", "status info ...");
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

    checkForNewStatuses() {
        this.statusService.search({ authorMastodonUserID: this.account.MastodonUserId, sinceID: this.statusList.Items[0].Id })
            .finally(() => this.loading = false)
            .subscribe(newStatusList => {
                this.newStatusList = newStatusList;
            });
    }

    getPreviousStatuses() {
        this.loading = true;
        this.statusService.search({ authorMastodonUserID: this.account.MastodonUserId, maxID: this.statusList.PageInformation.EarlierPageMaxID })
            .finally(() => this.loading = false)
            .subscribe(newStatusList => {
                this.appendItems(this.statusList.Items, newStatusList.Items);
                this.statusList.PageInformation = newStatusList.PageInformation;
                this.statusesWrapper.nativeElement.scrollTo(0, this.statusesWrapper.nativeElement.scrollTop);
            });
    }

    getMoreFollowing() {
        let userID = this.account.MastodonUserId;
        this.accountService.search({ followedByMastodonUserID: userID, includeFollowedByActiveUser: true, maxID: this.followingList.PageInformation.EarlierPageMaxID })
            .subscribe(newUsersList => {
                if (newUsersList.Items.length > 0) {
                    this.appendItems(this.followingList.Items, newUsersList.Items);
                    this.followingList.PageInformation = newUsersList.PageInformation;
                    this.followingWrapper.nativeElement.scrollTo(0, this.followingWrapper.nativeElement.scrollTop);
                }
            });
    }

    getMoreFollowers() {
        let userID = this.account.MastodonUserId;
        this.accountService.search({ followsMastodonUserID: userID, includeFollowsActiveUser: true, maxID: this.followerList.PageInformation.EarlierPageMaxID })
            .subscribe(newUsersList => {
                if (newUsersList.Items.length > 0) {
                    this.appendItems(this.followerList.Items, newUsersList.Items);
                    this.followerList.PageInformation = newUsersList.PageInformation;
                    this.followersWrapper.nativeElement.scrollTo(0, this.followersWrapper.nativeElement.scrollTop);
                }
            });
    }

    /**
     * Add the new items to main feed array, scroll to top, empty newItems
     */
    viewNewStatuses() {
        this.prependItems(this.statusList.Items, this.newStatusList.Items);
        this.scrollToTop('statuses');
        this.newStatusList = new PagedList<Status>();
    }




    /**
     * Scrolls the status area to the top
     */
    scrollToTop(tab: string) {
        if (tab === 'statuses')
            this.statusesWrapper.nativeElement.scrollTo(0, 0);
        else if (tab === 'following')
            this.followingWrapper.nativeElement.scrollTo(0, 0);
        else if (tab === 'followers')
            this.followersWrapper.nativeElement.scrollTo(0, 0);
        
    }

    /**
     * Infinite scroll function that is called
     * when scrolling down and near end of view port
     * @param ev
     */
    onScrollDown(ev: any, tab: string) {
        if (tab === 'statuses')
            this.getPreviousStatuses();
        else if (tab === 'following')
            this.getMoreFollowing();
        else if (tab === 'followers')
            this.getMoreFollowers();
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
