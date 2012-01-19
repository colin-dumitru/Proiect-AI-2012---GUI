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
    var doc = $("#doc");
    doc.append("<h1>" + title + "</h1>");
    doc.append("<h2>Information</h2>");
    doc.append("<p>Type: " + xml.find("type").text() + "</p>");
    doc.append("<h2>Characters</h2>");
    xml.find("characters").find("character").each(function () {
        doc.append("<p>" + $(this).text() + "</p>");
    });
    doc.append("<h2>Summary</h2>");
    doc.append("<div id='sum'>" + xml.find("summary").text() + "</div>");
}