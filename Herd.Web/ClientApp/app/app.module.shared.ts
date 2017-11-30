import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { Routes, RouterModule, CanActivate } from '@angular/router';
import { CardModule } from './components/card'

// External Dependencies
import { Angular2FontawesomeModule } from 'angular2-fontawesome/angular2-fontawesome';
import { BsModalModule } from 'ng2-bs3-modal';
import { ClipboardModule } from 'ngx-clipboard';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';
import { ModalGalleryModule } from 'angular-modal-gallery';
import { TabsModule, BsDropdownModule, ModalModule } from 'ngx-bootstrap';
import { SimpleNotificationsModule } from 'angular2-notifications';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { PerfectScrollbarConfigInterface } from 'ngx-perfect-scrollbar';

const PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
    suppressScrollX: true,
    wheelPropagation: true,
    minScrollbarLength: 50,
};


// Pages
import {
    AdminPage, AppPage, HomePage, LocalFeedPage,
    LoginPage, NotificationsPage, ProfilePage, RegisterPage,
    SearchResultsPage, SettingsPage, StatusViewPage
} from './pages';

// Components
import {
    AccountListComponent, InstancePickerComponent, NavMenuComponent, NotificationComponent,
    StatusComponent, StatusFormComponent, StatusFormModalComponent, StatusTimelineComponent,
    TopHashtagsComponent, UserCardComponent, ProfileUpdaterComponent, NotificationListComponent
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
        AccountListComponent, AdminPage, AppPage, HomePage, LocalFeedPage,
        LoginPage, NotificationsPage, ProfilePage, RegisterPage, SearchResultsPage, SettingsPage, StatusViewPage,
        // Components
        InstancePickerComponent, NavMenuComponent, NotificationComponent,
        StatusComponent, StatusFormComponent, StatusFormModalComponent, StatusTimelineComponent,
        TopHashtagsComponent, UserCardComponent, ProfileUpdaterComponent, NotificationListComponent,
        // Pipes
        SafePipe
    ],
    imports: [
        Angular2FontawesomeModule, BsModalModule, BsDropdownModule.forRoot(), ClipboardModule, CardModule, CommonModule, 
        HttpModule, FormsModule, InfiniteScrollModule, ModalGalleryModule.forRoot(), ModalModule.forRoot(),
        PerfectScrollbarModule.forRoot(PERFECT_SCROLLBAR_CONFIG),
        TabsModule.forRoot(), SimpleNotificationsModule.forRoot(),
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            // Admin Page 
            { path: 'admin', component: AdminPage, data: { title: "Herd - Admin" } },
            // Login, Register
            { path: 'login', component: LoginPage, data: { title: "Herd - Login" } },
            { path: 'register', component: RegisterPage, data: { title: "Herd - Registration" } },
            // Account Settings
            { path: 'instance-picker', component: InstancePickerComponent, canActivate: [AuthGuard], data: { title: "Instance Picker" } },
            // TimeLines
            { path: 'home', component: HomePage, canActivate: [AuthGuard], data: { title: "Herd - Home" } },
            { path: 'localfeed', component: LocalFeedPage, canActivate: [AuthGuard], data: { title: "Herd - Public Feed" } },
            // Notification
            { path: 'notifications', component: NotificationsPage, canActivate: [AuthGuard], data: { title: "Herd - Notifications" } },
            // Profile
            { path: 'profile/:id', component: ProfilePage, canActivate: [AuthGuard], data: { title: "Herd - Profile" } },
            // Search Results (am I doing this right?)
            { path: 'searchresults', component: SearchResultsPage, canActivate: [AuthGuard], data: { title: "Herd - Search Results" } },
            // Settings Page
            { path: 'settings', component: SettingsPage, canActivate: [AuthGuard], data: { title: "Herd - Settings" } },
            // Status View Page
            { path: 'status/:id', component: StatusViewPage, canActivate: [AuthGuard], data: { title: "Status View" } },
            // Etc Pages
            { path: '**', redirectTo: 'home' },

        ])
    ],
    providers: [AccountService, AdminAuthGuard, AuthenticationService, AuthGuard, EventAlertService,
        HttpClientService, StatusService]
})
export class AppModuleShared {}
