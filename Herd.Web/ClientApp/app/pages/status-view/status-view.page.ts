import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { NotificationsService } from "angular2-notifications";
import { ActivatedRoute, ParamMap } from '@angular/router';
import { Observable } from "rxjs/Observable";

import { AccountService, EventAlertService, StatusService } from "../../services";
import { Account, Status, PagedList } from '../../models/mastodon';
import { Storage, EventAlertEnum, ListTypeEnum } from '../../models';
import { BsModalComponent } from "ng2-bs3-modal/ng2-bs3-modal";
import { TabsetComponent } from "ngx-bootstrap";
import { Subscription } from "rxjs/Rx";
import { AccountListComponent, StatusTimelineComponent } from "../../components/index";


@Component({
    selector: 'status-view',
    templateUrl: './status-view.page.html',
    styleUrls: ['./status-view.page.css'],
})
export class StatusViewPage implements OnInit {

    constructor(
        private accountService: AccountService, private eventAlertService: EventAlertService,
        private localStorage: Storage, private route: ActivatedRoute,
        private statusService: StatusService, private toastService: NotificationsService) {
    }

    ngOnInit() {

    }

}