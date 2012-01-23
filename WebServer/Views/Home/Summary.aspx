<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="TitleContent" ContentPlaceHolderID="TitleContent" runat="server">
    Summary
</asp:Content>


<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeaderContent" runat="server">
    Summary
</asp:Content>


<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="Stylesheet" type="text/css" href="/Content/Summary.css"></link>
    
    <script src="/Scripts/jit.js" type="text/javascript"></script>
    <script src="/Scripts/jit-yc.js" type="text/javascript"></script>
    <script src="/Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
    <script src="/Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
    <script src="/Scripts/summary.js" type="text/javascript"></script>
</asp:Content>


<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div id="loading">
        Document is loading...
    </div>
    
    <div id="doc">
        <div>
            <h1 id="novel_title">
                Title
            </h1>
        </div>

        <div id="information">
            <h2>Information</h2>
            <div>
                <div>Type</div>
                <div id="novel_type"></div>
            </div>
        </div>       

        <div>
            <div id="characters">
                <h2>Characters</h2>
                <div id="novel_characters"></div>
            </div>

            <div id="container">
                <div id="center-container">
                    <div id="infovis"></div>
                </div>
                <div id="log"></div>
            </div>

            <div id="locations">
                <h2> Locations</h2>
                <div id="novel_places"></div>
            </div>
            <div id="summary">
                <h2>Summary</h2>
                <div id="novel_summary"></div>
            </div>
        </div>
</asp:Content>
