$(document).ready(function () {

    $('#profile-picture-upload').change(function () {
        readURLUser(this);
    });

    $('#location-picture-upload').change(function () {
        readURLLocation(this);
    });

    $('#vehicle-picture-upload').change(function () {
        readURLVehicle(this);
    });

})


function readURLUser(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $('#profile-photo')
                .attr('src', e.target.result)
                .width(150)
                .height(200);
        };

        reader.readAsDataURL(input.files[0]);

        $('#upload-file-info').text(input.files[0].name);
    }
}

function readURLLocation(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $('#location-photo')
                .attr('src', e.target.result);
        };

        reader.readAsDataURL(input.files[0]);

        $('#upload-file-info').text(input.files[0].name);
    }
}

function readURLVehicle(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $('#vehicle-photo')
                .attr('src', e.target.result);
        };

        reader.readAsDataURL(input.files[0]);

        $('#upload-file-info').text(input.files[0].name);
    }
}