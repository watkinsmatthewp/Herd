import { Component } from '@angular/core';
import { AuthenticationService } from '../services/authentication.service';

@Component({
    selector: 'nav-menu',
    templateUrl: './navmenu.component.html',
    styleUrls: ['./navmenu.component.css']
})
export class NavMenuComponent {

    constructor(private authService: AuthenticationService) { }
}
