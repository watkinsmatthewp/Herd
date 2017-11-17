import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';


import { StatusService } from "../../services";
import { Hashtag } from "../../models/mastodon";


@Component({
    selector: 'top-hashtags',
    templateUrl: './top-hashtags.component.html',
    styleUrls: ['./top-hashtags.component.css']
})
export class TopHashtagsComponent implements OnInit {
    hashtags: Hashtag[] = [];

    constructor(private statusService: StatusService) { }

    ngOnInit() {
        this.getPopularHashtags();
    }

    getPopularHashtags() {
        this.statusService.getTopHashtags().subscribe(hashtagList => {
            this.hashtags = hashtagList;
        });
    }
}