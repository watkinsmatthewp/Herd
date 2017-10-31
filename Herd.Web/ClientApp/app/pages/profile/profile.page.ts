import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { NotificationsService } from "angular2-notifications";
import { ActivatedRoute, ParamMap } from '@angular/router';
import { Observable } from "rxjs/Observable";

import { AccountService, EventAlertService, StatusService } from "../../services";
import { Account, Status } from '../../models/mastodon';
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

    statusId: number;
    specificStatus: Status;
    replyStatus: Status;

    account: Account;
    userPosts: Status[] = []; // List of posts from this user
    newItems: Status[] = [];
    following: Account[] = [];
    followers: Account[] = [];

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

        setInterval(() => { this.checkForNewItems(); }, 10 * 1000);

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
            .map(response => response[0] as Account)
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
            .subscribe(feed => {
                this.userPosts = feed;
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
            .subscribe(users => {
                this.followers = users;
            });
    }

    /**
     * Get who this user is following
     * @param userID
     */
    getFollowing(userID: string) {
        this.accountService.search({ followedByMastodonUserID: userID, includeFollowedByActiveUser: true })
            .subscribe(users => {
                this.following = users;
            });
    }

    updateSpecificStatus(statusId: string): void {
        this.loading = true;
        let progress = this.toastService.info("Retrieving", "status info ...");
        this.statusService.search({ postID: statusId, includeAncestors: true, includeDescendants: true })
            .map(posts => posts[0] as Status)
            .finally(() => this.loading = false)
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
        let progress = this.toastService.info("Retrieving", "status info ...");
        this.statusService.search({ postID: statusId, includeAncestors: false, includeDescendants: false })
            .map(posts => posts[0] as Status)
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

    checkForNewItems() {
        this.statusService.search({ authorMastodonUserID: this.account.MastodonUserId, sinceID: this.userPosts[0].Id })
            .finally(() => this.loading = false)
            .subscribe(newItems => {
                this.newItems = newItems;
            });
    }

    getPreviousStatuses() {
        this.loading = true;
        this.statusService.search({ authorMastodonUserID: this.account.MastodonUserId, maxID: this.userPosts[this.userPosts.length - 1].Id })
            .finally(() => this.loading = false)
            .subscribe(new_items => {
                this.appendItems(this.userPosts, new_items);
                let currentYPosition = this.statusesWrapper.nativeElement.scrollTop;
                this.statusesWrapper.nativeElement.scrollTo(0, currentYPosition);
            });
    }

    getMoreFollowing() {
        let userID = this.account.MastodonUserId;
        let lastID = this.following[this.following.length-1].MastodonUserId;
        this.accountService.search({ followedByMastodonUserID: userID, includeFollowedByActiveUser: true, sinceID: lastID })
            .subscribe(new_users => {
                this.appendItems(this.following, new_users);
                let currentYPosition = this.followingWrapper.nativeElement.scrollTop;
                this.followingWrapper.nativeElement.scrollTo(0, currentYPosition);
            });
    }

    getMoreFollowers() {
        let userID = this.account.MastodonUserId;
        let lastID = this.followers[this.followers.length-1].MastodonUserId;
        this.accountService.search({ followsMastodonUserID: userID, includeFollowsActiveUser: true, sinceID: lastID })
            .subscribe(new_users => {
                this.appendItems(this.followers, new_users);
                let currentYPosition = this.followersWrapper.nativeElement.scrollTop;
                this.followersWrapper.nativeElement.scrollTo(0, currentYPosition);
            });
    }

    /**
     * Add the new items to main feed array, scroll to top, empty newItems
     */
    viewNewStatuses() {
        this.prependItems(this.userPosts, this.newItems);
        this.scrollToTop('statuses');
        this.newItems = [];
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
