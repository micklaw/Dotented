var gulp = require('gulp'),
    browserSync = require('browser-sync').create(),
    cssmin = require('gulp-cssmin'),
    sourcemaps = require('gulp-sourcemaps'),
    sass = require('gulp-sass'),
    browserify = require('browserify'),
    source = require('vinyl-source-stream'),
    buffer = require('vinyl-buffer');

var config = {
    dist: "dist",
    root: "Example"
}

gulp.task('copy', function () {
    gulp.src('CNAME')
        .pipe(gulp.dest(config.dist));

    gulp.src(config.root + '/plugins/animate/*.css')
        .pipe(gulp.dest(config.dist + "/css"));

    gulp.src(config.root + '/plugins/bootstrap/css/*.min.css')
        .pipe(gulp.dest(config.dist + "/css"));

    gulp.src(config.root + '/plugins/bootstrap/js/*.min.js')
        .pipe(gulp.dest( config.dist + "/js"));

    gulp.src(config.root + '/plugins/jquery/*.min.js')
        .pipe(gulp.dest( config.dist + "/js"));

    gulp.src(config.root + '/plugins/owl-carousel/*.min.js')
        .pipe(gulp.dest( config.dist + "/js"));

    gulp.src(config.root + '/plugins/owl-carousel/assets/*.min.css')
        .pipe(gulp.dest(config.dist + "/css"));

    gulp.src(config.root + '/plugins/owl-carousel/assets/*.{gif,png}')
        .pipe(gulp.dest(config.dist + "/css"));

    gulp.src(config.root + '/plugins/pe-icons/**/*.*')
        .pipe(gulp.dest(config.dist + "/css"));

    gulp.src(config.root + '/plugins/video-background/*.js')
        .pipe(gulp.dest( config.dist + "/js"));

    gulp.src(config.root + '/plugins/video-background/*.css')
        .pipe(gulp.dest( config.dist + "/css"));

    gulp.src(config.root + '/plugins/flexslider/*-min.js')
        .pipe(gulp.dest( config.dist + "/js"));

    gulp.src(config.root + '/plugins/flexslider/*.css')
        .pipe(gulp.dest( config.dist + "/css"));

    gulp.src(config.root + '/plugins/flexslider/fonts/*.*')
        .pipe(gulp.dest( config.dist + "/css/fonts"));

    gulp.src(config.root + '/plugins/font-awesome/css/*.min.css')
        .pipe(gulp.dest( config.dist + "/css"));
    
    gulp.src(config.root + '/plugins/font-awesome/fonts/*.*')
        .pipe(gulp.dest( config.dist + "/fonts"));

    gulp.src(config.root + '/plugins/wow/dist/*.min.js')
        .pipe(gulp.dest( config.dist + "/js"));

    return gulp.src(config.root + '/images/**/*.*')
        .pipe(gulp.dest( config.dist + "/images"));
});

gulp.task('scss', gulp.series('copy', function () {
    return gulp.src(config.root + '/scss/site.scss')
        .pipe(sass().on('error', sass.logError))
        .pipe(cssmin())
        .pipe(gulp.dest(config.dist + '/css'))
        .pipe(browserSync.stream());
}));

gulp.task('js', function () {
    return browserify(config.root + '/js/site.js', { debug: true })
        .bundle()
        .pipe(source('app.js'))
        .pipe(buffer())
        .pipe(sourcemaps.init({ loadMaps: true }))
        .pipe(sourcemaps.write('.'))
        .pipe(gulp.dest(config.dist + '/js'))
        .pipe(browserSync.stream());
});

gulp.task('fonts', function () {
    return gulp.src(config.root + '/bootstrap/fonts/*.*')
        .pipe(gulp.dest(config.dist + "/fonts"));
});

gulp.task('build', gulp.series('scss', 'js', 'fonts'));

gulp.task('run', gulp.series('build', function () {
    browserSync.init({
        server: {
            baseDir: config.dist
        }
    });

    gulp.watch(config.root + "/scss/**/*.scss", gulp.series('scss'));
    gulp.watch(config.root + "/js/**/*.js", gulp.series('js'));
    gulp.watch(config.dist + "/css/**/*.css").on('change', browserSync.reload);
    gulp.watch(config.dist + "/js/*.js").on('change', browserSync.reload);
    gulp.watch(config.dist + "/**/*.html").on('change', browserSync.reload);
    return gulp.watch(config.dist + "/*.html").on('change', browserSync.reload);
}));