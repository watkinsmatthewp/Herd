import { Component, OnInit } from '@angular/core';

import { MastodonService } from '../../services';

@Component({
    selector: 'searchresults',
    templateUrl: './searchresults.page.html'
})
export class SearchResultsPage implements OnInit {

    constructor(private mastodonService: MastodonService) { }

    ngOnInit(): void {

    }
}