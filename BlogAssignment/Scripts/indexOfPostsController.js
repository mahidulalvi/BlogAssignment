$(document).ready(function () {

    var counter = 2000;
    //var inputBox = $(".search-input");

    var timeOutVariable;
    var inputBox;

    $(".search-input").on('keyup', function (e) {
        e.preventDefault();
        inputBox = $(this);

        Starter();
    })

    function Starter() {
        timeOutVariable = setTimeout(function () {
            inputBox.closest('form').submit();
        }, counter);

    }

    function Stopper() {
        clearTimeout(timeOutVariable);
    }


    $(window).on('keydown', function (e) {        
        Stopper();
    });

    
});