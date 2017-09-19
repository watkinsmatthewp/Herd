import { Component, OnInit } from '@angular/core';
import { MastodonService } from '../services/mastodon.service';

@Component({
    selector: 'nav-menu',
    templateUrl: './navmenu.component.html',
    styleUrls: ['./navmenu.component.css']
})
export class NavMenuComponent implements OnInit {
    authenticated: boolean = false;

    constructor(private mastodonService: MastodonService) { }

    ngOnInit(): void {
        this.authenticated = this.mastodonService.isAuthenticated();
    }
}
