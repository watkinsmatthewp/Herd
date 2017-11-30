import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { Observable } from "rxjs/Observable";
import { Subscription } from "rxjs/Rx";

import { StatusService } from "../../services";
import { Status } from '../../models/mastodon';
import { NotificationsService } from "angular2-notifications";


@Component({
    selector: 'status-view',
    templateUrl: './status-view.page.html',
    styleUrls: ['./status-view.page.css'],
})
export class StatusViewPage implements OnInit {
    status: Status; 
    loading: boolean = false;
    

    constructor(private route: ActivatedRoute, private statusService: StatusService,
        private toastService: NotificationsService) {
    }

    ngOnInit() {
        // Monitor Param map to update status id 
        this.route.paramMap
            .switchMap((params: ParamMap) => Observable.of(params.get('id') || "-1"))
            .subscribe(statusID => {
                // if id switches we need to update the entire page again
                this.getStatus(statusID);
            });
        
    }

    private getStatus(statusID: string) {
        this.loading = true;
        let progress = this.toastService.info("Retrieving", "status info ...", { showProgressBar: false, pauseOnHover: false });
        this.statusService.search({ postID: statusID, includeAncestors: true, includeDescendants: true })
            .map(postList => postList.Items[0] as Status)
            .finally(() => this.loading = false)
            .subscribe(status => {
                this.toastService.remove(progress.id);
                this.status = status;
                this.status.Ancestors = status.Ancestors;
                this.status.Descendants = status.Descendants;
                this.toastService.success("Finished", "retrieving status.")
            }, error => {
                this.toastService.error("Error", error.error);
            });
    }

}