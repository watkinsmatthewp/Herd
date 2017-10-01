import { NgModule } from '@angular/core';
import { ServerModule } from '@angular/platform-server';
import { AppModuleShared } from './app.module.shared';

import { AppPage } from './pages/app/app.page';
import { Storage, ServerStorage } from './models';

@NgModule({
    bootstrap: [ AppPage ],
    imports: [
        ServerModule,
        AppModuleShared
    ],
    providers: [
        { provide: Storage, useClass: ServerStorage }
    ]
})
export class AppModule {
}
