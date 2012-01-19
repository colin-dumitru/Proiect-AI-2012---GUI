var documentId;

$(document).ready(function () {
    var split = document.URL.split("/");
    documentId = split[split.length - 1];
    $("#doc").append("<div id='dLoading'><p>Document is loading...</p></div>");
    tryToLoad();
});

function tryToLoad() {
    $.get("/Get/Summary/" + documentId, function (xml) {
        xml = $(xml);
        var state = xml.find("State").text();
        if (state == "Parsing") {
            setTimeout(tryToLoad, 500);
        }
        else if (state == "NotFound") {
            $("#dLoading").remove();
            $("#doc").append("<h1>Document wasn't found.</h1>");
        }
        else if (state == "Finished") {
            $("#dLoading").remove();
            writeDoc(xml);
        }
    });
}

function writeDoc(xml) {
    var title = xml.find("name").text();

    $("#title h1").text("Summary for „" + title + "”");

    $("#novel_title").text(title);
    $("#novel_information").text(xml.find("type").text());

    xml.find("characters").find("character").each(function () {
        $("novel_characters").append("<p>" + $(this).text() + "</p>");
    });

    $("#novel_summary").text(xml.find("summary").text());
}