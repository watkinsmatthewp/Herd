import { Component, OnInit } from '@angular/core';
import { MastodonService } from '../shared/services/mastodon.service';

@Component({
    selector: 'home',
    templateUrl: './home.component.html'
})
export class HomeComponent implements OnInit {
    randomInt: number;
    timelinePosts: Object[]; // This is the best thing I could find so far for a list

    constructor(private mastodonService: MastodonService) {
    }

    ngOnInit() {
        this.mastodonService.getRandomNumber().then(randomNum => {
            console.log("Random Number", randomNum);
            this.randomInt = randomNum.numero;
        })

        this.mastodonService.getHomeFeed().then(homeFeed => {
            //this.timelinePosts = homeFeed; // This is def wrong but I think the right format
        })
    }
}