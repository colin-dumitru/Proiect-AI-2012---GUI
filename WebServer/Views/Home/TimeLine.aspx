<%@ Import Namespace="WebServer.ModelView" %>
<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<DocumentModelView>" %>

<asp:Content ID="TitleContent" ContentPlaceHolderID="TitleContent" runat="server">
    TimeLine
</asp:Content>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeaderContent" runat="server">
    TimeLine
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <script src="/Content/jquery-1.4.1.min.js" type="text/javascript"></script>
    <script src="/Content/timeline.js" type="text/javascript"></script>
    <div id="doc"></div>
</asp:Content>
