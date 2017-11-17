import { Component, OnInit } from '@angular/core';
import { Notification, Status, Account } from '../../models/mastodon';

@Component({
    selector: 'notifications',
    templateUrl: './notifications.page.html',
})
export class NotificationsPage implements OnInit {

    testNotification: Notification;

    constructor() { }

    ngOnInit() {
        this.testNotification = new Notification();
        this.testNotification.Id = 5;
        this.testNotification.Type = "follow";
        this.testNotification.CreatedAt = new Date();
        this.testNotification.Status = new Status();
        this.testNotification.Account = new Account();
        this.testNotification.Account.MastodonProfileImageURL = "https://vignette.wikia.nocookie.net/scratchpad/images/8/88/Roo.jpeg/revision/latest?cb=20121220182049"
    }



}
