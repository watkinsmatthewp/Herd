import { Component, OnInit } from '@angular/core';
import { NavigationEnd, Router } from "@angular/router";
import { Title } from "@angular/platform-browser";

@Component({
    selector: 'settings',
    templateUrl: './settings.page.html',
    styleUrls: ['./settings.page.css']
})
export class SettingsPage implements OnInit {

    constructor(private router: Router, private titleService: Title) {
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

    }
}