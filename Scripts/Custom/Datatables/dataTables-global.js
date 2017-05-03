
$(document).ready(function () {
    
    var tableSelector = '.table';
    $(tableSelector).attr('class', 'table table-striped table-bordered table-hover');
    
    $(tableSelector).css('background-color', '#ffffff')
    // https://datatables.net/examples/api/multi_filter.html
    $(tableSelector + ' thead .hasInput').each(function () {
        var title = $(this).text();
        $(this).html('<input type="text" placeholder="Search ' + title + '" />');
    });

    var $datatable = $(tableSelector).DataTable();
    
    // Apply the search
    $datatable.columns().every(function () {
        var that = this;
        
        $(".table thead th input[type=text]").on('keyup change', function () {

            $datatable
                .column($(this).parent().index() + ':visible')
                .search(this.value)
                .draw();
        });
    });

})
