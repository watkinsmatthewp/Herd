import { Component, OnInit, Input, ViewChild, OnChanges, SimpleChanges } from '@angular/core';

import { TimelineTypeEnum } from "../../models";
import { StatusService } from "../../services";
import { PagedList, Status } from "../../models/mastodon";
import { NotificationsService } from "angular2-notifications";

@Component({
    selector: 'status-timeline',
    templateUrl: './status-timeline.component.html',
    styleUrls: ['./status-timeline.component.css']
})
export class StatusTimelineComponent implements OnInit, OnChanges {
    @Input() timelineType: TimelineTypeEnum;
    @Input() showStatusForm: boolean = false;
    @Input() autoCheckForStatuses: boolean = false;
    @Input() userID: string;
    @Input() search: string;
    @ViewChild('ps') scrollBar: any;
    // Status Lists 
    statusList: PagedList<Status> = new PagedList<Status>();
    newStatusList: PagedList<Status> = new PagedList<Status>();
    // Functions to call 
    getInitialFeed: Function;
    getPreviousStatuses: Function;
    checkForNewStatuses: Function;
    // Loading variable
    loading: boolean = false;


    constructor(private statusService: StatusService, private toastService: NotificationsService) {}

    ngOnInit() {
        // Check for required input - we AT LEAST need to know which types of statuses to get
        if (this.timelineType < 0) throw new Error("TimelineType is required");

        this.setupFunctions();
        this.getInitialFeed();
    }

    /**
     * On state changes do stuff
     * @param changes shows old value vs new value of state change
     */
    ngOnChanges(changes: SimpleChanges): void {
        if (changes.search && changes.search.previousValue) {
            this.getInitialFeed();
        }
    }

    private setupFunctions() {
        switch (+this.timelineType) {
            case TimelineTypeEnum.HOME: {
                // Set getInitialFeed 
                this.getInitialFeed = function (): void {
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

                // Set getPreviousStatuses
                this.getPreviousStatuses = function (): void {
                    this.loading = true;
                    this.statusService.search({ onlyOnActiveUserTimeline: true, maxID: this.statusList.PageInformation.EarlierPageMaxID })
                        .finally(() => this.loading = false)
                        .subscribe(newStatusList => {
                            this.appendItems(this.statusList.Items, newStatusList.Items);
                            this.statusList.PageInformation = newStatusList.PageInformation;
                            this.triggerScroll();
                        });
                }

                // Set checkForNewStatuses
                this.checkForNewStatuses = function (): void {
                    this.statusService.search({ onlyOnActiveUserTimeline: true, sinceID: this.statusList.Items[0].Id })
                        .finally(() => this.loading = false)
                        .subscribe(newStatusList => {
                            this.newStatusList = newStatusList;
                        });
                }
                break;
            }
            case TimelineTypeEnum.PROFILE: {
                // Set getInitialFeed 
                this.getInitialFeed = function (): void {
                    this.statusService.search({ authorMastodonUserID: this.userID })
                        .subscribe(statusList => {
                            this.statusList = statusList;
                        }, error => {
                            this.toastService.error("Error", error.error);
                        });
                }

                // Set getPreviousStatuses
                this.getPreviousStatuses = function (): void {
                    this.loading = true;
                    this.statusService.search({ authorMastodonUserID: this.userID, maxID: this.statusList.PageInformation.EarlierPageMaxID })
                        .finally(() => this.loading = false)
                        .subscribe(newStatusList => {
                            this.appendItems(this.statusList.Items, newStatusList.Items);
                            this.statusList.PageInformation = newStatusList.PageInformation;
                            this.triggerScroll();
                        });
                }

                // Set checkForNewStatuses
                this.checkForNewStatuses = function (): void {
                    this.statusService.search({ authorMastodonUserID: this.userID, sinceID: this.statusList.Items[0].Id })
                        .finally(() => this.loading = false)
                        .subscribe(newStatusList => {
                            this.newStatusList = newStatusList;
                        });
                }
                break;
            }
            case TimelineTypeEnum.SEARCH: {
                // Set getInitialFeed 
                this.getInitialFeed = function (): void {
                    this.statusService.search({ hashtag: this.search })
                        .subscribe(statusList => {
                            this.statusList = statusList;
                        });
                }

                // Set getPreviousStatuses
                this.getPreviousStatuses = function (): void {
                    this.loading = true;
                    this.statusService.search({ hashtag: this.search, maxID: this.statusList.PageInformation.EarlierPageMaxID })
                        .finally(() => this.loading = false)
                        .subscribe(newStatusList => {
                            this.appendItems(this.statusList.Items, newStatusList.Items);
                            this.statusList.PageInformation = newStatusList.PageInformation;
                            this.triggerScroll();
                        });
                }

                // Set checkForNewStatuses
                this.checkForNewStatuses = function (): void {
                    this.statusService.search({ hashtag: this.search, sinceID: this.statusList.Items[0].Id })
                        .finally(() => this.loading = false)
                        .subscribe(newStatusList => {
                            this.newStatusList = newStatusList;
                        });
                }
                break;
            }
        }

        if (this.autoCheckForStatuses) setInterval(() => { this.checkForNewStatuses(); }, 10 * 1000);
    }

    /** ----------------------------------------------------------- Button Actions ----------------------------------------------------------- */

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
        this.scrollBar.directiveRef.scrollToTop(0, 500);
    }

    /** ----------------------------------------------------------- Infinite Scrolling ----------------------------------------------------------- */

    triggerScroll() {
        this.scrollBar.directiveRef.scrollToY(this.scrollBar.directiveRef.position(true).y - 1);
        this.scrollBar.directiveRef.scrollToY(this.scrollBar.directiveRef.position(true).y + 1);
    }

    /**
     * When reach end of page, load next
     * @param event
     */
    reachEnd(event: any) {
        // This check prevents this from being called prematurely on page load
        if (event.target.getAttribute('class').indexOf("ps--scrolling-y") >= 0) {
            this.getPreviousStatuses();
        }
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
