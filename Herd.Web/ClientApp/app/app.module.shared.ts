import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { Routes, RouterModule, CanActivate } from '@angular/router';
import { CardModule } from './components/card'

// External Dependencies
import { Angular2FontawesomeModule } from 'angular2-fontawesome/angular2-fontawesome';
import { BsModalModule } from 'ng2-bs3-modal';
import { TabsModule } from 'ngx-bootstrap';
import { SimpleNotificationsModule } from 'angular2-notifications';

// Pages
import {
    AdminPage, AppPage, HomePage, LocalFeedPage,
    LoginPage, NotificationsPage, ProfilePage, RegisterPage, SearchResultsPage
} from './pages';

// Components
import {
    InstancePickerComponent, NavMenuComponent,
    StatusComponent, StatusFormComponent, StatusFormModalComponent,
    StatusModalComponent, UserCardComponent
} from './components';

// Guards
import { AdminAuthGuard, AuthGuard } from './guards'

// Services
import {
    AccountService, AuthenticationService, EventAlertService,
    HttpClientService, StatusService
} from './services';

// Pipes
import { SafePipe } from './pipes';

@NgModule({
    declarations: [
        // Page
        AdminPage, AppPage, HomePage, LocalFeedPage,
        LoginPage, NotificationsPage, ProfilePage, RegisterPage, SearchResultsPage,
        // Components
        InstancePickerComponent, NavMenuComponent,
        StatusComponent, StatusFormComponent, StatusFormModalComponent,
        StatusModalComponent, UserCardComponent,
        // Pipes
        SafePipe
    ],
    imports: [
        Angular2FontawesomeModule, BsModalModule, CardModule, CommonModule, 
        HttpModule, FormsModule, ModalGalleryModule.forRoot(), TabsModule.forRoot(),
        SimpleNotificationsModule.forRoot(),
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
            { path: 'profile/:id', component: ProfilePage, canActivate: [AuthGuard], data: { title: "Profile" } },
            // Search Results (am I doing this right?)
            { path: 'searchresults', component: SearchResultsPage, canActivate: [AuthGuard], data: { title: "Search Results" } },
            // Etc Pages
            { path: '**', redirectTo: 'home' }
        ])
    ],
    providers: [AccountService, AdminAuthGuard, AuthenticationService, AuthGuard, EventAlertService,
        HttpClientService, StatusService]
})
export class AppModuleShared {}
