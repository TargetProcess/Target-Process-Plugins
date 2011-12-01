function ErrorMessageContainerMock() {
    this._ctor();
}
ErrorMessageContainerMock.prototype = {
    _ctor: function () {
        this.errors = new Array();
    },

    add: function (error) {
        this.errors.push(error);
    },

    addRange: function (errors) {
        var that = this;
        $(errors).each(function (index, error) {
            that.add(error);
        });
    },

    clearErrors: function () {
        this.errors.length = 0;
        this.rendered = false;
    },

    render: function () {
        this.rendered = true;
    },

    setOnFocusEvent: function () { }
};