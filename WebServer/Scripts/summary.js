var documentId, labelType, useGradients, nativeTextSupport, animate;
var json = new Array(); ;



$(document).ready(function () {
    var split = document.URL.split("/");
    documentId = split[split.length - 1];
    $("#loading").css("display", "block");
    $("#loading").text("Document loading...");
    $("#doc").css("display", "none");
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
            $("#loading").css("display", "block");
            $("#loading").text("Document not found...");
            $("#doc").css("display", "none");
        }
        else if (state == "Finished") {
            $("#loading").css("display", "none");
            $("#doc").css("display", "block");
            writeDoc(xml);
        }
    });
}

function writeDoc(xml) {
    var title = xml.find("name").text();

    $("#title h1").text("Summary for „" + title + "”");

    $("#novel_title").text(title);
    $("#novel_type").text(xml.find("type").text());

    xml.find("characters").find("character").each(function () {
        $("#novel_characters").append("<div>" + $(this).attr("name") + "<div>");
    });

    xml.find("locations").find("location").each(function () {
        $("#novel_places").append("<div>" + $(this).attr("name") + "<div>");
    });

    $("#novel_summary").text(xml.find("summary").text());

    /*crearea JSN-ului pentru personaje*/
    var persons = new Array();

    xml.find("characters").find("character").each(function () {
        persons.push($(this).attr("name"));
    });

    for (var i = 0; i < persons.length; i++) {
        var adjacencies = new Array();

        xml.find("relations").find("relation").each(function () {
            if ($(this).attr("character1") == persons[i])
                adjacencies.push(
                {   "nodeTo": $(this).attr("character2"),
                    "nodeFrom": $(this).attr("character1"),
                    "data": 
                    {   "$color": "#557EAA",
                        "relation": $(this).attr("verb")
                    }
                });
        });


        json.push(
        { "adjacencies": adjacencies,
            "data": {
                "$color": "#416D9C",
                "$type": "circle",
                "$dim": 30
            },
            "id": persons[i],
            "name": persons[i]
        });
    }

    initChars(json);
    
}


/*tot ce tine de graf-ul personajelor*/
var Log = {
    elem: false,
    write: function (text) {
        if (!this.elem)
            this.elem = document.getElementById('log');
        this.elem.innerHTML = text;
        this.elem.style.left = (500 - this.elem.offsetWidth / 2) + 'px';
    }
};

/*initializarea garfului*/
function initData() {
    var ua = navigator.userAgent,
      iStuff = ua.match(/iPhone/i) || ua.match(/iPad/i),
      typeOfCanvas = typeof HTMLCanvasElement,
      nativeCanvasSupport = (typeOfCanvas == 'object' || typeOfCanvas == 'function'),
      textSupport = nativeCanvasSupport
        && (typeof document.createElement('canvas').getContext('2d').fillText == 'function');

    labelType = (!nativeCanvasSupport || (textSupport && !iStuff)) ? 'Native' : 'HTML';
    nativeTextSupport = labelType == 'Native';
    useGradients = nativeCanvasSupport;
    animate = !(iStuff || !nativeCanvasSupport);
}

function initChars(json) {
    initData();

    var fd = new $jit.ForceDirected({
        //id of the visualization container
        injectInto: 'infovis',
        //Enable zooming and panning
        //by scrolling and DnD
        Navigation: {
            enable: true,
            //Enable panning events only if we're dragging the empty
            //canvas (and not a node).
            panning: 'avoid nodes',
            zooming: 10 //zoom speed. higher is more sensible
        },
        // Change node and edge styles such as
        // color and width.
        // These properties are also set per node
        // with dollar prefixed data-properties in the
        // JSON structure.
        Node: {
            overridable: true
        },
        Edge: {
            overridable: true,
            color: '#23A4FF',
            lineWidth: 0.4
        },
        //Native canvas text styling
        Label: {
            type: labelType, //Native or HTML
            size: 10,
            style: 'bold'
        },
        //Add Tips
        Tips: {
            enable: true,
            onShow: function (tip, node) {
                //count connections
                var count = 0;
                node.eachAdjacency(function () { count++; });
                //display node info in tooltip
                tip.innerHTML = "<div class=\"tip-title\">" + node.name + "</div>"
          + "<div class=\"tip-text\"><b>connections:</b> " + count + "</div>";
            }
        },
        // Add node events
        Events: {
            enable: true,
            //Change cursor style when hovering a node
            onMouseEnter: function () {
                fd.canvas.getElement().style.cursor = 'move';
            },
            onMouseLeave: function () {
                fd.canvas.getElement().style.cursor = '';
            },
            //Update node positions when dragged
            onDragMove: function (node, eventInfo, e) {
                var pos = eventInfo.getPos();
                node.pos.setc(pos.x, pos.y);
                fd.plot();
            },
            //Implement the same handler for touchscreens
            onTouchMove: function (node, eventInfo, e) {
                $jit.util.event.stop(e); //stop default touchmove event
                this.onDragMove(node, eventInfo, e);
            },
            //Add also a click handler to nodes
            onClick: function (node) {
                if (!node) return;
                // Build the right column relations list.
                // This is done by traversing the clicked node connections.
                var html = "<h4>" + node.name + "</h4><b> connections:</b><ul><li>",
            list = [];
                node.eachAdjacency(function (adj) {

                    list.push(adj.nodeTo.name + " --> " + adj.relation);
                });
                //append connections information
                $jit.id('inner-details').innerHTML = html + list.join("</li><li>") + "</li></ul>";
            }
        },
        //Number of iterations for the FD algorithm
        iterations: 200,
        //Edge length
        levelDistance: 130,
        // Add text to the labels. This method is only triggered
        // on label creation and only for DOM labels (not native canvas ones).
        onCreateLabel: function (domElement, node) {
            domElement.innerHTML = node.name;
            var style = domElement.style;
            style.fontSize = "0.8em";
            style.color = "#ddd";
        },
        // Change node styles when DOM labels are placed
        // or moved.
        onPlaceLabel: function (domElement, node) {
            var style = domElement.style;
            var left = parseInt(style.left);
            var top = parseInt(style.top);
            var w = domElement.offsetWidth;
            style.left = (left - w / 2) + 'px';
            style.top = (top + 10) + 'px';
            style.display = '';
        }
    });
    // load JSON data.
    //alert(json);
    //alert(json[0]["adjacencies"][0]["nodeTo"]+"----"+json[0]["adjacencies"][0]["nodeFrom"]+"===="+json[0]["id"]);
    fd.loadJSON(json);
    // compute positions incrementally and animate.
    fd.computeIncremental({
        iter: 40,
        property: 'end',
        onStep: function (perc) {
            Log.write(perc + '% loaded...');
        },
        onComplete: function () {
            Log.write('done');
            fd.animate({
                modes: ['linear'],
                transition: $jit.Trans.Elastic.easeOut,
                duration: 2500
            });
        }
    });
    // end
}