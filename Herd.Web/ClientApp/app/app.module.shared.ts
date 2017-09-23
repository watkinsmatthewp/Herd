import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { Routes, RouterModule, CanActivate } from '@angular/router';

// Components
import { AlertComponent } from './components/shared/alert/alert.component';
import { AppComponent } from './components/app/app.component';
import { HomeComponent } from './components/home/home.component';
import { InstancePickerComponent } from './components/instance-picker/instance-picker.component';
import { LocalFeedComponent } from './components/localfeed/localfeed.component';
import { LoginComponent } from './components/login/login.component';
import { NavMenuComponent } from './components/shared/navmenu/navmenu.component';
import { NotificationsComponent } from './components/notifications/notifications.component';
import { ProfileComponent } from './components/profile/profile.component';
import { RegisterComponent } from './components/register/register.component';
import { PostComponent } from './components/post/post.component';
// Services
import { AccountService } from './components/shared/services/account.service';
import { AlertService } from './components/shared/services/alert.service';
import { AuthenticationService } from './components/shared/services/authentication.service';
import { AuthGuard } from './components/shared/services/auth-guard.service';
import { HttpClientService } from './components/shared/services/http-client.service';
import { MastodonService } from './components/shared/services/mastodon.service';

// Pipes
import { SafePipe } from './components/shared/pipes/safe.pipe';

@NgModule({
    declarations: [
        // Components
        AlertComponent,
        AppComponent,
        HomeComponent,
        InstancePickerComponent,
        LocalFeedComponent,
        LoginComponent,
        NavMenuComponent,
        NotificationsComponent,
        ProfileComponent,
        RegisterComponent,
        PostComponent,
        // Pipes
        SafePipe
    ],
    imports: [
        CommonModule,
        HttpModule,
        FormsModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'login', pathMatch: 'full' },
            // Login, Register
            { path: 'login', component: LoginComponent },
            { path: 'register', component: RegisterComponent },
            // Account Settings
            { path: 'instance-picker', component: InstancePickerComponent, canActivate: [AuthGuard] },
            // TimeLines
            { path: 'home', component: HomeComponent, canActivate: [AuthGuard] },
            { path: 'localfeed', component: LocalFeedComponent, canActivate: [AuthGuard] },
            // Notification
            { path: 'notifications', component: NotificationsComponent, canActivate: [AuthGuard] },
            // Profile
            { path: 'profile', component: ProfileComponent, canActivate: [AuthGuard] },
            // Etc Pages
            { path: '**', redirectTo: 'login' }
        ])
    ],
    providers: [AccountService, AlertService, AuthenticationService, AuthGuard, HttpClientService, MastodonService]
})
export class AppModuleShared {
}
