import { TestBed, async, inject } from '@angular/core/testing';
import { HttpModule, Http, Response, ResponseOptions, XHRBackend } from '@angular/http';
import { MockBackend, MockConnection } from '@angular/http/testing';
import { Observable } from "rxjs/Observable";

import { HttpClientService } from './http-client.service';
import { Storage, BrowserStorage } from '../../models';

describe('Service: HttpClient Service', () => {
    let http: HttpClientService;
    let mockBackend: MockBackend;

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
    beforeEach(inject([HttpClientService, MockBackend], (hcs: HttpClientService, mb: MockBackend) => {
        http = hcs;
        mockBackend = mb;
    }));

    describe('Headers', () => {
        it('should start with default headers of Content-Type', inject([], () => {
            expect(http.getHeaderByKey("Content-Type")).toBeDefined();
            expect(http.getHeaderByKey("Content-Type")).toBe("application/json; charset=UTF-8");
        }));

        it('should be able to remove headers', inject([], () => {
            expect(http.getHeaderByKey("Content-Type")).toBeDefined();
            expect(http.getHeaderByKey("Content-Type")).toBe("application/json; charset=UTF-8");

            http.removeHeader("Content-Type");
                
            expect(http.getHeaderByKey("Content-Type")).toBeUndefined();
        }));
        
        it('should be able to set headers', inject([], () => {
            expect(http.getHeaderByKey("Content-Type")).toBeDefined();
            expect(http.getHeaderByKey("Content-Type")).toBe("application/json; charset=UTF-8");

            http.setHeader("Content-Type", "text/plain");

            expect(http.getHeaderByKey("Content-Type")).toBe("text/plain");
        }));
    });

    describe('Get', () => {
        it('should make a GET request with ', inject([], () => {
            // Create a mockedResponse
            const mockResponse = {
                Data: {
                    User: {
                        ID: 1,
                        firstName: 'Thomas',
                        lastName: 'Ortiz'
                    }
                }
            };

            // If there is an HTTP request intercept it and return the above mockedResponse
            mockBackend.connections.subscribe((connection: MockConnection) => {
                connection.mockRespond(new Response(new ResponseOptions({
                    body: JSON.stringify(mockResponse)
                })));
            });

            // Make the request
            expect(http.getHeaderByKey("Content-Type")).toBeDefined();
            expect(http.getHeaderByKey("Content-Type")).toBe("application/json; charset=UTF-8");
            }));

        it('should start with default headers of Content-Type', inject([], () => {

        }));
    });

});