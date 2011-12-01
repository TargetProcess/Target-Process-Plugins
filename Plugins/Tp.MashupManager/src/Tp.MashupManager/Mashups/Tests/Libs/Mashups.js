var tau = tau || {};

tau.mashups = (function () {

    function Mashups() { }

    Mashups.prototype = {
        currentPlaceholder: null,
        libs: [],
        mashups: [],

        setPlaceholder: function (placeholder) {
            this.currentPlaceholder = placeholder;
        },

        addMashup: function (func) {
            var id = this.currentPlaceholder;
            var libs = this.libs;
            this.libs = [];
            require.ready(
                function () {
                    require(libs,
                    function () {
                        var args = Array.prototype.slice.call(arguments);
                        args.push({ 'placeholderId': id });
                        return func.apply(null, args);
                    }
                 );
                }
            );
        },

        addDependency: function (moduleName) {
            this.libs.push(moduleName);
            return this;
        },

        addModule: function (name, module) {
            var libs = this.libs;
            this.libs = [];
            define(name, libs,
                function () {
                    if (typeof(module) !== 'function')
                        return module;

                    var args = Array.prototype.slice.call(arguments);
                    return module.apply(null, args);
                }
            );
        }
    }
    return new Mashups();
})();
