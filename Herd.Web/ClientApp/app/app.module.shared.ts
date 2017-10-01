import { Angular2FontawesomeModule } from 'angular2-fontawesome/angular2-fontawesome';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { Routes, RouterModule, CanActivate } from '@angular/router';

// Pages
import { AdminPage } from './pages/admin/admin.page';
import { AppPage } from './pages/app/app.page';
import { HomePage } from './pages/home/home.page';
import { LocalFeedPage } from './pages/localfeed/localfeed.page';
import { LoginPage } from './pages/login/login.page';
import { NotificationsPage } from './pages/notifications/notifications.page';
import { ProfilePage } from './pages/profile/profile.page';
import { RegisterPage } from './pages/register/register.page';

// Components
import { AlertComponent } from './components/alert/alert.component';
import { InstancePickerComponent } from './components/instance-picker/instance-picker.component';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { StatusComponent } from './components/status/status.component';
import { StatusFormComponent } from './components/status-form/status-form.component';

// Services
import { AccountService } from './services/account.service';
import { AdminAuthGuard } from './services/admin-auth-guard.service';
import { AlertService } from './services/alert.service';
import { AuthenticationService } from './services/authentication.service';
import { AuthGuard } from './services/auth-guard.service';
import { HttpClientService } from './services/http-client.service';
import { MastodonService } from './services/mastodon.service';

// Pipes
import { SafePipe } from './pipes/safe.pipe';

@NgModule({
    declarations: [
        // Page
        AdminPage,
        AppPage,
        HomePage,
        LocalFeedPage,
        LoginPage,
        NotificationsPage,
        ProfilePage,
        RegisterPage,
        // Components
        AlertComponent,
        InstancePickerComponent,
        NavMenuComponent,
        StatusComponent,
        StatusFormComponent,
        // Pipes
        SafePipe
    ],
    imports: [
        Angular2FontawesomeModule,
        CommonModule,
        HttpModule,
        FormsModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            // Admin Page 
            { path: 'admin', component: AdminPage },
            // Login, Register
            { path: 'login', component: LoginPage },
            { path: 'register', component: RegisterPage },
            // Account Settings
            { path: 'instance-picker', component: InstancePickerComponent, canActivate: [AuthGuard] },
            // TimeLines
            { path: 'home', component: HomePage, canActivate: [AuthGuard] },
            { path: 'localfeed', component: LocalFeedPage, canActivate: [AuthGuard] },
            // Notification
            { path: 'notifications', component: NotificationsPage, canActivate: [AuthGuard] },
            // Profile
            { path: 'profile', component: ProfilePage, canActivate: [AuthGuard] },
            // Etc Pages
            { path: '**', redirectTo: 'home' }
        ])
    ],
    providers: [AccountService, AdminAuthGuard, AlertService, AuthenticationService, AuthGuard, HttpClientService, MastodonService]
})
export class AppModuleShared {
}
