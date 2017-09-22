import { Component, OnInit } from '@angular/core';
import { MastodonService } from '../shared/services/mastodon.service';

@Component({
    selector: 'home',
    templateUrl: './home.component.html'
})
export class HomeComponent implements OnInit {
    randomInt: number;
    
    constructor(private mastodonService: MastodonService) {}

    ngOnInit() {
        this.mastodonService.getRandomNumber().then(randomNum => {
            this.randomInt = randomNum.numero;
        })
    }
}
