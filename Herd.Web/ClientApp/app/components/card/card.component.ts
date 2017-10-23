import {
    Component, Input, AfterContentInit, ContentChildren, QueryList
} from '@angular/core';

import { CardTitleComponent } from './card-title.component';

@Component({
    selector: 'card',
    styleUrls: ['./card.component.css'],
    templateUrl: './card.component.html'
})
export class CardComponent implements AfterContentInit {
    @Input()
    public size: string;
    @Input()
    public fillWidth: boolean;

    @Input()
    public headerImgSrc: string;

    public headerNotMissing: boolean;

    @ContentChildren(CardTitleComponent)
    public titleComponent: QueryList<CardTitleComponent>;

    public showTitle: boolean = true;

    public ngAfterContentInit() {
        this.showTitle = this.titleComponent.length > 0;
        this.headerNotMissing = this.headerImgSrc !== "https://mastodon.social/headers/original/missing.png";
    }
}