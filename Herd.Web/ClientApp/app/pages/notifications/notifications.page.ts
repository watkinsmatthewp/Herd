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
        this.testNotification.Type = "reblog";
        this.testNotification.CreatedAt = new Date();
        this.testNotification.Status = new Status();
        this.testNotification.Account = new Account();

        this.testNotification.Account.MastodonProfileImageURL = "https://vignette.wikia.nocookie.net/winniethepooh/images/2/2a/Roo.PNG/revision/latest?cb=20081015165029";
        this.testNotification.Account.FollowersCount = 5;
        this.testNotification.Account.FollowingCount = 80;
        this.testNotification.Account.FollowsActiveUser = true;
        this.testNotification.Account.IsFollowedByActiveUser = true;
        this.testNotification.Account.MastodonDisplayName = "Roo";
        this.testNotification.Account.MastodonHeaderImageURL = "http://fbcoverstreet.com/content/SORQN5PIFMJ874eKUboYNZ2vySd1tQjaAqhm9h7WhHCRIHl9jC9cngV2Vfa4bYYx.jpg";
        this.testNotification.Account.MastodonShortBio = "Hello! I'm Roo the software engineer!.";
        this.testNotification.Account.MastodonUserId = "6";
        this.testNotification.Account.MastodonUserName = "jake-a-roo";
        this.testNotification.Account.PostCount = 67;

        this.testNotification.Status.Author = new Account();
        this.testNotification.Status.Author.MastodonProfileImageURL = "https://pbs.twimg.com/profile_images/420241225283674113/xoCDeFzV.jpeg";
        this.testNotification.Status.Author.FollowersCount = 87;
        this.testNotification.Status.Author.FollowingCount = 346;
        this.testNotification.Status.Author.FollowsActiveUser = true;
        this.testNotification.Status.Author.IsFollowedByActiveUser = true;
        this.testNotification.Status.Author.MastodonDisplayName = "Dat Boi";
        this.testNotification.Status.Author.MastodonHeaderImageURL = "http://fbcoverstreet.com/content/5gOXvxOR1KodtoEyZX1ZHRwNKVaKWykc2fTKQrmKkrOoc3fLPaKxGogmtANZWE5B.jpg";
        this.testNotification.Status.Author.MastodonShortBio = "I like to code.";
        this.testNotification.Status.Author.MastodonUserId = "9";
        this.testNotification.Status.Author.MastodonUserName = "boi";
        this.testNotification.Status.Author.PostCount = 875;

        this.testNotification.Status.Content = "I wonder how pointers work internally.";
        this.testNotification.Status.CreatedOnUTC = new Date();
        this.testNotification.Status.FavouritesCount = 77;
        this.testNotification.Status.Id = "5567";
        this.testNotification.Status.InReplyToPostId = "3";
        this.testNotification.Status.IsFavourited = true;
        this.testNotification.Status.IsReblogged = false;
        this.testNotification.Status.IsSensitive = false;
        // Omit media attachment
        this.testNotification.Status.ReblogCount = 0;
        // Omit rest of status



    }



}
