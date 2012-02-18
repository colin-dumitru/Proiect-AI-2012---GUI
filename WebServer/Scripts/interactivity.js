function GetAnswer() {
    var elem = document.createElement("li");
    elem.innerText = $("#question").val();
    elem.className = "question";

    $("#answers").append(elem);
    

    /*facem o cerere jquery la server*/
    $.ajax({
        url: "/Get/Answer",
        type: "post",
        data: { id: document.docId, question: $("#question").val() },
        dataType: "json",
        success: function (data) {
            var elem = document.createElement("li");
            elem.innerText = data.Answer;
            elem.className = "answer";

            $("#answers").append(elem);
        }
    });
}