var doc;
var canvas;
var context;
var canvasWidth = 740;
var canvasHeight = 100;
var currentSituation = null;

$(document).ready(function () {
    doc = $("#doc");
    tryToLoad();
});

function dateStringToDate(dateString) {
    var a = dateString.split("/");
    var b = a[2].split(":");
    return new Date(parseInt(b[0]), parseInt(a[1]), parseInt(a[0]), parseInt(b[1]), parseInt(b[2]));
}

function tryToLoad() {
    $.get("/Content/timeline.xml", function (xmlFile) {
        var xml = $(xmlFile);
        loaded(xml);
    });
}

function loaded(xml) {
    $("<style>\
           ul.selectables {padding:0;margin:0;}\
           ul.selectables li {list-style: none; width:185px; float:left;} \
           ul.selectables li span {text-align:center; background: #CCE; display:block; -moz-border-radius: 8px; border-radius: 8px; margin:1px; padding:1px 0}\
           ul.selectables li span:hover {background: #DDF;}\
           ul.selectables li span.excluded {background: #EEE; color:#CCC}\
           div.tooltip {background:#CCE; /*width:200px; */padding:5px 5px 15px 5px; -moz-border-radius: 0 10px 10px 10px; border-radius:0 10px 10px 10px;}\
           div.tooltip h3 {margin-top:0;}\
           div.tooltip ul {padding:0;margin:0;}\
           div.tooltip ul li {list-style:none; margin:1px; background:#BBD; padding:1px 3px;}\
           div.tooltip ul li:hover {background:#DDF}\
           h2 {margin-top:30px;}\
           div#situation {min-height:400px}\
           canvas {margin-top:20px}\
       </style>").appendTo("head");
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

    canvas = $("<canvas></canvas>").attr({ id: "canv", width: canvasWidth, height: canvasHeight });
    doc.append(canvas);

    doc.append("<div id='situation'></div>");


    context = canvas[0].getContext("2d");

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

    context.fillStyle = "#DDDDDD";
    context.fillRect(0, 0, canvasWidth, canvasHeight);
    context.fillStyle = "#333355";

    drawSituations(situations, 8);
}

function drawCircle(x, y, r) {
    context.beginPath();
    context.arc(x, y, r, 0, Math.PI * 2, true);
    context.closePath();
    context.fill();
}

function drawSituations(situations, intervals) {
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
        drawCircle(i * canvasPeriod + h, h, h * ratio);
    }

    var currentElement = null;

    canvas.mousemove(function (e) {
        var off = canvas.offset();
        var x = e.pageX - off.left;
        var y = e.pageY - off.top;
        var currentInterval = Math.floor(x / canvasPeriod);
        var centerX = currentInterval * canvasPeriod + h;
        var centerY = h;
        var distance = Math.sqrt(Math.pow(centerX - x, 2) + Math.pow(centerY - y, 2));

        if (distance <= radius[currentInterval]) {
            if (currentElement != currentInterval) {
                destroyTooltip();
                currentElement = currentInterval;
                createTooltip(off.left + centerX, off.top + centerY);
            } else {
                //moveTooltip(off.left + x + 2, off.top + y + 2);
            }
        } else {
            destroyTooltip();
            currentElement = null;
        }
    });

    var toolTip;
    var createTooltip = function (x, y) {
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
            var name = xml.find("name").text();
            var li = $("<li>" + name + "</li>");
            ul.append(li);
            li.hover(function () {
                // TODO: E greșit aici. Repar mai târziu.
                situationDiv.empty();
                situationDiv.append("<h2>" + $(this).text() + "</h2>");

                var parag = $("<p></p>");
                var addThings = function (elem, name) {
                    var p = name + ": ";
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
                situationDiv.append("<p>" + xml.find("paragraph").text() + "</p>");
            });
        }
        toolTip.append(ul);

    }
    var destroyTooltip = function () {
        if (toolTip)
            toolTip.remove();
    }
    var moveTooltip = function (x, y) {
        toolTip.css({
            top: y + "px",
            left: x + "px"
        });
    }
}