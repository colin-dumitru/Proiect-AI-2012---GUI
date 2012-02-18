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
        <% using (Html.BeginForm("Upload", "Load", FormMethod.Post, new { enctype = "multipart/form-data" })) { %>
        <%: Html.ValidationSummary() %>
        <div>
            <h1>Please upload a document.</h1>
        </div>

        <div id="upload">
            <input type="file" id="fileUpload" name="file" />
            <input class="submit" type="submit" id="submitBtn" value="Upload" />
        </div>
        <% } %>
    </div>
</asp:Content>
