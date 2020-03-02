'use strict';

const gulp = require('gulp');
const sass = require('gulp-sass');
const nodeSass = require( 'node-sass');
sass.compiler = nodeSass;
const inlineCss = require('gulp-inline-css');
const styleInject = require('gulp-style-inject');
const fileInclude = require('gulp-file-include');
const handlebars = require( 'gulp-compile-handlebars');
const rename = require('gulp-rename');
const replace = require('gulp-replace');
const watch = require('gulp-watch');
const beautify = require( 'gulp-beautify');

const templatesPath = './Templates';
const targetPath = templatesPath + '/sample';

const paths = {
    scssFiles: targetPath + '/scss/*.scss',
    scssFilesAll: targetPath + '/scss/**/*.scss',
    htmlFiles: targetPath + '/html/*.html',
    htmlFilesAll: targetPath + '/html/**/*.html',
    dataFile: targetPath + '/data/replacements.json',
    htmlPartialsFolder: targetPath + '/html/partials/',
    distFolder: targetPath + '/dist/',
    cssFolder: targetPath + '/dist/css/'
};

gulp.task('scss:compile', () => {
    return gulp.src(paths.scssFiles)
        .pipe(sass().on('error', sass.logError))
        .pipe(gulp.dest(paths.cssFolder));
});

gulp.task('html:inline', () => {
    return gulp.src(paths.htmlFiles)
        .pipe(fileInclude({
            prefix: '@@',
            basepath: paths.htmlPartialsFolder
        }))
        // Workaround for inexplicable characters:
        // https://github.com/haoxins/gulp-file-include/issues/128#issuecomment-283314286
        .pipe(replace(/[\u200B-\u200D\uFEFF]/g, ""))
        .pipe(styleInject({ path: paths.cssFolder }))
        .pipe(inlineCss({
            preserveMediaQueries: true,
            applyStyleTags: true,
            applyLinkTags: true,
            removeStyleTags: true,
            removeLinkTags: false,
            applyWidthAttributes: true
        }))
        .pipe(rename({
            extname: ".hbs"
        }))
        .pipe(beautify.html({ indent_size: 4 }))
        .pipe(gulp.dest(paths.distFolder))
        .pipe(handlebars(require(paths.dataFile), { ignorePartials: true }))
        .pipe(rename({
            extname: ".html"
        }))
        .pipe(gulp.dest(paths.distFolder));
});

gulp.task('default', gulp.series('scss:compile', 'html:inline'));

gulp.task('watch', function () {
    return watch(
        [paths.scssFilesAll, paths.htmlFilesAll, paths.dataFile],
        {
            verbose: true
        },
        gulp.series('scss:compile', 'html:inline'));
});