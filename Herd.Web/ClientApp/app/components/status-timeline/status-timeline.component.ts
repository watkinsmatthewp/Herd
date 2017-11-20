import { Component, OnInit, Input, ViewChild } from '@angular/core';

import { TimelineTypeEnum } from "../../models";
import { StatusService } from "../../services";
import { PagedList, Status } from "../../models/mastodon";
import { NotificationsService } from "angular2-notifications";

@Component({
    selector: 'status-timeline',
    templateUrl: './status-timeline.component.html',
    styleUrls: ['./status-timeline.component.css']
})
export class StatusTimelineComponent implements OnInit {
    @Input() timelineType: TimelineTypeEnum;
    @Input() showStatusForm: boolean = false;
    @Input() autoCheckForStatuses: boolean = false;
    @Input() userID: string;

    @ViewChild('statusesWrapper') statusesWrapper: any;

    statusList: PagedList<Status> = new PagedList<Status>();
    newStatusList: PagedList<Status> = new PagedList<Status>();

    getInitialFeed: Function;
    getPreviousStatuses: Function;
    checkForNewStatuses: Function;


    loading: boolean = false;


    constructor(private statusService: StatusService, private toastService: NotificationsService) {}

    ngOnInit() {
        // Check for required input
        if (this.timelineType < 0) throw new Error("TimelineType is required");

        this.setupFunctions();
        this.getInitialFeed();
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

                // Call on scroll down
                this.getPreviousStatuses = function (): void {
                    this.loading = true;
                    this.statusService.search({ onlyOnActiveUserTimeline: true, maxID: this.statusList.PageInformation.EarlierPageMaxID })
                        .finally(() => this.loading = false)
                        .subscribe(newStatusList => {
                            this.appendItems(this.statusList.Items, newStatusList.Items);
                            this.statusList.PageInformation = newStatusList.PageInformation;
                            this.statusesWrapper.nativeElement.scrollTo(0, this.statusesWrapper.nativeElement.scrollTop);
                        });
                }

                // Function to check for new statuses
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
                this.getInitialFeed = function (): void {
                    this.statusService.search({ authorMastodonUserID: this.userID })
                        .subscribe(statusList => {
                            this.statusList = statusList;
                        }, error => {
                            this.toastService.error("Error", error.error);
                        });
                }

                this.getPreviousStatuses = function (): void {
                    this.loading = true;
                    this.statusService.search({ authorMastodonUserID: this.userID, maxID: this.statusList.PageInformation.EarlierPageMaxID })
                        .finally(() => this.loading = false)
                        .subscribe(newStatusList => {
                            this.appendItems(this.statusList.Items, newStatusList.Items);
                            this.statusList.PageInformation = newStatusList.PageInformation;
                            this.statusesWrapper.nativeElement.scrollTo(0, this.statusesWrapper.nativeElement.scrollTop);
                        });
                }

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
                this.getInitialFeed = function (): void {

                }

                this.getPreviousStatuses = function (): void {

                }

                this.checkForNewStatuses = function (): void {

                }
                break;
            }
        }

        if (this.autoCheckForStatuses) setInterval(() => { this.checkForNewStatuses(); }, 10 * 1000);
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
