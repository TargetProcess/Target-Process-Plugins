tau.mashups
    .addDependency("BugzillaListDetails/IconViewer")
    .addDependency("libs/jquery/jquery")
    .addDependency("libs/jquery/jquery.tmpl")
    .addDependency("libs/jquery/jquery.ui")
    .addMashup(function (viewer) {

        function bugzillaBugIcon(config) {
            this._create(config);
        }

        bugzillaBugIcon.prototype = {
            viewer: null,

            _create: function(config){
                this.viewer = config.viewer;
            },

            render: function(){
                this.viewer.drawBugIcons();
            }
        };

        new bugzillaBugIcon({
            viewer: new viewer({})
        }).render();
    })