var helpers = require('./helpers');

exports.config = {
    framework: 'jasmine',
    jasmineNodeOpts: {
        showTiming: true,
        showColors: true,
        isVerbose: false,
        includeStackTrace: false,
        defaultTimeoutInterval: 400000
      },
    seleniumAddress: 'http://localhost:4444/wd/hub',
    specs: [
        helpers.root('/**/**.e2e.ts')
    ],
    allScriptsTimeout: 110000,
    useAllAngular2AppRoots: true,
}