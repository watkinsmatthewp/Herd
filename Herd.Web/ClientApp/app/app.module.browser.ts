import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppModuleShared } from './app.module.shared';

import { AppPage } from './pages/app/app.page';
import { Storage, BrowserStorage } from './models';
import 'hammerjs';
import 'mousetrap';

@NgModule({
    bootstrap: [ AppPage ],
    imports: [
        BrowserModule,
        BrowserAnimationsModule,
        AppModuleShared
    ],
    providers: [
        { provide: 'BASE_URL', useFactory: getBaseUrl },
        { provide: Storage, useClass: BrowserStorage }
    ]
})
export class AppModule {
}

export function getBaseUrl() {
    return document.getElementsByTagName('base')[0].href;
}
