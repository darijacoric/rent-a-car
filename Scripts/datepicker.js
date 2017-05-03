
$(document).ready(function () {
    
    $('.datefield').attr('onkeydown', 'return false');
    
    var dateFormat = 'D/M/YYYY';
    $('.datefield').removeAttr("data-val-date"); // Removes Date validation attribute

    $('.datefield').datetimepicker({
         format: dateFormat, // Moment js format
        minDate: moment('1/1/1900', dateFormat),
        maxDate: moment('31/12/2100', dateFormat),
        //local: moment.locale('hr')
        //defaultDate: new Date(),
    });

    var dateTimeFormat = 'D/M/YYYY H:m';

    var defaultDate = new Date();
    $('#receive-datetime').attr('onkeydown', 'return false');
    $('#receive-datetime').removeAttr("data-val-date");

    $('#receive-datetime').datetimepicker({
        format: dateTimeFormat, // Moment js format
        minDate: moment(defaultDate),
        maxDate: moment('31/12/2100 23:59:59', dateTimeFormat),
        defaultDate: defaultDate,
        sideBySide: true,
        widgetPositioning: {
            horizontal: 'auto',
            vertical: 'bottom'
        }
    });
        
    $('#destination-datetime').attr('onkeydown', 'return false');
    $('#destination-datetime').removeAttr("data-val-date");

    $('#destination-datetime').datetimepicker({
        format: dateTimeFormat, // Moment js format
        minDate: moment(defaultDate),
        maxDate: moment('31/12/2100 23:59:59', dateTimeFormat),
        defaultDate: defaultDate.setDate(defaultDate.getDate() + 30),
        sideBySide: true,
        widgetPositioning: {
            horizontal: 'auto',
            vertical: 'bottom'
        }
    });

    //dateFormat = 'YYYY-MM-DD';
    //defaultDate.setDate(defaultDate.getDate());
    //$('#card-expiration-date').attr('onkeydown', 'return false');

    //$('#card-expiration-date').datetimepicker({
    //    format: dateTimeFormat, // Moment js format
    //    minDate: moment(defaultDate),
    //    maxDate: moment('2100-12-31', dateFormat),
    //    defaultDate: defaultDate
    //});
})