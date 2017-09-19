import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';
// Components
import { AppComponent } from './components/app/app.component';
import { ErrorPageComponent } from './components/shared/errorpage/errorpage.component';
import { HomeComponent } from './components/home/home.component';
import { LocalFeedComponent } from './components/localfeed/localfeed.component';
import { LoginComponent } from './components/login/login.component';
import { NavMenuComponent } from './components/shared/navmenu/navmenu.component';
import { NotificationsComponent } from './components/notifications/notifications.component';
import { ProfileComponent } from './components/profile/profile.component';
// Services
import { MastodonService } from './components/shared/services/mastodon.service';
// Pipes
import { SafePipe } from './components/shared/pipes/safe.pipe';

@NgModule({
    declarations: [
        // Components
        AppComponent,
        ErrorPageComponent,
        HomeComponent,
        LocalFeedComponent,
        LoginComponent,
        NavMenuComponent,
        NotificationsComponent,
        ProfileComponent,
        // Pipes
        SafePipe
    ],
    imports: [
        CommonModule,
        HttpModule,
        FormsModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'login', pathMatch: 'full' },
            { path: 'login', component: LoginComponent },
            { path: 'home', component: HomeComponent },
            { path: 'localfeed', component: LocalFeedComponent },
            { path: 'notifications', component: NotificationsComponent },
            { path: 'profile', component: ProfileComponent },
            { path: 'errorpage', component: ErrorPageComponent },
            { path: '**', redirectTo: 'home' }
        ])
    ],
    providers: [MastodonService]
})
export class AppModuleShared {
}
