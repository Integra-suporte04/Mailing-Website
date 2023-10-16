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

/*
function adjustFontSize() {
    var infoDiv = document.querySelector('.info h6');
    var fontSize = window.getComputedStyle(infoDiv, null).getPropertyValue('font-size');
    var fontSizeNum = parseFloat(fontSize);

    while (infoDiv.scrollWidth > infoDiv.offsetWidth && fontSizeNum > 10) {
    fontSizeNum -= 0.5;  // reduz o tamanho da fonte em 0.5px a cada iteração
    infoDiv.style.fontSize = fontSizeNum + 'px';
    }
}

window.onload = adjustFontSize;  // chama a função quando a página é carregada
window.onresize = adjustFontSize;  // chama a função quando a janela é redimensionada

*/