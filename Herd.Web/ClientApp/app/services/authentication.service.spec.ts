import { TestBed, async, inject } from '@angular/core/testing';
import { HttpModule, Http, Response, ResponseOptions, XHRBackend } from '@angular/http';
import { MockBackend, MockConnection } from '@angular/http/testing';
import { Observable } from "rxjs/Observable";

import { AuthenticationService } from './authentication.service';
import { HttpClientService } from './http-client.service';
import { Storage, BrowserStorage } from '../models';

describe('Service: Authentication Service', () => {

    beforeEach(() => {
        TestBed.configureTestingModule({
            imports: [HttpModule],
            providers: [
                AuthenticationService,
                { provide: XHRBackend, useClass: MockBackend },
                HttpClientService,
                { provide: Storage, useClass: BrowserStorage },
            ]
        });
    });

    describe('Initial Auth on creation', () => {
        it('return false for isAuthenticatied',
            inject([AuthenticationService], (authService: AuthenticationService) => {
                let isAuthenticated = authService.isAuthenticated();
                expect(isAuthenticated).toBeFalsy();
            })
        );

        it('return false for isConnectedToMastodon',
            inject([AuthenticationService], (authService: AuthenticationService) => {
                let isConnectedToMastodon = authService.checkIfConnectedToMastodon();
                expect(isConnectedToMastodon).toBeFalsy();
            })
        );
    });

    it('should return a User on successful login',
        inject([AuthenticationService, XHRBackend], (authService: AuthenticationService, mockBackend: MockBackend) => {
            // Create a mockedResponse
            const mockResponse = {
                Data : {
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

            // Make the login request from our authentication service
            authService.login("thomas", "password").subscribe((response) => {
                //console.log(JSON.stringify(response, null, 2));
                let user = response.User;
                expect(user.ID).toEqual(1);
                expect(user.firstName).toEqual('Thomas');
                expect(user.lastName).toEqual('Ortiz');
            });
        })
    );

});