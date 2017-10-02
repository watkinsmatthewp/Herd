// Karma configuration file, see link for more information
// https://karma-runner.github.io/0.13/config/configuration-file.html

module.exports = function (config) {
    config.set({
        basePath: '.',
        frameworks: ['jasmine'],
        files: [
            '../../node_modules/es6-shim/es6-shim.js',
            '../../node_modules/babel-polyfill/dist/polyfill.js',
            '../../wwwroot/dist/vendor.js',
            './boot-tests.ts',
        ],
        preprocessors: {
            './boot-tests.ts': ['webpack'],
        },
        reporters: ['dots', 'html', 'coverage'],
        htmlReporter: {
            outputDir: './ClientApp/test/karma_html', // report will get generated inside folder `karma_html` in the test folder
        },
        coverageReporter: {
            type: 'html',
            dir: 'karma_coverage_html', // report will get generated inside folder `karma_coverage_html` in the test folder
        },
        port: 9876,
        colors: true,
        logLevel: config.LOG_INFO,
        autoWatch: true,
        browsers: ['PhantomJS'],
        mime: { 'application/javascript': ['ts','tsx'] },
        singleRun: true,
        webpack: require('../../webpack.config.js')().filter(config => config.target !== 'node'), // Test against client bundle, because tests run in a browser
        webpackMiddleware: { stats: 'errors-only' }
    });
};
