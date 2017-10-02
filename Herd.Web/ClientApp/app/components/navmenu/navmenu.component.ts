import { Component } from '@angular/core';

import { AuthenticationService } from '../../services';

@Component({
    selector: 'nav-menu',
    templateUrl: './navmenu.component.html',
    styleUrls: ['./navmenu.component.css']
})
export class NavMenuComponent {
    model: any = {};

    constructor(private authService: AuthenticationService) { }

    isAuthenticated(): boolean {
        return this.authService.isAuthenticated();
    }

    isConnectedToMastodon(): boolean {
        return this.authService.checkIfConnectedToMastodon();
    }

    search() {
        alert(this.model.searchItem);
    }
}
