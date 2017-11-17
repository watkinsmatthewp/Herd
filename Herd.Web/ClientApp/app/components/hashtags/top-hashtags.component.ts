import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';


import { StatusService } from "../../services";


@Component({
    selector: 'top-hashtags',
    templateUrl: './top-hashtags.component.html',
    styleUrls: ['./top-hashtags.component.css']
})
export class TopHashtagsComponent implements OnInit {
    hashtags: string[] = [];

    ngOnInit() {
        this.getPopularHashtags();
    }

    getPopularHashtags() {
        // call service to get hashtags, for now just mocked.
        this.hashtags.push("lunch", "ipreo", "Avengers", "IronMan", "BlackWidow", "CaptainAmerica", "TheHulk", "NickFury", "DrStrange", "ClintBarton");
    }
}