$(document).ready(function () {
    
    var escKey = 27;

    // All elements with href attribute
    $('.modal-link').click(function (event) {

        event.preventDefault();       

        $clickedElement = $(this);        

        $modal = $('#app-modal');

        $modal.on('hidden.bs.modal', function () {

            $(this).data('bs.modal', null);

            //$(this).find('.modal-scripts').remove();
        });

        var requestUrl = $clickedElement.attr('href');

        if ($clickedElement.is('.tr')) {
            openModalWithAjax($clickedElement, requestUrl, $modal);
        }
        else {
            if ($clickedElement.is('.btn')) {
                $clickedElement.button('loading');
            }

            setModalDataAttributes($clickedElement);

            if ($clickedElement.is('.btn')) {
                $clickedElement.button('reset');
            }
        }        
    })

    function openModalWithAjax($clickedElement, requestUrl, $modal) {

        $.ajax({
            type: 'GET',
            url: requestUrl,
            async: false,
            success: function (htmlData) {

                setModalDataAttributes($clickedElement);
                $modal.find('.modal-content').html(htmlData);
            },
            error: function (e) {
                alert("Error");
            }
        });

    }

    function setModalDataAttributes($clickedElement) {

        $clickedElement.attr('data-target', '#app-modal');
        $clickedElement.attr('data-toggle', 'modal');        
    }

    //$(document).bind('keydown', function (key) {

    //    if (key.which === escKey) {
    //        $('.modal-cancel-btn').click();
    //    }
    //});

    //// Attach listener to .modal-close-btn's so that when the button is pressed the modal dialog disappears
    //$('body').on('click', '.modal-cancel-btn', function () {

    //    $('[id*="modal-container"]').modal('hide');

    //    // Uncomment for clearing cache data
    //    $('[id*="modal-container"]').on('hidden.bs.modal', function () {
            
    //        $(this).removeData();
    //        $(this).data('bs.modal', null);
    //        $(this).empty();
            
    //        // In short: ONLY ON CANCEL BUTTON, MODAL DATA WILL BE REMOVED
    //        // Remove hidden event after modal closes because if it is not switched off
    //        // then next time in same session modal is opened, event will be active and if user
    //        // clicks outside modal and hides it then this event will be invoked and all data will be removed.            
    //        $(this).off('hidden.bs.modal');

    //    });
    //});

    //$(function () {
    //    $('.modal-save-btn').click(function () {
    //        $('[id*="modal-container"]').modal('hide');
    //    });
    //});

 
})