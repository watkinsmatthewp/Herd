// Load required polyfills and testing libraries
import 'reflect-metadata';
import 'zone.js';
import 'zone.js/dist/long-stack-trace-zone';
import 'zone.js/dist/proxy.js';
import 'zone.js/dist/sync-test';
import 'zone.js/dist/jasmine-patch';
import 'zone.js/dist/async-test';
import 'zone.js/dist/fake-async-test';
import * as testing from '@angular/core/testing';
import * as testingBrowser from '@angular/platform-browser-dynamic/testing';

declare var require: any;

// First, initialize the Angular testing environment
testing.getTestBed().initTestEnvironment(
    testingBrowser.BrowserDynamicTestingModule,
    testingBrowser.platformBrowserDynamicTesting()
);

// Then we find all the tests
const context = require.context('../', true, /\.e2e\.ts$/);

// And load the modules
context.keys().map(context);
