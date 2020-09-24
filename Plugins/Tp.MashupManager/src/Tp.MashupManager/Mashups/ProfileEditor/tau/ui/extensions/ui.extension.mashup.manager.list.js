tau.mashups
    .addDependency('Underscore')
    .addDependency('jQuery')
    .addDependency('tau/core/extension.base')
    .addDependency('tau/mashup.manager/utils/utils.mashup.manager.actionType')
    .addDependency('jquery.ui.confirmation')
    .addModule('tau/mashup.manager/ui/extensions/ui.extension.mashup.manager.list',
        function(_, $, ExtensionBase, actionType) {
            return ExtensionBase.extend({
                'bus afterInit + afterRender': function(evtData, initConfig, renderData) {
                    var $element = renderData.element;
                    var context = initConfig.config.context;
                    this._service = context.configurator.service('mashup.manager');
                    var navigator = context.configurator.service('navigator');

                    this._bindDelete($element, navigator);
                    this._bindAdd($element, navigator);
                    this._bindRefresh();
                    this._bindRedirect(navigator);
                    this._showLibrary($element);

                    switch (context.actionData.actionType) {
                        case actionType.library:
                            this._selectLibrary($element);
                            break;
                        case actionType.updateMashup:
                            this._selectMashup($element, context.actionData.mashupName);
                            break;
                    }
                },

                _bindDelete: function($element, navigator) {
                    var mashupManagerService = this._service;

                    $element.on('click', '.i-role-deleteMashup', function(e) {
                        e.preventDefault();
                        // in context of event callback 'this' is a DOM node where click listener is registered
                        // in our case it is a clicked button with i-role-deleMashup class
                        var deleteBlock = $(this);
                        deleteBlock.confirmation({
                            message: '',
                            okLabel: 'OK',
                            cancelLabel: 'Cancel',
                            ok: function() {
                                deleteBlock.confirmation('hideConfirmationMessage');
                                mashupManagerService.deleteMashup(deleteBlock.closest('a').data('mashupname')).done(
                                    function() {
                                        navigator.to('library');
                                    });
                            },
                            cancel: function() {
                                deleteBlock.confirmation('hide');
                            }
                        });
                        deleteBlock.confirmation('show');
                    });
                },

                _bindAdd: function($element, navigator) {
                    $element.on('click', '.i-role-addMashup', function() {
                        navigator.to('add');
                    });
                },

                _bindRefresh: function() {
                    this._service.on(['packageInstalled', 'mashupDeleted'], this._refresh, this);
                },

                _bindRedirect: function(navigator) {
                    var redirect = function(evtData) {
                        this._redirectToMashup(navigator, evtData.data.Name);
                    }.bind(this);
                    this._service.on(['mashupAdded', 'mashupUpdated'], redirect, this);
                },

                _showLibrary: function($element) {
                    var $mashupLibraryItem = $element.find('.i-role-libraryItem');
                    $mashupLibraryItem.show();
                },

                _selectLibrary: function($element) {
                    var $mashupLibrary = $element.find('.i-role-libraryButton');
                    this._selectItem($element, $mashupLibrary);
                },

                _selectMashup: function($element, mashupName) {
                    var $mashupItem = $element.find('[data-mashupName="' + mashupName + '"]');
                    this._selectItem($element, $mashupItem);
                },

                _selectItem: function($element, $mashupItem) {
                    if ($mashupItem.hasClass('selected')) {
                        return;
                    }
                    $element.find('a.mashupName').removeClass('selected');
                    $mashupItem.addClass('selected');
                },

                _refresh: function() {
                    this.fire('refresh');
                },

                _redirectToMashup: function(navigator, mashupName) {
                    navigator.to('mashups/' + mashupName);
                },

                destroy: function() {
                    if (this._service) {
                        this._service.removeAllListeners(this);
                    }
                    this._super();
                }
            });
        }
    );
