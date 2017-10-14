import { TestBed, async, inject } from '@angular/core/testing';
import { HttpModule, Http, Response, ResponseOptions, XHRBackend } from '@angular/http';
import { MockBackend, MockConnection } from '@angular/http/testing';
import { Observable } from "rxjs/Observable";

import { MastodonService } from './mastodon.service';
import { HttpClientService } from '../http-client-service/http-client.service';
import { Status } from '../../models/mastodon';

describe('Service: Mastodon Service', () => {
    let mastodonService: MastodonService;

    beforeEach(() => {
        // Set up the test bed
        TestBed.configureTestingModule({
            imports: [HttpModule],
            providers: [
                MastodonService,
                { provide: XHRBackend, useClass: MockBackend },
                HttpClientService,
            ]
        });
    });

    // Inject commonly used services
    beforeEach(inject([MastodonService], (ms: MastodonService) => {
        mastodonService = ms;
    }));

    describe('Get Home Timeline', () => {
        it('should return an array of statuses if following people who have posted',
            inject([XHRBackend], (mockBackend: MockBackend) => {
                // Create a mockedResponse
                const mockResponse = {
                    Data: {
                        RecentFeedItems: [
                            { Content: 'Content1', Id: 1, Url: 'example.com/1' },
                            { Content: 'Content2', Id: 2, Url: 'example.com/2' },
                            { Content: 'Content3', Id: 3, Url: 'example.com/3' },
                        ]  
                    }
                };

                // If there is an HTTP request intercept it and return the above mockedResponse
                mockBackend.connections.subscribe((connection: MockConnection) => {
                    connection.mockRespond(new Response(new ResponseOptions({
                        body: JSON.stringify(mockResponse)
                    })));
                });

                // Make the login request from our authentication service
                mastodonService.getHomeFeed().subscribe((response) => {
                    expect(response[0].Content).toBe("Content1");
                    expect(response[0].Id).toBe(1);
                    expect(response[1].Content).toBe("Content2");
                    expect(response[1].Id).toBe(2);
                    expect(response[2].Content).toBe("Content3");
                    expect(response[2].Id).toBe(3);
                });
            })
        );

        it('should return an empty array of statuses if following no one',
            inject([XHRBackend], (mockBackend: MockBackend) => {
                // Create a mockedResponse
                const mockResponse = {
                    Data: {
                        RecentFeedItems: []
                    }
                };

                // If there is an HTTP request intercept it and return the above mockedResponse
                mockBackend.connections.subscribe((connection: MockConnection) => {
                    connection.mockRespond(new Response(new ResponseOptions({
                        body: JSON.stringify(mockResponse)
                    })));
                });

                // Make the login request from our authentication service
                mastodonService.getHomeFeed().subscribe((response) => {
                    expect(response.length).toBe(0);
                });
            })
        );
    });

});