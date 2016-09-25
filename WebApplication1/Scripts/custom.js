function CallEventSink() {

    var personData = { "Name": "John", "Surname": "Doe", "CustomerAddress" : { "PostCode": "PE86PR" } };
    

    $.ajax({
        type: "POST",
        url: "/EventSink/EnquireyRequested",
        data: JSON.stringify({ person: personData, male: false }),
        contentType: "application/json",
        dataType: "json",
        success: function(data) {
            alert("That seemed to work! " + data);
        },
        error: DoThisOnError
    });
}



function DoThisOnError() {
    alert("No. That didn't work!");
}