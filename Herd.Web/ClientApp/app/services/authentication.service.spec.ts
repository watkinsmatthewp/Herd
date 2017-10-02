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

    describe('Login Process', () => {
        it('should return a User on successful login',
            inject([AuthenticationService, XHRBackend], (authService: AuthenticationService, mockBackend: MockBackend) => {
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

        it('should return an Error on unSuccessful login',
            inject([AuthenticationService, XHRBackend], (authService: AuthenticationService, mockBackend: MockBackend) => {
                // Create a mockedResponse
                const mockResponse = {
                    Data: {
                        Errors: [
                            { Message: "Invalid email or password" }
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
                authService.login("wrong-username", "wrong-password").subscribe((response) => {
                    let errors = response.Errors;
                    expect(errors.length).toEqual(1);
                    expect(errors[0].Message).toEqual('Invalid email or password');
                });
            })
        );
    });

    describe('Logout Process', () => {
        beforeEach(inject([Storage], (localStorage: Storage) => {
            // Add login data to storage
            const mockUser = {
                ID: 5,
                ProfileID: 5,
                MastodonConnection: {
                    ApiAccessToken: '8asdf8w3233423',
                    AppRegistrationID: 1,
                    CreatedAt: '1506886650',
                    Scope: 'read write follow',
                    TokenType: 'bearer'
                }
            }
            localStorage.setItem('currentUser', JSON.stringify(mockUser));
            localStorage.setItem('connectedToMastodon', true);
        }));

        it('should logout successfully',
            inject([AuthenticationService, XHRBackend], (authService: AuthenticationService, mockBackend: MockBackend) => {
                // Create a mockedResponse
                const mockResponse = {
                    Success: true,
                    Data: {
                        logoutSuccessful: true
                    }
                };

                // If there is an HTTP request intercept it and return the above mockedResponse
                mockBackend.connections.subscribe((connection: MockConnection) => {
                    connection.mockRespond(new Response(new ResponseOptions({
                        body: JSON.stringify(mockResponse)
                    })));
                });

                // Make the login request from our authentication service
                authService.logout().subscribe((response) => {
                    expect(response.logoutSuccessful).toBe(true);
                });
            })
        );
    });

    describe('OAuth Process', () => {
        it('should be able to get an app registrations ID', inject([AuthenticationService, XHRBackend], (authService: AuthenticationService, mockBackend: MockBackend) => {
            // Create a mockedResponse
            const mockResponse = {
                Success: true,
                Data: {
                    Registration: {
                        ID: 1
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
            authService.getRegistrationId('instance-name').subscribe((response) => {
                expect(response.Registration.ID).toBe(1);
            });

        }));
    })
});