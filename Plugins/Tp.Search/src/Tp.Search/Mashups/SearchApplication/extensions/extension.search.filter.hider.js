tau.mashups
    .addDependency('jQuery')
    .addDependency('Underscore')
    .addDependency('tau/core/extension.base')
    .addDependency('tp/search/services/service.search')
    .addModule('tp/search/extensions/extension.search.filter.hider', function ($, _, ExtensionBase) {
        return ExtensionBase.extend({
            'bus afterRenderAll': function (evtData, afterRenderAllData) {
                var configurator = afterRenderAllData.data.context.configurator;
                var searchServiceInstance = configurator.service('search');
                var $element = afterRenderAllData.element;
                var hideFilterIfNeededHandler = _.bind(this._hideFilterIfNeeded, this, searchServiceInstance, $element);
                var getDelayedVisibilityTracker = function (hideFilterIfNeededHandler) {
                    var timeout;
                    return function () {
                        if (timeout) {
                            clearTimeout(timeout);
                            timeout = null;
                        }
                        timeout = setTimeout(function () {
                            hideFilterIfNeededHandler();
                        }, 200);
                    };
                };

                $element.on('keyup', '.i-role-search-string', getDelayedVisibilityTracker(hideFilterIfNeededHandler));
                $element.on('change', '.i-role-search-string', hideFilterIfNeededHandler);
                hideFilterIfNeededHandler();
            },
            _hideFilterIfNeeded: function (searchServiceInstance, $element) {
                var searchString = _.trim($element.find('.i-role-search-string').val());
                var shouldHideFilter = !searchServiceInstance.getSuitableCommand(searchString).allowsToBeFiltered;
                $element.toggleClass('tau-search-no-filter', shouldHideFilter);
            }
        });
    });
