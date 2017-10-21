import { TestBed, async, inject } from '@angular/core/testing';
import { TimelineAlertService } from './timeline-alert.service';

describe('Service: Timeline Alert Service', () => {
    let timelineAlertService: TimelineAlertService;

    beforeEach(() => {
        // Set up the test bed
        TestBed.configureTestingModule({
            imports: [],
            providers: [
                TimelineAlertService,
            ]
        });
    });

    // Inject commonly used services
    beforeEach(inject([TimelineAlertService], (tas: TimelineAlertService) => {
        timelineAlertService = tas;
    }));

    describe('Get Home Timeline', () => {
        it('should return an array of statuses if following people who have posted',
            inject([], () => {
                timelineAlertService.getMessage().subscribe(alert => {
                    expect(alert.message).toBe("mssg1");
                    expect(alert.statusId).toBe("1");
                });

                timelineAlertService.addMessage("mssg1", "1");
                
            })
        );

    });

});