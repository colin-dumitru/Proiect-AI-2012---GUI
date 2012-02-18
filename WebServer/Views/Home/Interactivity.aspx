<%@ Import Namespace="WebServer.ModelView" %>
<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<DocumentModelView>" %>

<asp:Content ID="TitleContent" ContentPlaceHolderID="TitleContent" runat="server">
    Ask a Question
</asp:Content>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeaderContent" runat="server">
    Ask a Question
</asp:Content>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="Stylesheet" type="text/css" href="/Content/Interactivity.css"></link>
    <script src="../../Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="/Scripts/interactivity.js"></script>

    <script type="text/javascript">

        function init() {
            document.docId = <%=Model.DocumentId %>;
        }

        init();
    </script>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <ul id="answers"></ul>
        <div class="input_container">
            <input id="question"/>
            <button onclick="GetAnswer();">Send</button>
        </div>       
        
    </div>
</asp:Content>
