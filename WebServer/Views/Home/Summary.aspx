<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="TitleContent" ContentPlaceHolderID="TitleContent" runat="server">
    Summary
</asp:Content>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeaderContent" runat="server">
    Summary
</asp:Content>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
   <script src="/Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
   <script src="/Scripts/summary.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">      
    <div id="doc">
        <div>
            <h1 id="novel_title"> Title </h1>
        </div>
        <div>
            <h2>Information</h2>
            <div>
                <div>Type</div>
                <div id="novel_type"></div>
            </div>
        </div>

        <div>
            <h2>Characters</h2>
            <div id="novel_characters">
            </div>
        </div>

        <div>
            <h2>Locations</h2>
            <div id="novel_places">
            </div>
        </div>

        <div>
            <h2>Summary</h2>
            <div id="novel_summary">
            </div>
        </div>
    
    </div>
</asp:Content>
