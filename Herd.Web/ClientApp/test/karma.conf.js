// Karma configuration file, see link for more information
// https://karma-runner.github.io/0.13/config/configuration-file.html

var path = require('path');
var webpackConfig = require('../../webpack.config.js')().filter(config => config.target !== 'node')[0];
webpackConfig.module.rules = [...webpackConfig.module.rules, {
    test: /\.ts$/,
    include: [path.resolve(__dirname, "../app/")],
    use: {
        loader: 'istanbul-instrumenter-loader',
        options: { esModules: true },  
    },
    exclude: [/\.spec\.ts$/],
    enforce: 'post'
}];

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

        webpack: webpackConfig,
        webpackMiddleware: { stats: 'errors-only' },

        reporters: ['dots', 'karma-remap-istanbul'],
        remapIstanbulReporter: {
            reports: {
                html: 'ClientApp/test/coverage',
               'text-summary': null
            }
        },

        port: 9876,
        colors: true,
        logLevel: config.LOG_INFO,
        autoWatch: true,
        browsers: ['PhantomJS'],
        mime: { 'application/javascript': ['ts', 'tsx'] },
        singleRun: true,
    });
};
