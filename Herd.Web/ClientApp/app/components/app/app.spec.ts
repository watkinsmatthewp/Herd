import { } from 'jasmine';
import { TestBed, async, ComponentFixture } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { AppComponent } from './app.component';
import { NavMenuComponent } from '../shared/navmenu/navmenu.component';
import { AlertComponent } from '../shared/alert/alert.component';
import { AuthenticationService } from '../shared/services/authentication.service';
import { AlertService } from '../shared/services/alert.service';

describe('Component: AppComponent', () => {
    let fixture: ComponentFixture<AppComponent>;
    let component: AppComponent;
    let spy = null;
    let authServiceStub = {};
    let alertServiceStub = { };

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            imports: [RouterTestingModule],
            declarations: [
                AppComponent,
                NavMenuComponent,
                AlertComponent,
            ],
            providers: [
                { provide: AuthenticationService, useValue: authServiceStub },
                { provide: AlertService, useValue: alertServiceStub },
            ]
        }).compileComponents();

        fixture = TestBed.createComponent(AppComponent);
        fixture.detectChanges();
        component = fixture.componentInstance;
    }));

});