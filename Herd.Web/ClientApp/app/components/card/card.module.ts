import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { FormsModule } from '@angular/forms';

import { CardActionsComponent } from './card-actions.component';
import { CardContentComponent } from './card-content.component';
import { CardTitleComponent } from './card-title.component';
import { CardComponent } from './card.component';
import { CardImageHeaderComponent } from './card-image-header.component';

@NgModule({
    declarations: [
        CardActionsComponent,
        CardComponent,
        CardContentComponent,
        CardTitleComponent,
        CardImageHeaderComponent,
    ],
    imports: [
        CommonModule,
        FormsModule,
    ],
    exports: [
        CardActionsComponent,
        CardComponent,
        CardContentComponent,
        CardTitleComponent,
        CardImageHeaderComponent,
    ]
})
export class CardModule { }