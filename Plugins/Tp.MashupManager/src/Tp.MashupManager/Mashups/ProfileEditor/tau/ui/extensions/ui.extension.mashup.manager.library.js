tau.mashups
    .addDependency('jQuery')
    .addDependency('tau/core/extension.base')
    .addDependency('tau/ui/behaviour/common/ui.behaviour.progressIndicator')
    .addModule('tau/mashup.manager/ui/extensions/ui.extension.mashup.manager.library',
        function($, ExtensionBase, ProgressIndicator) {
            return ExtensionBase.extend({
                'bus afterInit + afterRender' : function(evtData, initConfig, renderData){
                    var $element = renderData.element;
                    var service = initConfig.config.context.configurator.service('mashup.manager');
                    this._bindRefresh($element, service);
                },
                'bus refresh > afterRender': function(evtData, refreshData, renderData){
                    ProgressIndicator.get(renderData.element.closest('.i-role-mashupDetails')).hide();
                },
                _bindRefresh: function($element, service){
                    $element.on('click', '.i-role-libraryRefresh', _.bind(function(mashupManagerService){
                        var $refreshLink = $element.find('.i-role-libraryRefresh');
                        $refreshLink.attr('disabled', 'disabled');
                        $refreshLink.addClass('library-refresh-disabled');
                        $element.find('.i-role-library').remove();
                        ProgressIndicator.get($element.closest('.i-role-mashupDetails')).show();
                        mashupManagerService.refreshLibrary()
                            .done(_.bind(function(){
                                this.fire('refresh');
                            }, this))
                            .fail(_.bind(function(){
                                this.fire('refresh');
                            }, this));
                    }, this, service));
                }
            });
        }
    );
