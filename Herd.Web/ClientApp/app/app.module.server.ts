import { NgModule } from '@angular/core';
import { ServerModule } from '@angular/platform-server';
import { AppModuleShared } from './app.module.shared';
import { AppPage } from './pages/app/app.page';
import { StorageService, ServerStorage } from "./models/Storage";

@NgModule({
    bootstrap: [ AppPage ],
    imports: [
        ServerModule,
        AppModuleShared
    ],
    providers: [
        { provide: StorageService, useClass: ServerStorage }
    ]
})
export class AppModule {
}
