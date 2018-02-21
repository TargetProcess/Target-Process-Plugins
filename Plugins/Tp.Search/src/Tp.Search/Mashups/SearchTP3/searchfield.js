tau.mashups
    .addDependency('jQuery')
    .addDependency('Underscore')
    .addDependency('tau/core/bus.reg')
    .addDependency('tau-intl')
    .addDependency('tau/configurator')
    .addModule('tp/search/searchfield', function ($, _, busRegistry, intl, configurator) {

        var $searchElement = $([
            '<div class="tau-main-search__wrap">',
            '    <form action="" class="tau-main-search__form i-role-search-form">',
            '        <div class="tau-search tau-search--global">',
            '            <div class="tau-search__wrap">',
            '                <input type="text" class="tau-search__input i-role-search-string" placeholder="' + _.escape(intl.formatMessage('Search')) + '">',
            '                <div class="tau-search__icon">',
            '                    <span class="tau-icon-general tau-icon-close-default i-role-close" title="' + _.escape(intl.formatMessage('Close')) + '"></span>',
            '                </div>',
            '            </div>',
            '        </div>',
            '    </form>',
            '</div>'
        ].join(''));

        var $searchInput = $searchElement.find('.i-role-search-string');

        busRegistry.getByName('board.search.placeholder').done(function (bus) {
            bus.once('afterRender', function (eventName, data) {
                if (!configurator.getFeaturesService().isEnabled('search')) return;

                data.element.append($searchElement);
                $searchInput.focus(function () {
                    data.element.addClass('active');
                });
                $searchInput.blur(function () {
                    data.element.removeClass('active');
                });
            }.bind(this));
        }.bind(this));

        return $searchElement;
    });
