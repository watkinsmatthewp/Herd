//declare var require: any;

//var path = require('path');
var webpackConfig = require('../../webpack.config.js')().filter(config => config.target !== 'node')[0];
var runner = require('protractor-webpack');

runner.run('./protractor.conf.js', webpackConfig);