$(function () {
    $('.custom-file-input').on('change', function () {
        var fileName = $(this).val().split('\\').pop();
        var input = $('#attachment-input');
        input.html(fileName);
        input.val(fileName);
    });


});



