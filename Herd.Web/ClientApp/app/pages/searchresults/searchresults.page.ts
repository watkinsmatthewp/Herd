import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { Storage, ListTypeEnum } from '../../models';
import { StatusTimelineComponent, AccountListComponent } from "../../components/index";

@Component({
    selector: 'searchresults',
    templateUrl: './searchResults.page.html',
    styleUrls: ['./searchResults.page.css']
})
export class SearchResultsPage implements OnInit {
    public listTypeEnum = ListTypeEnum;
    @ViewChild('usersWrapper') usersWrapper: any;
    @ViewChild('statusTimeline') statusTimeline: StatusTimelineComponent;
    @ViewChild('userList') userListComponent: AccountListComponent;

    search: string;
    searchingText: string = "Searching for ";

    constructor(private route: ActivatedRoute, private router: Router, private localStorage: Storage) { }

    ngOnInit(): void {
        this.route
            .queryParams
            .subscribe(params => {
                this.search = params['searchString'] || "John";
                if (this.search.indexOf("#") >= 0) {
                    this.search = this.search.replace("#", "");
                }
                this.statusTimeline.search = this.search;
                this.userListComponent.search = this.search;
            });
    }

    updateSearchingText(event: any) {
        if (event) {
            this.searchingText = "Results for";
        }
    }
}