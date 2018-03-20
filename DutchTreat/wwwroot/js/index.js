$(document).ready(function () {
    var theForm = $("#theForm")
    theForm.hide();

    var button = $("#indexButton");
    button.on("click", function () {
        alert("don't buy!");
    });

    var productInfo = $(".product-props li");
    productInfo.on("click", function () {
        alert("you clicked on " + $(this).text());
    });

    var $loginToggle = $("#loginToggle");
    var $popUpForm = $(".popup-form");

    $loginToggle.on("click", function () {
        $popUpForm.fadeToggle(1000);
    });
});