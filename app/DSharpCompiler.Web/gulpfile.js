var gulp = require('gulp');

gulp.task("copyNodeModules", () => {
    gulp.src([
            'core-js/client/**',
            'systemjs/dist/**',
            'reflect-metadata/**',
            'rxjs/**',
            'zone.js/dist/**',
            '@angular/**',
            'jquery/dist/jquery.*js',
            'bootstrap/dist/**',
            "toastr/build/**"
    ], {
        cwd: "node_modules/**"
    })
        .pipe(gulp.dest("./wwwroot/libs"));
});

gulp.task('default', ['copyNodeModules']);