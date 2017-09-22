import { Component, OnInit } from '@angular/core';
import { MastodonService } from '../shared/services/mastodon.service';
import { Post } from '../shared/models/Post';

@Component({
    selector: 'home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
    randomInt: number;

    // List of posts for the home feed
    homeFeed: Post[];

    constructor(private mastodonService: MastodonService) {
    }

    ngOnInit() {
        this.mastodonService.getRandomNumber().then(randomNum => {
            console.log("Random Number", randomNum);
            this.randomInt = randomNum.numero;
        })

        this.mastodonService.getHomeFeed()
            .then(feed => {
                this.homeFeed = feed;
        })
    }
}