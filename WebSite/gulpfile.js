// Sass configuration
var gulp = require('gulp'),    
    debug = require('gulp-debug'),
    cssmin = require('gulp-cssmin'),
    rename = require('gulp-rename'),
    sass = require('gulp-sass'),
    sourcemaps = require('gulp-sourcemaps'),
    autoprefixer = require('gulp-autoprefixer'),
    compress = require('gulp-compress'),
    uglify = require('gulp-uglify'),
    concat = require("gulp-concat"),
    autoprefixerOptions = {
        browsers: ['last 2 versions', '> 5%', 'Firefox ESR']
    };

// ### PATHS ##
var etpl = ".";
var paths = {
    scss : etpl + "/sass/*.scss",
    pathSCSS : etpl + "/sass/",
    css : etpl + "/cssDev/*.css",
    pathDevCSS : etpl + "/cssDev/",
    pathCSS : etpl + "/css/",
    js : etpl + "/jsDev/*.js",
    pathDevJS : etpl + "/jsDev/",
    pathJS : etpl + "/js/"
};

gulp.task('sass', function() {
    gulp.src(paths.scss)
        .pipe(sourcemaps.init())
        .pipe(sass().on('error', sass.logError))
        //.pipe(autoprefixer(autoprefixerOptions))
        .pipe(gulp.dest(paths.pathDevCSS))
        .pipe(sourcemaps.write('.'))
        .pipe(gulp.dest(paths.pathDevCSS));
});

var jsOrder = [
    paths.pathDevJS + 'tether.min.js',
    paths.pathDevJS + 'bootstrap.min.js',
    //paths.pathDevJS + 'lightbox.min.js',
    paths.pathDevJS + 'select2.full.min.js',
    paths.pathDevJS + 'arduino.js'
];

gulp.task('min:js', function() {
    gulp.src(jsOrder)
        .pipe(concat('arduino-all.js'))
        .pipe(gulp.dest(paths.pathJS));
        
    return gulp.src(jsOrder)
        .pipe(concat('arduino-all.min.js'))
        .pipe(uglify())
        .pipe(gulp.dest(paths.pathJS));
});

var cssOrder = [
    paths.pathDevCSS + 'tether.min.css',
    paths.pathDevCSS + 'bootstrap.min.css',
    //paths.pathDevCSS + 'normalize.css',
    paths.pathDevCSS + 'font-awesome.min.css',
    paths.pathDevCSS + 'select2.min.css',
    paths.pathDevCSS + 'style.css'
];

gulp.task('min:css', function() {
    return gulp.src(cssOrder)
        .pipe(concat('style-all.min.css'))
        .pipe(cssmin())
        .pipe(gulp.dest(paths.pathCSS))
});

gulp.task('default', function() {
    gulp.watch(paths.scss, ['sass']);
    gulp.watch(paths.css, ['min:css']);
    gulp.watch(paths.js, ['min:js']);
});