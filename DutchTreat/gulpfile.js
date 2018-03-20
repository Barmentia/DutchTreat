/*
This file is the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. https://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require('gulp');
var uglify = require("gulp-uglify");
var concat = require("gulp-concat");

gulp.task('minify', function () {
    // it returns all the .js files within the subdirectories in the js directory.
    return gulp.src("wwwroot/js/**/*.js")
        .pipe(uglify()) // it minifies the files, each one separately.
        .pipe(concat("dutchtreat.min.js"))  // it concats all the previous files into only one.
        .pipe(gulp.dest("wwwroot/dist"))    // it saves the previous file in the specified route.
});

gulp.task('default', ["minify"]);