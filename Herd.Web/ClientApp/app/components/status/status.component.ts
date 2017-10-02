import { Component, Input } from '@angular/core';

import { MastodonService } from "../../services";
import { Status } from '../../models/mastodon';

@Component({
    selector: 'status',
    templateUrl: './status.component.html',
    styleUrls: ['./status.component.css']
})
export class StatusComponent {
    @Input() status: Status;

    constructor(private mastodonService: MastodonService) {
    }

    makeNewPostInFeed() {
        this.mastodonService.makeNewPost();
    }
}
