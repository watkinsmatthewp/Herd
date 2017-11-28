import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { Title } from "@angular/platform-browser";

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

    constructor(private route: ActivatedRoute, private router: Router, private titleService: Title, private localStorage: Storage) {
        router.events.subscribe(event => {
            if (event instanceof NavigationEnd) {
                var title = this.getTitle(router.routerState, router.routerState.root).join('-');
                titleService.setTitle(title);
            }
        });
    }

    private getTitle(state: any, parent: any): any {
        var data = [];
        if (parent && parent.snapshot.data && parent.snapshot.data.title) {
            data.push(parent.snapshot.data.title);
        }

        if (state && parent) {
            data.push(... this.getTitle(state, state.firstChild(parent)));
        }
        return data;
    }

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