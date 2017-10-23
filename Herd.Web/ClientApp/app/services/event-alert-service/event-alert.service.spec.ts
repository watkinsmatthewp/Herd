import { TestBed, async, inject } from '@angular/core/testing';
import { EventAlertService } from './event-alert.service';
import { EventAlertEnum } from '../../models';

describe('Service: Event Alert Service', () => {
    let eventAlertService: EventAlertService;

    beforeEach(() => {
        // Set up the test bed
        TestBed.configureTestingModule({
            imports: [],
            providers: [
                EventAlertService,
            ]
        });
    });

    // Inject commonly used services
    beforeEach(inject([EventAlertService], (eas: EventAlertService) => {
        eventAlertService = eas;
    }));

    describe('Add Event', () => {
        it('should add an event with an extra details',
            inject([], () => {
                eventAlertService.getMessage().subscribe(event => {
                    expect(event.eventType).toBe(EventAlertEnum.UPDATE_FOLLOWING);
                    expect(event.StatusID).toBe("1");
                });

                eventAlertService.addEvent(EventAlertEnum.UPDATE_FOLLOWING, { StatusID: "1" });
            })
        );

        it('should add an event without any details',
            inject([], () => {
                eventAlertService.getMessage().subscribe(event => {
                    expect(event.eventType).toBe(EventAlertEnum.UPDATE_FOLLOWING_AND_FOLLOWERS);
                });

                eventAlertService.addEvent(EventAlertEnum.UPDATE_FOLLOWING_AND_FOLLOWERS);
            })
        );

    });

});