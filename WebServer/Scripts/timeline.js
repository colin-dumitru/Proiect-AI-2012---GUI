﻿var doc;
var documentId;
var clusters = [4, 6, 8, 10, 14, 18, 24, 30, 44];
var clusterIndex = 4;
var toolTip = null;
var situations;
var excludedPlayers = new Object();
var excludedPlaces = new Object();

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
    var charUl = $("<ul></ul>").attr("id", "playerList").addClass("selectables nosel");
    xml.find("characters").find("character").each(function () {
        var name = $(this).attr("name");
        charUl.append("<li><span>" + name + "</span></li>");
        excludedPlayers[name] = false;
    });
    doc.append(charUl);
    doc.append($("<div></div>").css({ clear: "both" }));
    doc.append("<h2>Locations</h2>");
    var locUl = $("<ul></ul>").attr("id", "placeList").addClass("selectables nosel");
    xml.find("locations").find("location").each(function () {
        locUl.append("<li><span>" + $(this).attr("name") + "</span></li>");
    });
    doc.append(locUl);
    doc.append($("<div></div>").css({ clear: "both" }));

    $("#playerList span").click(function () {
        var jThis = $(this);

        if (jThis.hasClass("excluded"))
            jThis.removeClass("excluded");
        else
            jThis.addClass("excluded");

        excludedPlayers[jThis.text()] = jThis.hasClass("excluded");
        drawSituations(situations, clusters[clusterIndex], clusterIndex);
    });

    $("#placeList span").click(function () {
        var jThis = $(this);

        if (jThis.hasClass("excluded"))
            jThis.removeClass("excluded");
        else
            jThis.addClass("excluded");

        excludedPlaces[jThis.text()] = jThis.hasClass("excluded");
        drawSituations(situations, clusters[clusterIndex], clusterIndex);
    });

    doc.append("<h2>Situation clusters</h2>");
    var bigger = $("<a class='clustersBtn nosel'>+</span>").click(function () {
        if (clusterIndex + 1 < clusters.length) {
            clusterIndex++;
            drawSituations(situations, clusters[clusterIndex], clusterIndex);
        }
    });
    var smaller = $("<a class='clustersBtn nosel'>-</span>").click(function () {
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

    situations = new Array();

    var getAsArray = function (xml, name) {
        var ret = new Array();
        xml.find(name + "s").find(name).each(function () {
            ret.push($(this).text());
        });
        return ret;
    };

    xml.find("situation").each(function () {
        var jThis = $(this);
        var paragraph = jThis.find("paragraph");
        situations.push({
            "initialTime": dateStringToDate(jThis.attr("initial_time")),
            "finalTime": dateStringToDate(jThis.attr("final_time")),
            "name": jThis.find("name").text(),
            "players": getAsArray(jThis, "player"),
            "objects": getAsArray(jThis, "object"),
            "events": getAsArray(jThis, "event"),
            "keywords": getAsArray(jThis, "keyword"),
            "places": getAsArray(jThis, "place"),
            "pageNumber": paragraph.attr("pageNumber"),
            "paragraph": paragraph.text()
        });
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

function drawSituations(situations, intervals, clusterIndex) {
    if (toolTip != null)
        toolTip.remove();

    var canvasDiv = $("#canvasDiv");
    canvasDiv.empty();
    var canvasWidth = canvasDiv.width() * (clusterIndex + 1);
    var canvasHeight = canvasDiv.height() - 20;

    var canvas = $("<canvas></canvas>").attr({ id: "canv", width: canvasWidth, height: canvasHeight });
    canvasDiv.append(canvas);
    context = canvas[0].getContext("2d");

    var min = situations[0].initialTime.getTime();
    var max = situations[situations.length - 1].finalTime.getTime();
    var timePeriod = (max - min) / intervals;
    var canvasPeriod = canvasWidth / intervals;

    var interval = new Array(intervals);
    for (var i = 0; i < intervals; i++)
        interval[i] = new Array();

    for (var i = 0; i < situations.length; i++) {
        var permitted = true;
        for (var j = 0; j < situations[i].players.length; j++)
            if (excludedPlayers[situations[i].players[j]])
                permitted = false;
        for (var j = 0; j < situations[i].places.length; j++)
            if (excludedPlaces[situations[i].places[j]])
                permitted = false;
        if (permitted) {
            var place = Math.floor((situations[i].initialTime.getTime() - min) / timePeriod);
            interval[place].push(situations[i]);
        }
    }

    var maxSits = 0;
    for (var i = 0; i < interval.length; i++)
        if (interval[i].length > maxSits)
            maxSits = interval[i].length;

    var h = canvasHeight / 2;

    // The arrow.
    context.fillStyle = "#888";
    context.fillRect(0, h - 0.5, canvasWidth - 5, 1);
    context.beginPath();
    context.moveTo(canvasWidth, h);
    context.lineTo(canvasWidth - 6, h - 3);
    context.lineTo(canvasWidth - 6, h + 3);
    context.fill();

    context.fillStyle = "#383738";

    var radius = new Array(interval.length);
    for (var i = 0; i < interval.length; i++) {
        if (interval[i].length == 0)
            continue;
        var ratio = Math.sqrt(interval[i].length / Math.PI) / Math.sqrt(maxSits / Math.PI);
        radius[i] = ratio * h / 2;
        drawCircle(context, i * canvasPeriod + h, h, h * ratio / 2);
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
            var sit = interval[currentElement][i];
            var li = $("<li>" + sit.name + "</li>");
            ul.append(li);
            li.click(function (sit) {
                return function () {
                    situationDiv.empty();
                    situationDiv.append("<h2>Situation: " + sit.name + "</h2>");

                    var parag = $("<p></p>");
                    var pos = ["Players", "Objects", "Events", "Places", "Keywords"];
                    pos.forEach(function (name) {
                        parag.append("<strong>" + name + ":</strong> " +
                                sit[name.toLowerCase()].join(", ") + "<br/>");
                    });

                    situationDiv.append(parag);
                    situationDiv.append("<p><strong>From page " + sit.pageNumber
                            + ":</strong> " + sit.paragraph + "</p>");
                }
            } (sit));
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