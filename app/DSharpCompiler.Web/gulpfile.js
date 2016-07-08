
var path = require('path');
var gulp = require('gulp');

var webroot = "./wwwroot/";

var config = {
    libBase: 'node_modules',
    lib: [
        require.resolve('@angular/common/bundles/common.umd.js'),
        require.resolve('@angular/common/bundles/common.umd.js'),
        require.resolve('@angular/compiler/bundles/compiler.umd.js'),
        require.resolve('@angular/core/bundles/core.umd.js'),
        require.resolve('@angular/forms/bundles/forms.umd.js'),
        require.resolve('@angular/http/bundles/http.umd.js'),
        require.resolve('@angular/platform-browser/bundles/platform-browser.umd.js'),
        require.resolve('@angular/platform-browser-dynamic/bundles/platform-browser-dynamic.umd.js'),
        require.resolve('@angular/router-deprecated/bundles/router-deprecated.umd.js'),
        require.resolve('@angular/upgrade/bundles/upgrade.umd.js'),
        require.resolve('bootstrap/dist/css/bootstrap.min.css'),
        require.resolve('bootstrap/dist/js/bootstrap.min.js'),
        path.dirname(require.resolve('bootstrap/dist/fonts/glyphicons-halflings-regular.woff')) + '/**',
        require.resolve('core-js/client/shim.min.js'),
        require.resolve('jquery/dist/jquery.min.js'),
        require.resolve('reflect-metadata/Reflect.js'),
        path.dirname(require.resolve('rxjs/Rx.js')) + '/**',
        require.resolve('systemjs/dist/system.src.js'),
        require.resolve('toastr/build/toastr.min.js'),
        require.resolve('toastr/build/toastr.min.css'),
        require.resolve('zone.js/dist/zone.js'),
    ]
};

gulp.task('copyModules', function () {
    return gulp.src(config.lib, { base: config.libBase })
        .pipe(gulp.dest(webroot + 'lib'));
});

gulp.task("default", ["copyModules"])