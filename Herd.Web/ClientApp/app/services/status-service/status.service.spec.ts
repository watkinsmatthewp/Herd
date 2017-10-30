import { TestBed, async, inject } from '@angular/core/testing';
import { HttpModule, Http, Response, ResponseOptions, XHRBackend } from '@angular/http';
import { MockBackend, MockConnection } from '@angular/http/testing';
import { Observable } from "rxjs/Observable";

import { StatusService } from './status.service';
import { HttpClientService } from '../http-client-service/http-client.service';
import { Status, Visibility } from '../../models/mastodon';

describe('Service: Status Service', () => {
    let statusService: StatusService;

    beforeEach(() => {
        // Set up the test bed
        TestBed.configureTestingModule({
            imports: [HttpModule],
            providers: [
                StatusService,
                { provide: XHRBackend, useClass: MockBackend },
                HttpClientService,
            ]
        });
    });

    // Inject commonly used services
    beforeEach(inject([StatusService], (ms: StatusService) => {
        statusService = ms;
    }));

    describe('Status Search Parameter Setting', () => {
        it('should set all parameters successfully',
            inject([XHRBackend], (mockBackend: MockBackend) => {
                // Create a mockedResponse
                const mockResponse = {
                    Data: {
                        Posts: [
                            { Content: 'Content1', Id: "1", Url: 'example.com/1' },
                            { Content: 'Content2', Id: "2", Url: 'example.com/2' },
                            { Content: 'Content3', Id: "3", Url: 'example.com/3' },
                        ]
                    }
                };

                // If there is an HTTP request intercept it and return the above mockedResponse
                mockBackend.connections.subscribe((connection: MockConnection) => {
                    connection.mockRespond(new Response(new ResponseOptions({
                        body: JSON.stringify(mockResponse)
                    })));
                });

                // Make the request
                let params = {
                    onlyOnActiveUserTimeline: true,
                    authorMastodonUserID: "1",
                    postID: "1",
                    hashtag: "#hello",
                    includeAncestors: true,
                    includeDescendants: true,
                    max: 30,
                    maxID: "5000",
                    sinceID: "4000"
                }
                statusService.search(params).subscribe((response) => {
                    expect(response[0].Content).toBe("Content1");
                    expect(response[0].Id).toBe("1");
                    expect(response[1].Content).toBe("Content2");
                    expect(response[1].Id).toBe("2");
                    expect(response[2].Content).toBe("Content3");
                    expect(response[2].Id).toBe("3");
                });
            })
        );
    });

    describe('Get Home Timeline', () => {
        it('should return an array of statuses if following people who have posted',
            inject([XHRBackend], (mockBackend: MockBackend) => {
                // Create a mockedResponse
                const mockResponse = {
                    Data: {
                        Posts: [
                            { Content: 'Content1', Id: "1", Url: 'example.com/1' },
                            { Content: 'Content2', Id: "2", Url: 'example.com/2' },
                            { Content: 'Content3', Id: "3", Url: 'example.com/3' },
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
                statusService.search({ onlyOnActiveUserTimeline: true }).subscribe((response) => {
                    expect(response[0].Content).toBe("Content1");
                    expect(response[0].Id).toBe("1");
                    expect(response[1].Content).toBe("Content2");
                    expect(response[1].Id).toBe("2");
                    expect(response[2].Content).toBe("Content3");
                    expect(response[2].Id).toBe("3");
                });
            })
        );

        it('should return an empty array of statuses if following no one',
            inject([XHRBackend], (mockBackend: MockBackend) => {
                // Create a mockedResponse
                const mockResponse = {
                    Data: {
                        Posts: []
                    }
                };

                // If there is an HTTP request intercept it and return the above mockedResponse
                mockBackend.connections.subscribe((connection: MockConnection) => {
                    connection.mockRespond(new Response(new ResponseOptions({
                        body: JSON.stringify(mockResponse)
                    })));
                });

                // Make the login request from our authentication service
                statusService.search({ onlyOnActiveUserTimeline: true }).subscribe((response) => {
                    expect(response.length).toBe(0);
                });
            })
        );
    });

    describe('Get User Timeline', () => {
        it('should return an array of statuses',
            inject([XHRBackend], (mockBackend: MockBackend) => {
                // Create a mockedResponse
                const mockResponse = {
                    Data: {
                        Posts: [
                            { Content: 'Content1', Id: "1", Url: 'example.com/1' },
                            { Content: 'Content2', Id: "2", Url: 'example.com/2' },
                            { Content: 'Content3', Id: "3", Url: 'example.com/3' },
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
                statusService.search({}).subscribe((response) => {
                    expect(response[0].Content).toBe("Content1");
                    expect(response[0].Id).toBe("1");
                    expect(response[1].Content).toBe("Content2");
                    expect(response[1].Id).toBe("2");
                    expect(response[2].Content).toBe("Content3");
                    expect(response[2].Id).toBe("3");
                });
            })
        );

        it('should return an empty array of statuses if none have been made by this user',
            inject([XHRBackend], (mockBackend: MockBackend) => {
                // Create a mockedResponse
                const mockResponse = {
                    Data: {
                        Posts: []
                    }
                };

                // If there is an HTTP request intercept it and return the above mockedResponse
                mockBackend.connections.subscribe((connection: MockConnection) => {
                    connection.mockRespond(new Response(new ResponseOptions({
                        body: JSON.stringify(mockResponse)
                    })));
                });

                // Make the login request from our authentication service
                statusService.search({}).subscribe((response) => {
                    expect(response.length).toBe(0);
                });
            })
        );
    });

    describe('Status', () => {
        it('should get a status with no context successfully',
            inject([XHRBackend], (mockBackend: MockBackend) => {
                // Create a mockedResponse
                const mockResponse = {
                    Success: true,
                    Data: {
                        Posts: [
                            { Content: 'Content1', Id: "1", Url: 'example.com/1' },
                        ]
                    }
                };

                // If there is an HTTP request intercept it and return the above mockedResponse
                mockBackend.connections.subscribe((connection: MockConnection) => {
                    connection.mockRespond(new Response(new ResponseOptions({
                        body: JSON.stringify(mockResponse)
                    })));
                });

                // Make the request
                statusService.search({ postID: "1", includeAncestors: false, includeDescendants: false })
                    .map(posts => posts[0] as Status)
                    .subscribe((status) => {
                        expect(status).toBeDefined();
                        expect(status.Id).toBe("1");
                }, error => {
                    fail();
                });
            })
        );

        it('should get a status with context successfully',
            inject([XHRBackend], (mockBackend: MockBackend) => {
                // Create a mockedResponse
                const mockResponse = {
                    Success: true,
                    Data: {
                        Posts: [
                            {
                                Content: 'Content1', Id: "1", Url: 'example.com/1',
                                Ancestors: [{ Content: 'Ancestor1', Id: "2", Url: 'example.com/2'}],
                                Descendants: [{ Content: 'Descendant1', Id: "3", Url: 'example.com/3' }],
                            },
                        ]
                    }
                };

                // If there is an HTTP request intercept it and return the above mockedResponse
                mockBackend.connections.subscribe((connection: MockConnection) => {
                    connection.mockRespond(new Response(new ResponseOptions({
                        body: JSON.stringify(mockResponse)
                    })));
                });

                // Make the request
                statusService.search({ postID: "1", includeAncestors: true, includeDescendants: true })
                    .map(posts => posts[0] as Status)
                    .subscribe((status) => {
                        expect(status).toBeDefined();
                        expect(status.Id).toBe("1");
                        expect(status.Ancestors.length).toBe(1);
                        expect(status.Ancestors[0].Content).toBe("Ancestor1");
                        expect(status.Descendants.length).toBe(1);
                        expect(status.Descendants[0].Content).toBe("Descendant1");
                }, error => {
                    fail();
                });
            })
        );

        it('should make a new status successfully',
            inject([XHRBackend], (mockBackend: MockBackend) => {
                // Create a mockedResponse
                const mockResponse = {
                    Success: true,
                    Data: {}
                };

                // If there is an HTTP request intercept it and return the above mockedResponse
                mockBackend.connections.subscribe((connection: MockConnection) => {
                    connection.mockRespond(new Response(new ResponseOptions({
                        body: JSON.stringify(mockResponse)
                    })));
                });

                // Make the request
                let message = "hello world";
                let visibility = Visibility.PUBLIC;
                statusService.makeNewStatus(message, visibility).subscribe((response) => {
                }, error => {
                    fail();
                });
            })
        );

        it('should like a status',
            inject([XHRBackend], (mockBackend: MockBackend) => {
                // Create a mockedResponse
                const mockResponse = {
                    Success: true,
                    Data: {}
                };

                // If there is an HTTP request intercept it and return the above mockedResponse
                mockBackend.connections.subscribe((connection: MockConnection) => {
                    connection.mockRespond(new Response(new ResponseOptions({
                        body: JSON.stringify(mockResponse)
                    })));
                });

                // Make the request
                statusService.like("1", true).subscribe((response) => {
                }, error => {
                    fail();
                });
            })
        );

        it('should fail at liking a status',
            inject([XHRBackend], (mockBackend: MockBackend) => {
                // Create a mockedResponse
                const mockResponse = {
                    Success: false,
                    Data: {}
                };

                // If there is an HTTP request intercept it and return the above mockedResponse
                mockBackend.connections.subscribe((connection: MockConnection) => {
                    connection.mockRespond(new Response(new ResponseOptions({
                        body: JSON.stringify(mockResponse)
                    })));
                });

                // Make the request
                statusService.like("1", true).subscribe((response) => {
                    fail();
                }, error => {
                    
                });
            })
        );

        it('should repost a status',
            inject([XHRBackend], (mockBackend: MockBackend) => {
                // Create a mockedResponse
                const mockResponse = {
                    Success: true,
                    Data: {}
                };

                // If there is an HTTP request intercept it and return the above mockedResponse
                mockBackend.connections.subscribe((connection: MockConnection) => {
                    connection.mockRespond(new Response(new ResponseOptions({
                        body: JSON.stringify(mockResponse)
                    })));
                });

                // Make the request
                statusService.repost("1", true).subscribe((response) => {
                }, error => {
                    fail();
                });
            })
        );

        it('should fail at reposting a status',
            inject([XHRBackend], (mockBackend: MockBackend) => {
                // Create a mockedResponse
                const mockResponse = {
                    Success: false,
                    Data: {}
                };

                // If there is an HTTP request intercept it and return the above mockedResponse
                mockBackend.connections.subscribe((connection: MockConnection) => {
                    connection.mockRespond(new Response(new ResponseOptions({
                        body: JSON.stringify(mockResponse)
                    })));
                });

                // Make the request
                statusService.repost("1", true).subscribe((response) => {
                    fail();
                }, error => {

                });
            })
        );

    });

});