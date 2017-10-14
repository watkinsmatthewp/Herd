import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { Routes, RouterModule, CanActivate } from '@angular/router';

// External Dependencies
import { Angular2FontawesomeModule } from 'angular2-fontawesome/angular2-fontawesome';
import { BsModalModule } from 'ng2-bs3-modal';
import { SimpleNotificationsModule } from 'angular2-notifications';

// Pages
import {
    AdminPage, AppPage, HomePage, LocalFeedPage,
    LoginPage, NotificationsPage, ProfilePage, RegisterPage, SearchResultsPage
} from './pages';

// Components
import {
    AlertComponent, InstancePickerComponent, NavMenuComponent,
    StatusComponent, StatusFormComponent, StatusFormModalComponent,
    StatusModalComponent, StatusReplyModalComponent, UserCardComponent
} from './components';

// Guards
import { AdminAuthGuard, AuthGuard } from './guards'

// Services
import {
    AccountService, AlertService, AuthenticationService,
    HttpClientService, MastodonService, TimelineAlertService
} from './services';

// Pipes
import { SafePipe } from './pipes';

@NgModule({
    declarations: [
        // Page
        AdminPage, AppPage, HomePage, LocalFeedPage,
        LoginPage, NotificationsPage, ProfilePage, RegisterPage, SearchResultsPage,
        // Components
        AlertComponent, InstancePickerComponent, NavMenuComponent,
        StatusComponent, StatusFormComponent, StatusFormModalComponent,
        StatusModalComponent, StatusReplyModalComponent, UserCardComponent,
        // Pipes
        SafePipe
    ],
    imports: [
        Angular2FontawesomeModule, BsModalModule, CommonModule,
        HttpModule, FormsModule, SimpleNotificationsModule.forRoot(),
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
            { path: 'home/:id', component: HomePage, canActivate: [AuthGuard], data: { title: "Home" } },
            { path: 'localfeed', component: LocalFeedPage, canActivate: [AuthGuard], data: { title: "Local Feed" } },
            // Notification
            { path: 'notifications', component: NotificationsPage, canActivate: [AuthGuard], data: { title: "Notifications" } },
            // Profile
            { path: 'profile', component: ProfilePage, canActivate: [AuthGuard], data: { title: "Profile" } },
            // Search Results (am I doing this right?)
            { path: 'searchresults', component: SearchResultsPage, canActivate: [AuthGuard], data: { title: "Search Results" } },
            // Etc Pages
            { path: '**', redirectTo: 'home' }
        ])
    ],
    providers: [AccountService, AdminAuthGuard, AlertService, AuthenticationService, AuthGuard,
        HttpClientService, MastodonService, TimelineAlertService]
})
export class AppModuleShared {}
