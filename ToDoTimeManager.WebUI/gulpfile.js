// Imports section
const gulp = require('gulp');
const cleanCSS = require('gulp-clean-css');
const concat = require('gulp-concat');
const rename = require('gulp-rename');
const uglify = require('gulp-uglify-es').default;

// End imports section
// Define the paths to your CSS files
const paths = {
    css: ['wwwroot/css/**/*.css', '!wwwroot/css/site.min.css'],
    cssDest: 'wwwroot/css/',
};

// Combine and minify CSS task
function combineAndMinifyCSS() {
    return gulp.src([
        'wwwroot/css/*.css', 
        '!wwwroot/css/site.min.css'
    ], { sourcemaps: true })
        .pipe(concat('site.css')) // Combine all CSS files into one file named site.css
        .pipe(cleanCSS({ compatibility: 'ie8' })) // Minify the combined CSS file
        .pipe(rename({ suffix: '.min' })) // Rename the minified file to styles.min.css
        .pipe(gulp.dest(paths.cssDest)); // Save the minified file to the destination folder
}

function watchFiles() {
    gulp.watch(paths.css, combineAndMinifyCSS);
}

// Tasks section
gulp.task("default", gulp.series(combineAndMinifyCSS, watchFiles));

gulp.task("build", gulp.series(combineAndMinifyCSS));
// End tasks section
