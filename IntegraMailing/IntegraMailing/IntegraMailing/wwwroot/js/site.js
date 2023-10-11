// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    $("#sidebarToggle").click(function () {
        $("#sidebar").toggleClass("open");
        $("#mainContent").toggleClass("expanded");
    });
});

$(document).ready(function () {
    $(".toggle-button").click(function () {
        $(this).parent().toggleClass("expanded");
    });
});