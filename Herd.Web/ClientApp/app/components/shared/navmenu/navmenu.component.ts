import { Component } from '@angular/core';
import { AuthenticationService } from '../services/authentication.service';

@Component({
    selector: 'nav-menu',
    templateUrl: './navmenu.component.html',
    styleUrls: ['./navmenu.component.css']
})
export class NavMenuComponent {
    model: any = {};


    constructor(private authService: AuthenticationService) { }

    search() {
        alert(this.model.searchItem);
    }
}
