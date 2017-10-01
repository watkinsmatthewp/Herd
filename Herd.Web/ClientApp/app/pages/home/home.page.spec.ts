//import { TestBed, async, ComponentFixture } from '@angular/core/testing';
//import { HomeComponent } from './home.component';
//import { MastodonService } from "../shared/services/mastodon.service";

//class MockMastodonService {
//    public randomInt: number = 3;

//    getRandomNumer() {
//        return Promise.resolve(this.randomInt);
//    }
//}


//describe('Component: HomeComponent', () => {
//    let fixture: ComponentFixture<HomeComponent>;
//    let component: HomeComponent;
//    let spy;

//    beforeEach(async(() => {
//        TestBed.configureTestingModule({
//            declarations: [HomeComponent],
//            providers: [{ provide: MastodonService, useClass: MockMastodonService }]
//        });


//        const fixture = TestBed.createComponent(HomeComponent);
//        //fixture.detectChanges(); // Tells angular to perform changes (data rendering etc)
//        component = fixture.componentInstance;

//        // Service actually injected into the component
//        //mastodonService = fixture.debugElement.injector.get(MastodonService);
//        //spy = spyOn(mastodonService, "getRandomNumber").and.returnValue(Promise.resolve(3))
//    }));

//    it("should have be defined", () => {
//        expect(component).toBeDefined();
//    });

//    it("should get a random number on initalization", () => {
//        component.ngOnInit();
//        expect(component.randomInt).toBe(3);
//    });

//});