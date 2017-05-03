$(document).ready(function () {
       
    $("#Facebook").click(function () {

        //onButtonClick();
    });

});
function onButtonClick() {
    // Add this to a button's onclick handler
    FB.AppEvents.logEvent("sentFriendRequest");
}