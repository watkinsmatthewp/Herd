import { Angular2FontawesomeModule } from 'angular2-fontawesome/angular2-fontawesome';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { Routes, RouterModule, CanActivate } from '@angular/router';

// Pages
import {
    AdminPage, AppPage, HomePage, LocalFeedPage,
    LoginPage, NotificationsPage, ProfilePage, RegisterPage
} from './pages';

// Components
import {
    AlertComponent, InstancePickerComponent, NavMenuComponent,
    StatusComponent, StatusFormComponent
} from './components';

// Services
import {
    AccountService, AdminAuthGuard, AlertService, AuthenticationService,
    AuthGuard, HttpClientService, MastodonService
} from './services';

// Pipes
import { SafePipe } from './pipes';

@NgModule({
    declarations: [
        // Page
        AdminPage, AppPage, HomePage, LocalFeedPage,
        LoginPage, NotificationsPage, ProfilePage, RegisterPage,
        // Components
        AlertComponent, InstancePickerComponent, NavMenuComponent,
        StatusComponent, StatusFormComponent,
        // Pipes
        SafePipe
    ],
    imports: [
        Angular2FontawesomeModule, CommonModule,
        HttpModule, FormsModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            // Admin Page 
            { path: 'admin', component: AdminPage, data: { title: "Admin" } },
            // Login, Register
            { path: 'login', component: LoginPage, data: { title: "Login" } },
            { path: 'register', component: RegisterPage, data: { title: "Registration" } },
            // Account Settings
            { path: 'instance-picker', component: InstancePickerComponent, canActivate: [AuthGuard], data: { title: "Instance Picker" } },
            // TimeLines
            { path: 'home', component: HomePage, canActivate: [AuthGuard], data: { title: "Home" } },
            { path: 'localfeed', component: LocalFeedPage, canActivate: [AuthGuard], data: { title: "Local Feed" } },
            // Notification
            { path: 'notifications', component: NotificationsPage, canActivate: [AuthGuard], data: { title: "Notifications" } },
            // Profile
            { path: 'profile', component: ProfilePage, canActivate: [AuthGuard], data: { title: "Profile" } },
            // Etc Pages
            { path: '**', redirectTo: 'home' }
        ])
    ],
    providers: [AccountService, AdminAuthGuard, AlertService, AuthenticationService, AuthGuard, HttpClientService, MastodonService]
})
export class AppModuleShared {}
