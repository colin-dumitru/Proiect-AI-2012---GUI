<%@ Import Namespace="WebServer.ModelView" %>
<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<DocumentModelView>" %>

<asp:Content ID="TitleContent" ContentPlaceHolderID="TitleContent" runat="server">
    Home
</asp:Content>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeaderContent" runat="server">
    Upload a document.
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <% using(Html.BeginForm("Upload", "Load", FormMethod.Post, new { enctype = "multipart/form-data" })){ %>
    	    <%: Html.ValidationSummary() %>
            <input type="file" id="fileUpload" name="file"/>
            <input type="submit" id="submitBtn" value="Upload"/>
        <% } %>
    </div>
</asp:Content>
