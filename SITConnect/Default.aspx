<%@ Page Title="SITConnect" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SITConnect._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="Content/Main.css" rel="stylesheet" type="text/css" />

    <div class="gallery">
        <img class="gallery-img" src="Media/img/Gallery-1.jpg" />
    </div>

    <div class="clearfix main-content">
        <div class="section clearfix section-category">
            <h1 class="title">Browse by Category</h1>
            <br />
            <div class="col-md-2 category">
                <ul class="category-eg">
                    <li>Pens</li>
                    <li>Pencils</li>
                    <li>Highlighters</li>
                    <li>Markers</li>
                </ul>
                <img class="category-img" src="Media/img/Writing Supplies.jpg" />
                <a class="category-title" href="javascript:void()">Writing Supplies</a>
            </div>
            <div class="col-md-2 category">
                <ul class="category-eg">
                    <li>Paint</li>
                    <li>Brushes</li>
                    <li>Colour Pencils</li>
                    <li>Scissors</li>
                    <li>Glue</li>
                </ul>
                <img class="category-img" src="Media/img/Art Supplies.png" />
                <a class="category-title" href="javascript:void()">Art & Craft Supplies</a>
            </div>
            <div class="col-md-2 category">
                <ul class="category-eg">
                    <li>Files</li>
                    <li>School Bags</li>
                    <li>Holders & Dividers</li>
                    <li>Pencil Cases</li>
                </ul>
                <img class="category-img" src="Media/img/Filing and Storage.png" />
                <a class="category-title" href="javascript:void()">Filing & Storage</a>
            </div>
            <div class="col-md-2 category">
                <ul class="category-eg">
                    <li>Labels</li>
                    <li>Notebooks</li>
                    <li>Photocopy Paper</li>
                    <li>Sticky Notes</li>
                </ul>
                <img class="category-img" src="Media/img/Paper Products.png" />
                <a class="category-title" href="javascript:void()">Notebook & Paper Products</a>
            </div>
            <div class="col-md-2 category">
                <ul class="category-eg">
                    <li>Printers</li>
                    <li>Calculators</li>
                    <li>Cartridge & Toner</li>
                </ul>
                <img class="category-img" src="Media/img/Electronics.png" />
                <a class="category-title" href="javascript:void()">Electronics</a>
            </div>
            <div class="col-md-2 category">
                <ul class="category-eg">
                    <li>Printers</li>
                    <li>Calculators</li>
                    <li>Cartridge & Toner</li>
                </ul>
                <img class="category-img" src="Media/img/Others.png" />
                <a class="category-title" href="javascript:void()">Others</a>
            </div>
        </div>
        <div class="section clearfix section-hot">
            <h1 class="title">Hot This Week</h1>
        </div>
    </div>

</asp:Content>
