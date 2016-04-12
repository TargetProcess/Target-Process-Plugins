tau.mashups
    .addDependency('jQuery')
    .addDependency('tau/core/extension.base')
    .addDependency('tau/mashup.manager/utils/utils.mashup.manager.actionType')
    .addDependency('jquery.ui.confirmation')
    .addModule('tau/mashup.manager/ui/extensions/ui.extension.mashup.manager.list',
        function($, ExtensionBase, actionType) {
            return ExtensionBase.extend({
                'bus afterInit + afterRender': function(evtData, initConfig, renderData){
                    var $element = renderData.element;
                    var service = initConfig.config.context.configurator.service('mashup.manager');
                    var navigator = initConfig.config.context.configurator.service('navigator');
                    this._bindDelete($element, service, navigator);
                    this._bindAdd($element, navigator);
                    this._bindRefresh(service);
                    this._bindRedirect(service, navigator);
                    if (initConfig.config.context.actionData.libraryIsAllowed){
                        this._showLibrary($element);
                    }
                    switch (initConfig.config.context.actionData.actionType){
                        case actionType.library:
                            this._selectLibrary($element);
                            break;
                        case actionType.updateMashup:
                            this._selectMashup($element, initConfig.config.context.actionData.mashupName);
                            break;
                    }
                },
                _bindDelete: function($element, service, navigator){
                    $element.on('click', '.i-role-deleteMashup', _.bind(function(mashupManagerService, navigatorService, e){
                        e.preventDefault();
                        var $target = $(e.target);
                        if($target.is('span')){
                            $target = $target.parent();
                        }
                        var deleteBlock = $target.parent();
                        deleteBlock.confirmation({
                            message: '',
                            okLabel: 'OK',
                            cancelLabel: 'Cancel',
                            ok: function() {
                                deleteBlock.confirmation('hideConfirmationMessage');
                                mashupManagerService.deleteMashup($target.closest('a').data('mashupname')).done(function(){
                                    navigator.to('library');
                                });
                            },

                            cancel: function() {
                                deleteBlock.confirmation('hide');
                            }
                        });
                        deleteBlock.confirmation('show');
                    }, this, service, navigator));
                },
                _bindAdd: function($element, navigator){
                    $element.on('click', '.i-role-addMashup', _.bind(function(navigatorService){
                        navigatorService.to('add');
                    }, this, navigator));
                },
                _bindRefresh: function(service){
                    service.on('packageInstalled', this._refresh, this);
                    service.on('mashupDeleted', this._refresh, this);
                },
                _bindRedirect: function(service, navigator){
                    var redirect = _.bind(function(navigator, evtData){
                        this._redirectToMashup(navigator, evtData.data.Name);
                    }, this, navigator);
                    service.on('mashupAdded', redirect, this);
                    service.on('mashupUpdated', redirect, this);
                },
                _showLibrary: function($element){
                    var $mashupLibraryItem = $element.find('.i-role-libraryItem');
                    $mashupLibraryItem.show();
                },
                _selectLibrary: function($element){
                    var $mashupLibrary = $element.find('.i-role-libraryButton');
                    this._selectItem($element, $mashupLibrary);
                },
                _selectMashup: function($element, mashupName){
                    var $mashupItem = $element.find('[data-mashupName="' + mashupName + '"]');
                    this._selectItem($element, $mashupItem);
                },
                _selectItem: function($element, $mashupItem){
                    if ($mashupItem.hasClass('selected')){
                        return;
                    }
                    $element.find('a.mashupName').removeClass('selected');
                    $mashupItem.addClass('selected');
                },
                _refresh: function(){
                    this.fire('refresh');
                },
                _redirectToMashup: function(navigator, mashupName){
                    navigator.to('mashups/' + mashupName);
                }
            });
        }
    );
