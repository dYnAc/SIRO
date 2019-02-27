define([
        // Dojo
        "dojo/dom",
        "dojo/_base/declare",
        "dojo/html",
        "dojo/on", 
        "dojo/request", 
        "dojo/dom-form",
        "dojo/dom-style",
        "dojo/mouse",
        "dojo/domReady!",
        
        // Dijit
        "dijit/Dialog",
        "dijit/registry",
        "dijit/_TemplatedMixin",
        "dijit/_Widget",
        "dijit/_WidgetsInTemplateMixin",

        //CMS (use epi/cms if you are running EPiServer 7.0, epi-cms if you are running 7.1 or above)
        "epi-cms/_ContentContextMixin"
], function (
        // Dojo
        dom,
        declare,
        html,
        on,
        request,
        domForm,
        domStyle,
        mouse,
        domReady,

        // Dijit
        dialog,
        registry,
        _TemplatedMixin,
        _Widget,
        _WidgetsInTemplateMixin,

        //CMS
        _ContentContextMixin
    ) {

    //We declare the namespace of our widget and add the mixins we want to use.
    //Note: Declaring the name of the widget is not needed in EPiServer 7.1
    //but the release version of EPiServer 7 uses Dojo 1.7 and still needs this.
    return declare("nucleus/components/SitemapGadget", [_Widget, _TemplatedMixin, _WidgetsInTemplateMixin], {

        // summary: A simple widget that listens to changes to the 
        // current content item and puts the name in a div.

        templateString: dojo.cache("/SitemapGadget"),
      
        startup: function () {
            this.inherited(arguments);

            var myButton = dom.byId("btnGenerateSitemaps");
            
            on(myButton, "click", function (evt) {

                // Post the data to the server
                request.post("/SitemapGadget/GenerateSitemap", {
                    // Send the username and password
                    data: domForm.toObject("sitemapForm"),
                    // Wait 2 seconds for a response
                    timeout: 2000,
                    handleAs: "json"
                }).then(function (response) {

                    // Create a new dialog
                    var dt = new dialog({
                        // Dialog title
                        title: "Sitemap generation result ",
                        width: 500,
                        // Create Dialog content
                        content: ("<p><strong>" + response + "</p></strong>")
                    }); 

                    dt.show();
                });
            });
        }
    });
});