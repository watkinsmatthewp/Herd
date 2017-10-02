import { TestBed, async, inject } from '@angular/core/testing';
import { HttpModule, Http, Response, ResponseOptions, XHRBackend } from '@angular/http';
import { MockBackend, MockConnection } from '@angular/http/testing';
import { Observable } from "rxjs/Observable";

import { MastodonService } from './mastodon.service';
import { HttpClientService } from './http-client.service';
import { Status } from '../models/mastodon';

describe('Service: Mastodon Service', () => {

    beforeEach(() => {
        TestBed.configureTestingModule({
            imports: [HttpModule],
            providers: [
                MastodonService,
                { provide: XHRBackend, useClass: MockBackend },
                HttpClientService,
            ]
        });
    });

    describe('Login Process', () => {
        it('should return a User on successful login',
            inject([MastodonService, XHRBackend], (mastodonService: MastodonService, mockBackend: MockBackend) => {
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
                    expect(response[0].Url).toBe('example.com/1');
                    expect(response[1].Content).toBe("Content2");
                    expect(response[1].Id).toBe(2);
                    expect(response[1].Url).toBe('example.com/2');
                    expect(response[2].Content).toBe("Content3");
                    expect(response[2].Id).toBe(3);
                    expect(response[2].Url).toBe('example.com/3');
                });
            })
        );
    });

});