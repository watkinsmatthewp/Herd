import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from '../services/authentication.service';

@Component({
    selector: 'nav-menu',
    templateUrl: './navmenu.component.html',
    styleUrls: ['./navmenu.component.css']
})
export class NavMenuComponent implements OnInit {
    authenticated: boolean = false;

    constructor(private authService: AuthenticationService) { }

    ngOnInit(): void {
        this.authenticated = this.authService.isAuthenticated();
    }
}
