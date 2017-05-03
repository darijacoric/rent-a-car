
$(document).ready(function () {

    setDropDownLocations($('#ReceiveCity'), $('#ReceivePlace'));
    setDropDownLocations($('#DestinationCity'), $('#DestinationPlace'));

    $('#ReceiveCity').on('change', function () {

        setDropDownLocations($(this), $('#ReceivePlace'));
    })
    $('#DestinationCity').on('change', function () {

        setDropDownLocations($(this), $('#DestinationPlace'));
    });

    $('#confirm-order-btn').click(function () {

        var action = confirm('Do you want to confirm order?');

        if (action == true) {
            var previousClass = $(this).attr('class');
            var previousWidth = $(this).width();

            // $(this).attr('class', 'btn btn-success next-btn')
            $(this).button('loading');
            $(this).width(100);

            ajaxConfirmOrder();

            $(this).button('reset');
            $(this).width(previousWidth);
            // $(this).attr('class', previousClass);
        }

    })
})

function ajaxConfirmOrder() {

    var currentPath = window.location.path;
    $.ajax({
        type: 'POST',
        url: "/Home/OrderSummary",
        dataType: 'json',
        async: false,
        success: function (data) {

            var successMsgHead = data.headMsg;
            var successMsgBody = data.bodyMsg;

            //setDivResponse('#order-response-success', 'alert alert-success fade show', successMsgHead, successMsgBody)
            window.location.href = '/Home';


        },
        error: function (errorData) {

            var errorMsgHeading = errorData.responseJSON.errorHead;
            var errorContent = errorData.responseJSON.errorBody;

            setDivResponse('#order-response-error', 'alert alert-danger', errorMsgHeading, errorContent)
        }
    })
}

function setDivResponse(divId, newClass, headingMsg, contentMsg) {
    var $div = $(divId);
    $div.attr('class', newClass);

    $div.html('<button type="button" class="close" data-dismiss="alert" aria-label="Close">' +
                    '<span aria-hidden="true">&times;</span>' +
                '</button>' +
                '<h4 class="alert-heading">' + headingMsg + '</h4>' +
                '<p>' + contentMsg + '</p>');
}

function setDropDownLocations($CityDropDown, $PlacesDropDown) {
    var selectedCity = $CityDropDown.val();

    var filteredPlaces = getFilteredPlaces(selectedCity);

    if (filteredPlaces == null) {
        return;
    }

    setDropDownFilteredOptions($PlacesDropDown, filteredPlaces);
}

function setDropDownFilteredOptions($placesDropDown, placesArray) {

    $placesDropDown.empty();

    $.each(placesArray, function (index, value) {
        $placesDropDown.append('<option>' + value + '</option>');
    });
}

function getFilteredPlaces(selectedCity) {

    var $form = $('#date-and-location-form');
    var placesArray = null;

    $.ajax({
        type: 'POST',
        url: '/Home/Places',
        data: { selectedCity: selectedCity },
        dataType: 'json',
        async: false,
        success: function (places) {

            placesArray = places;
        },
        error: function (data) {
            var errors = data.responseJSON.errorMessages;
            addMessagesToValidationSummary($form, errors);
        }
    });

    return placesArray;
}

function addMessagesToValidationSummary($form, errors) {

    if (!errors) {
        errors = "Error retrieving error messages from server.";
    }

    var errorList = [];

    if ($.isArray(errors)) {
        $.each(errors, function (index, value) {
            var msg = value;
            errorList.push(value);
        });
    }
    else {
        errorList.push(errors);
    }

    // find summary div
    var $summary = $form.find("[data-valmsg-summary=true]");

    // find the unordered list
    var $ul = $summary.find("ul");

    // Clear existing errors from DOM by removing all element from the list
    $ul.empty();

    if (errorList.length < 1) {
        errorList.push("Error retrieving error messages from server.");
    }

    // Add all errors to the list
    $.each(errorList, function (index, message) {
        $("<li />").html(message).appendTo($ul);
    });

    // Add the appropriate class to the summary div
    $summary.removeClass("validation-summary-valid")
        .addClass("validation-summary-errors");

}

