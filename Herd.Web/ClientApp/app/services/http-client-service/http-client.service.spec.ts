import { TestBed, async, inject } from '@angular/core/testing';
import { HttpModule, Http, Headers, RequestOptions, Response, ResponseOptions, RequestOptionsArgs, XHRBackend } from '@angular/http';
import { MockBackend, MockConnection } from '@angular/http/testing';
import { Observable } from "rxjs/Observable";

import { HttpClientService } from './http-client.service';
import { Storage, BrowserStorage } from '../../models';

describe('Service: HttpClient Service', () => {
    let httpClient: HttpClientService;
    let mockBackend: MockBackend;
    let http: Http;

    beforeEach(() => {
        // Set up the test bed
        TestBed.configureTestingModule({
            imports: [HttpModule],
            providers: [
                HttpClientService,
                { provide: Storage, useClass: BrowserStorage },
                { provide: XHRBackend, useClass: MockBackend },
            ],
        });
    });

    // Inject commonly used services
    beforeEach(inject([HttpClientService, XHRBackend], (hcs: HttpClientService, mb: MockBackend) => {
        httpClient = hcs;
        mockBackend = mb;
    }));

    describe('Headers', () => {
        it('should start with default headers of Content-Type', inject([], () => {
            expect(httpClient.getHeaderByKey("Content-Type")).toBeDefined();
            expect(httpClient.getHeaderByKey("Content-Type")).toBe("application/json; charset=UTF-8");
        }));

        it('should be able to remove headers', inject([], () => {
            expect(httpClient.getHeaderByKey("Content-Type")).toBeDefined();
            expect(httpClient.getHeaderByKey("Content-Type")).toBe("application/json; charset=UTF-8");

            httpClient.removeHeader("Content-Type");

            expect(httpClient.getHeaderByKey("Content-Type")).toBeNull();
        }));
        
        it('should be able to set headers', inject([], () => {
            expect(httpClient.getHeaderByKey("Content-Type")).toBeDefined();
            expect(httpClient.getHeaderByKey("Content-Type")).toBe("application/json; charset=UTF-8");

            httpClient.setHeader("Content-Type", "text/plain");

            expect(httpClient.getHeaderByKey("Content-Type")).toBe("text/plain");
        }));
    });

    describe('Get', () => {
        it('should successfully make a basic GET request', inject([], () => {
            // Create a mockedResponse
            const mockResponse = {
                Success: true,
                Data: {
                    User: {
                        ID: "1",
                        firstName: 'Thomas',
                        lastName: 'Ortiz'
                    }
                },
                SystemErrors: [],
                UserErrors: [],
            };

            // If there is an HTTP request intercept it and return the above mockedResponse
            mockBackend.connections.subscribe((connection: MockConnection) => {
                connection.mockRespond(new Response(new ResponseOptions({
                    body: JSON.stringify(mockResponse)
                })));
            });

            // Make the request
            httpClient.get("api/getUser/1").subscribe((response) => {
                expect(response).toBeDefined();
                expect(response.Data).toBeUndefined();
                expect(response.User).toBeDefined();
                expect(response.User.ID).toBe("1");
                
            });
        }));

        it('should successfully make a GET request with options', inject([], () => {
            // Create a mockedResponse
            const mockResponse = {
                Success: true,
                Data: {
                    User: {
                        ID: "1",
                        firstName: 'Thomas',
                        lastName: 'Ortiz'
                    }
                },
                SystemErrors: [],
                UserErrors: [],
            };

            // If there is an HTTP request intercept it and return the above mockedResponse
            mockBackend.connections.subscribe((connection: MockConnection) => {
                connection.mockRespond(new Response(new ResponseOptions({
                    body: JSON.stringify(mockResponse)
                })));
            });

            let headers = new Headers({ 'Content-Type': 'plain/text' });
            let options = new RequestOptions({
                headers: headers
            });

            // Make the request
            httpClient.get("api/getUser/1", options).subscribe((response) => {
                expect(response).toBeDefined();
                expect(response.Data).toBeUndefined();
                expect(response.User).toBeDefined();
                expect(response.User.ID).toBe("1");
            });
        }));

        it('should return errors when the GET request failed', inject([], () => {
            // Create a mockedResponse
            const mockResponse = {
                Success: false,
                SystemErrors: ["1", "2", "3"],
                UserErrors: ["5", "6"],
            };

            // If there is an HTTP request intercept it and return the above mockedResponse
            mockBackend.connections.subscribe((connection: MockConnection) => {
                connection.mockRespond(new Response(new ResponseOptions({
                    body: JSON.stringify(mockResponse)
                })));
            });

            // Make the request
            httpClient.get("api/getUser/1").subscribe((response) => {
                // success part shouldn't be hit
                fail();
            }, error => {
                expect(error).toBeDefined();
            });
        }));
    });
    
    describe('Post', () => {
        it('should successfully make a basic Post request', inject([], () => {
            // Create a mockedResponse
            const mockResponse = {
                Success: true,
                Data: {
                    User: {
                        ID: "1",
                        firstName: 'user',
                        lastName: ''
                    }
                },
                SystemErrors: [],
                UserErrors: [],
            };

            // If there is an HTTP request intercept it and return the above mockedResponse
            mockBackend.connections.subscribe((connection: MockConnection) => {
                connection.mockRespond(new Response(new ResponseOptions({
                    body: JSON.stringify(mockResponse)
                })));
            });

            // Make the request
            let body = { "username": "user", "password": "password" };
            httpClient.post("api/auth/login", body).subscribe((response) => {
                expect(response).toBeDefined();
                expect(response.Data).toBeUndefined();
                expect(response.User).toBeDefined();
                expect(response.User.ID).toBe("1");

            });
        }));

        it('should successfully make a Post request with options', inject([], () => {
            // Create a mockedResponse
            const mockResponse = {
                Success: true,
                Data: {
                    User: {
                        ID: "1",
                        firstName: 'user',
                        lastName: ''
                    }
                },
                SystemErrors: [],
                UserErrors: [],
            };

            // If there is an HTTP request intercept it and return the above mockedResponse
            mockBackend.connections.subscribe((connection: MockConnection) => {
                connection.mockRespond(new Response(new ResponseOptions({
                    body: JSON.stringify(mockResponse)
                })));
            });

            let headers = new Headers({ 'Content-Type': 'plain/text' });
            let options = new RequestOptions({
                headers: headers
            });

            // Make the request
            let body = { "username": "user", "password": "password" };
            httpClient.post("api/auth/login", body, options).subscribe((response) => {
                expect(response).toBeDefined();
                expect(response.Data).toBeUndefined();
                expect(response.User).toBeDefined();
                expect(response.User.ID).toBe("1");
            });
        }));

        it('should return errors when the Post request failed', inject([], () => {
            // Create a mockedResponse
            const mockResponse = {
                Success: false,
                SystemErrors: ["1", "2"],
                UserErrors: ["5", "6"],
            };

            // If there is an HTTP request intercept it and return the above mockedResponse
            mockBackend.connections.subscribe((connection: MockConnection) => {
                connection.mockRespond(new Response(new ResponseOptions({
                    body: JSON.stringify(mockResponse)
                })));
            });

            // Make the request
            let body = { "username": "user", "password": "password" };
            httpClient.post("api/auth/login", body).subscribe((response) => {
                // success part shouldn't be hit
                fail();
            }, error => {
                expect(error).toBeDefined();
            });
        }));
    });

    describe('Generate Options', () => {
        it('should create new headers if none passed in', inject([], () => {
            // Create a mockedResponse
            const mockResponse = {
                Success: true,
                Data: {
                    User: {
                        ID: "1",
                        firstName: 'Thomas',
                        lastName: 'Ortiz'
                    }
                },
                SystemErrors: [],
                UserErrors: [],
            };

            // If there is an HTTP request intercept it and return the above mockedResponse
            mockBackend.connections.subscribe((connection: MockConnection) => {
                connection.mockRespond(new Response(new ResponseOptions({
                    body: JSON.stringify(mockResponse)
                })));
            });

            let options = new RequestOptions();

            // Make the request
            httpClient.get("api/getUser/1", options).subscribe((response) => {
                expect(response).toBeDefined();
                expect(response.Data).toBeUndefined();
                expect(response.User).toBeDefined();
                expect(response.User.ID).toBe("1");
            });
        }));

    });

    describe('MapRequest', () => {
        it('should return 0 errors on success', inject([], () => {
            // Create a mockedResponse
            const mockResponse = {
                Success: true,
                Data: {
                    User: {
                        ID: "1",
                        firstName: 'user',
                        lastName: ''
                    }
                },
                SystemErrors: [],
                UserErrors: [],
            };

            // If there is an HTTP request intercept it and return the above mockedResponse
            mockBackend.connections.subscribe((connection: MockConnection) => {
                connection.mockRespond(new Response(new ResponseOptions({
                    body: JSON.stringify(mockResponse)
                })));
            });

            // Make the request
            httpClient.get("api/getUser/1").subscribe((response) => {
                expect(response).toBeDefined();
                expect(response.Data).toBeUndefined();
                expect(response.User).toBeDefined();
                expect(response.User.ID).toBe("1");
            });
        }));

        it('should return 0 errors on failure with no errors in errors', inject([], () => {
            // Create a mockedResponse
            const mockResponse = {
                Success: false,
                Data: {
                    User: {
                        ID: "1",
                        firstName: 'user',
                        lastName: ''
                    }
                },
                SystemErrors: [],
                UserErrors: [],
            };

            // If there is an HTTP request intercept it and return the above mockedResponse
            mockBackend.connections.subscribe((connection: MockConnection) => {
                connection.mockRespond(new Response(new ResponseOptions({
                    body: JSON.stringify(mockResponse)
                })));
            });

            // Make the request
            httpClient.get("api/getUser/1").subscribe((response) => {
                fail();
            }, error => {
                expect(error).toBeDefined();
            });
        }));
    });

});