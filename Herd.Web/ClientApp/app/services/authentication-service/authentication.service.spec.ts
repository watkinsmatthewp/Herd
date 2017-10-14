import { TestBed, async, inject } from '@angular/core/testing';
import { HttpModule, Http, Response, ResponseOptions, XHRBackend } from '@angular/http';
import { MockBackend, MockConnection } from '@angular/http/testing';
import { Observable } from "rxjs/Observable";

import { AuthenticationService } from '../authentication-service/authentication.service';
import { HttpClientService } from '../http-client-service/http-client.service';
import { Storage, BrowserStorage, User } from '../../models';

describe('Service: Authentication Service', () => {
    let authService: AuthenticationService;

    beforeEach(() => {
        // Set up the test bed
        TestBed.configureTestingModule({
            imports: [HttpModule],
            providers: [
                AuthenticationService,
                { provide: XHRBackend, useClass: MockBackend },
                HttpClientService,
                { provide: Storage, useClass: BrowserStorage },
            ],
        });
    });

    // Inject commonly used services
    beforeEach(inject([AuthenticationService], (as: AuthenticationService) => {
        authService = as;
    }));
    
    describe('Initial Auth on creation', () => {
        it('should return false for isAuthenticatied', () => {
            let isAuthenticated = authService.isAuthenticated();
            expect(isAuthenticated).toBeFalsy();
        });

        it('should return false for isConnectedToMastodon', () => {
            let isConnectedToMastodon = authService.checkIfConnectedToMastodon();
            expect(isConnectedToMastodon).toBeFalsy();
        });
    });

    describe('Login Process', () => {
        it('should return a User on successful login',
            inject([XHRBackend], (mockBackend: MockBackend) => {
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
                    let user = response.User;
                    expect(user.ID).toEqual(1);
                    expect(user.firstName).toEqual('Thomas');
                    expect(user.lastName).toEqual('Ortiz');
                });
            })
        );

        it('should return an Error on unSuccessful login',
            inject([XHRBackend], (mockBackend: MockBackend) => {
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

        afterEach(inject([Storage], (localStorage: Storage) => {
            localStorage.clear();
        }));

        it('should logout successfully',
            inject([XHRBackend], (mockBackend: MockBackend) => {
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

                expect(localStorage.getItem("currentUser")).toBeDefined();
                expect(localStorage.getItem("connectedToMastodon")).toBeTruthy();
                // Make the login request from our authentication service
                authService.logout().subscribe((response) => {
                    expect(response.logoutSuccessful).toBe(true);
                });
            })
        );
    });

    describe('OAuth Process', () => {
        it('should be able to get an app registrations ID',
            inject([XHRBackend], (mockBackend: MockBackend) => {
                const instanceName = 'instance-name';
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
                authService.getRegistrationId(instanceName).subscribe((response) => {
                    expect(response.Registration.ID).toBe(1);
                });
            })
        );

        it('should be able to get an oAuth Url', inject([XHRBackend], (mockBackend: MockBackend) => {
            const instanceID = 1;
            // Create a mockedResponse
            const mockResponse = {
                Success: true,
                Data: {
                    URL: 'https://example.com'
                }
            };

            // If there is an HTTP request intercept it and return the above mockedResponse
            mockBackend.connections.subscribe((connection: MockConnection) => {
                connection.mockRespond(new Response(new ResponseOptions({
                    body: JSON.stringify(mockResponse)
                })));
            });

            // Make the login request from our authentication service
            authService.getOAuthUrl(instanceID).subscribe((response) => {
                expect(response.URL).toBe('https://example.com');
            });
        }));

        it('should be able to submit an oAuth token', inject([XHRBackend], (mockBackend: MockBackend) => {
            // Create a mockedResponse
            const mockResponse = {
                Success: true,
                Data: {
                    assignedToken: true
                }
            };

            // If there is an HTTP request intercept it and return the above mockedResponse
            mockBackend.connections.subscribe((connection: MockConnection) => {
                connection.mockRespond(new Response(new ResponseOptions({
                    body: JSON.stringify(mockResponse)
                })));
            });

            // Make the login request from our authentication service
            authService.submitOAuthToken('abc123', 1).subscribe((response) => {
                expect(response.assignedToken).toBe(true);
            });
        }));
    });

    describe('Register Process', () => {
        it('should let a new user register successfully',
            inject([XHRBackend], (mockBackend: MockBackend) => {
                // Create a mockedResponse
                const mockResponse = {
                    Data: {
                        Profile: {
                            FirstName: 'Jane',
                            LastName: 'Doe',
                            ID: 1,
                            UserID: 1,
                        },
                        User: {
                            ID: 1,
                            ProfileID: 1.
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
                const mockUser: User = {
                    id: 1,
                    email: 'JaneDoe@gmail.com',
                    password: 'password',
                    firstName: 'Jane',
                    lastName: 'Doe',
                    oAuthToken: 'token',
                };
                authService.register(mockUser).subscribe((response) => {
                    let user = response.User;
                    let profile = response.Profile;
                    expect(profile.FirstName).toEqual('Jane');
                    expect(profile.LastName).toEqual('Doe');
                    expect(profile.ID).toEqual(1);
                    expect(profile.UserID).toEqual(1);
                    expect(user.ID).toEqual(1);
                    expect(user.ProfileID).toEqual(1);
                });
            })
        );

        it('should return an error when new user registers with a pre-existing email',
            inject([XHRBackend], (mockBackend: MockBackend) => {
                // Create a mockedResponse
                const mockResponse = {
                    Data: {
                        Errors: [
                            { Message: 'Email is already registered' }
                        ]
                    }
                };

                // If there is an HTTP request intercept it and return the above mockedResponse
                mockBackend.connections.subscribe((connection: MockConnection) => {
                    connection.mockRespond(new Response(new ResponseOptions({
                        body: JSON.stringify(mockResponse)
                    })));
                });

                const mockUser: User = {
                    id: 1,
                    email: 'pre-existing-email',
                    password: 'password',
                    firstName: 'Jane',
                    lastName: 'Doe',
                    oAuthToken: 'token',
                }
                // Make the login request from our authentication service
                authService.register(mockUser).subscribe((response) => {
                    let errors = response.Errors;
                    expect(errors.length).toEqual(1);
                    expect(errors[0].Message).toEqual('Email is already registered');
                });
            })
        );
    });
});