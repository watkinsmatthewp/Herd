import { Component } from '@angular/core';

@Component({
    selector: 'app',
    templateUrl: './app.page.html',
    styleUrls: ['./app.page.css']
})
export class AppPage {
    public options = {
        position: ["bottom", "right"],
        timeOut: 5000,
        lastOnBottom: true,
        animate: "fromLeft"
    }
}
