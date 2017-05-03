
$(document).ready(function () {

    $('#app-modal').on('submit', 'form', function (e) {

        e.preventDefault(); // Prevent standard form submission

        var $form = $(this);

        var action = $form.attr('action');
        var method = $form.attr('method');
        //var data = $form.serialize();
        // Ajax doesn't support file serialization so this method must be used.
        // Also to use this method, ajax options contentType and processData must be false.
        var formData = new FormData($form[0]);

        //var token = $('[name=__RequestVerificationToken]', $form).attr('value');

        var $submitButton = $form.find('button[type=submit]');

        $submitButton.button('loading');

        // If server returns view because of an error than modal won't close and view will be rendered on modal.
        $.ajax({
            type: 'POST',
            url: action,
            method: method,       
            data: formData,
            dataType: 'json',
            // By default, data passed in to the data option as an object (technically, anything other than a string) will be processed and transformed into a query string, 
            // fitting to the default content-type "application/x-www-form-urlencoded". If you want to send a DOMDocument, or other non-processed data, set this option to false.
            contentType: false, 
            processData: false,
            async: false,            
            success: function (data, textStatus, jqXHR) {
                $submitButton.button('reset');
                $('#app-modal').find('.close').trigger('click');
                location.reload(); // Refresh to display new data
            },
            error: function (data) {

                $submitButton.button('reset');
                var errors = data.responseJSON.errorMessages;
                
                addMessagesToValidationSummary($form, errors);
            }
        })
    });

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

});
