//import { assert } from 'chai';
//import { InstancePickerComponent } from './instance-picker.component';
//import { TestBed, async, ComponentFixture } from '@angular/core/testing';

//describe('Component: InstancePickerComponent', () => {
//    let fixture: ComponentFixture<InstancePickerComponent>;
//    let component: InstancePickerComponent;
//    beforeEach(() => {
//        TestBed.configureTestingModule({ declarations: [InstancePickerComponent] });
//        fixture = TestBed.createComponent(InstancePickerComponent);
//        fixture.detectChanges();
//        component = fixture.componentInstance;
//    });

//    it('should have a defined component', () => {
//        expect(component).toBeDefined();
//    })

//    it('should display a title', async(() => {
//        const titleText = fixture.nativeElement.querySelector('h3').textContent;
//        expect(titleText).toEqual('Choose a Mastodon instance');
//    }));

//    it('should start with count 0, then increments by 1 when clicked', async(() => {
//        const countElement = fixture.nativeElement.querySelector('strong');
//        expect(countElement.textContent).toEqual('0');

//        const incrementButton = fixture.nativeElement.querySelector('button');
//        incrementButton.click();
//        fixture.detectChanges();
//        expect(countElement.textContent).toEqual('1');
//    }));

//    it('should show the second form when OAuthUrl is set', async(() => {
//        const buttonElement = fixture.nativeElement.querySelector('button');
//        const inputElement = fixture.nativeElement.querySelector('input');
//        inputElement.
//        buttonElement.click();

//        const countElement = fixture.nativeElement.querySelector('strong');
//        expect(countElement.textContent).toEqual('0');

//        const incrementButton = fixture.nativeElement.querySelector('button');
//        incrementButton.click();
//        fixture.detectChanges();
//        expect(countElement.textContent).toEqual('1');
//    }));
//});