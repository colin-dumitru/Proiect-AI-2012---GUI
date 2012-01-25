var doc;
var documentId;
var clusters = [4, 6, 8, 10, 14, 18, 24, 30, 44];
var clusterIndex = 1;
var toolTip = null;

$(document).ready(function () {
    $("body").mousemove(function (e) {
        window.mouseXPos = e.pageX;
        window.mouseYPos = e.pageY;
    });

    var split = document.URL.split("/");
    documentId = split[split.length - 1];
    doc = $("#doc");
    tryToLoad();
});

function dateStringToDate(dateString) {
    var a = dateString.split("/");
    var b = a[2].split(":");
    return new Date(parseInt(b[0]), parseInt(a[1]), parseInt(a[0]), parseInt(b[1]), parseInt(b[2]));
}

function tryToLoad() {
    $.get("/Get/Timeline/" + documentId, function (xmlFile) {
        var xml = $(xmlFile);
        loaded(xml);
    });
}

function loaded(xml) {
    doc.append("<h1>Timeline</h1>");
    doc.append("<h2>Characters</h2>");
    var charUl = $("<ul></ul>").addClass("selectables");
    xml.find("characters").find("character").each(function () {
        charUl.append("<li><span>" + $(this).attr("name") + "</span></li>");
    });
    doc.append(charUl);
    doc.append($("<div></div>").css({ clear: "both" }));
    doc.append("<h2>Locations</h2>");
    var locUl = $("<ul></ul>").addClass("selectables");
    xml.find("locations").find("location").each(function () {
        locUl.append("<li><span>" + $(this).attr("name") + "</span></li>");
    });
    doc.append(locUl);
    doc.append($("<div></div>").css({ clear: "both" }));

    $(".selectables span").click(function () {
        var jThis = $(this);
        if (jThis.hasClass("excluded"))
            jThis.removeClass("excluded");
        else
            jThis.addClass("excluded");
    });

    doc.append("<h2>Situation clusters</h2>");
    var bigger = $("<a class='clustersBtn nosel'>−</span>").click(function () {
        if (clusterIndex + 1 < clusters.length) {
            clusterIndex++;
            drawSituations(situations, clusters[clusterIndex], clusterIndex);
        }
    });
    var smaller = $("<a class='clustersBtn nosel'>+</span>").click(function () {
        if (clusterIndex - 1 >= 0) {
            clusterIndex--;
            drawSituations(situations, clusters[clusterIndex], clusterIndex);
        }
    });
    doc.append(bigger);
    doc.append(smaller);
    doc.append($("<div></div>").css({ clear: "both" }));
    doc.append("<div id='canvasDiv'></div>");

    doc.append("<div id='situation'></div>");

    var situations = new Array();

    xml.find("situation").each(function () {
        var jThis = $(this);
        var initialTime = dateStringToDate(jThis.attr("initial_time"));
        var finalTime = dateStringToDate(jThis.attr("final_time"));
        situations.push({ "initialTime": initialTime, "finalTime": finalTime, "xml": jThis })
    });

    var compare = function (a, b) {
        if (a.initialTime > b.initialTime) return 1;
        if (a.initialTime < b.initialTime) return -1;
        return 0;
    };

    situations.sort(compare);

    drawSituations(situations, clusters[clusterIndex], clusterIndex);
}

function drawCircle(context, x, y, r) {
    context.beginPath();
    context.arc(x, y, r, 0, Math.PI * 2, true);
    context.closePath();
    context.fill();
}

function drawSituations(situations, intervals, index) {
    if (toolTip != null)
        toolTip.remove();

    var canvasDiv = $("#canvasDiv");
    canvasDiv.empty();
    canvasDiv.css({ padding: "10px" });

    var canvasHeight = canvasDiv.height() -20 ;
    var canvasWidth = canvasDiv.width() * (index + 1);

    var canvas = $("<canvas></canvas>").attr({ id: "canv", width: canvasWidth, height: canvasHeight });
    canvasDiv.append(canvas);
    context = canvas[0].getContext("2d");

    //context.fillStyle = "#FFF";
    //context.fillRect(0, 0, canvasWidth, canvasHeight);
    context.fillStyle = "#383738";

    var min = situations[0].initialTime.getTime();
    var max = situations[situations.length - 1].finalTime.getTime();
    var timePeriod = (max - min) / intervals;
    var canvasPeriod = canvasWidth / intervals;

    var interval = new Array(intervals);
    for (var i = 0; i < intervals; i++)
        interval[i] = new Array();

    for (var i = 0; i < situations.length; i++) {
        var place = Math.floor((situations[i].initialTime.getTime() - min) / timePeriod);
        interval[place].push(situations[i]);
    }

    var maxSits = 0;
    for (var i = 0; i < interval.length; i++)
        if (interval[i].length > maxSits)
            maxSits = interval[i].length;

    var h = canvasPeriod / 2;
    var radius = new Array(interval.length);
    for (var i = 0; i < interval.length; i++) {
        if (interval[i].length == 0)
            continue;
        var ratio = Math.sqrt(interval[i].length / Math.PI) / Math.sqrt(maxSits / Math.PI);
        radius[i] = ratio * h;
        drawCircle(context, i * canvasPeriod + h, h, h * ratio);
    }

    var currentElement = null;

    var getCurrentCluster = function () {
        var off = canvas.offset();
        var x = window.mouseXPos - off.left;
        var y = window.mouseYPos - off.top;
        var currentInterval = Math.floor(x / canvasPeriod);
        var centerX = currentInterval * canvasPeriod + h;
        var centerY = h;
        var distance = Math.sqrt(Math.pow(centerX - x, 2) + Math.pow(centerY - y, 2));

        if (distance <= radius[currentInterval])
            return currentInterval;
        return null;
    }

    canvas.mousemove(function (e) {
        var theNewOne = getCurrentCluster();
        if (theNewOne == null) {
            destroyTooltip();
            currentElement = null;
        }
        else if (theNewOne != currentElement) {
            destroyTooltip();
            currentElement = theNewOne;
            createTooltip(currentElement);
        }
    });

    var createTooltip = function (currentInterval) {
        var off = canvas.offset();
        var x = off.left + currentInterval * canvasPeriod + h;
        var y = off.top + h;
        toolTip = $("<div></div>").addClass("tooltip");
        doc.append(toolTip);
        toolTip.css({
            position: "absolute",
            top: y + "px",
            left: x + "px"
        }).show();

        var situationDiv = $("#situation");

        toolTip.append("<h3>Situations:</h3>");
        var ul = $("<ul></ul>");
        for (var i = 0; i < interval[currentElement].length; i++) {
            var xml = interval[currentElement][i].xml;
            var li = $("<li>" + xml.find("name").text() + "</li>");
            ul.append(li);
            li.click(function (xml) {
                return function () {
                    situationDiv.empty();
                    situationDiv.append("<h2>Situation: " + xml.find("name").text() + "</h2>");

                    var parag = $("<p></p>");
                    var addThings = function (elem, name) {
                        var p = "<strong>" + name + ":</strong> ";
                        xml.find(elem).each(function (i) {
                            p += ((i > 0) ? ", " : "") + "<span>" + $(this).text() + "</span>";
                        });
                        parag.append(p + "<br/>");
                    };
                    addThings("player", "Players");
                    addThings("object", "Objects");
                    addThings("event", "Events");
                    addThings("keyword", "Keywords");
                    situationDiv.append(parag);
                    var paragraph = xml.find("paragraph");
                    situationDiv.append("<p><strong>From page " + paragraph.attr("pageNumber")
                            + ":</strong> " + paragraph.text() + "</p>");
                }
            } (xml));
        }
        toolTip.append(ul);
        toolTip.mouseenter(function () {
            toolTip.mouseleave(function () {
                if (getCurrentCluster() == null)
                    destroyTooltip();
            });
        });
    }
    var destroyTooltip = function () {
        if (toolTip)
            toolTip.remove();
    }
}