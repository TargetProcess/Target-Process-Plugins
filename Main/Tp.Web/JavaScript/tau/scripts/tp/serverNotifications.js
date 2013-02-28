define([
    'tp/utils',
    'tp/date.utils',
    'Underscore',
    'tpPath!/notifications/hubs'], function (utils, dateUtils, _, $) {
    function Connection() {
        this.started = false;
        this.connection = $.connection;
        this.proxies = {
            'resource':new Proxy(this, 'Resource'),
            'slice':new Proxy(this, 'Slice')
        };
    }

    var disconnectedEvent = 'onDisconnected';
    var connectedEvent = 'onConnected';

    Connection.prototype = {

        start: function (callback) {
            if (this.started) {
                throw new Error('Proxy was already started');
            }
            var that = this;

            $(this).bind(connectedEvent, callback);
            if (this.hasBeenPreviouslyStarted) {
                this.connection.hub.start();
            }
            else {
                this.connection.hub.disconnected(function () {
                    that.started = false;
                    $(that).trigger(disconnectedEvent);
                    $(that).unbind(disconnectedEvent);
                });
                this.connection.hub.start({ transport:'auto' }, function () {
                    that.started = true;
                    that.hasBeenPreviouslyStarted = true;
                    $(that).trigger(connectedEvent);
                    $(that).unbind(connectedEvent);
                });
            }
        },

        stop:function (callback) {
            if (!this.started) {
                throw new Error('Proxy should be started (on stop)');
            }
            var that = this;
            $(this).bind(disconnectedEvent, callback);
            var proxies = _.keys(this.proxies).map(function (key) {
                return that.proxies[key];
            });
            $.when($(proxies).each(function (index, proxy) {
                proxy.unsubscribe(_.map(proxy.subscriptions, function (subscription) {
                    return subscription.id;
                }))
            })).then(function () {
                    that.started = false;
                    that.connection.hub.stop();
                    callback();
                });
        },

        get:function (hub) {
            return this.proxies[hub];
        }
    };

    function Proxy(connection, hub) {
        this.subscriptions = {};
        this.connection = connection;
        this.hub = connection.connection[hub];
        this.hub.client.notifyChanged = $.proxy(this._notifyChanged, this);

        // TODO: for comet debugging purpose only
        if (window['verbose.mode']) {
            this.hub.connection.logging = true;
        }
    }

    Proxy.prototype = {
        subscribe:function (subscriptions, callback) {
            if (!this.connection.started) {
                throw new Error('Proxy should be started (on subscribe)');
            }
            var subscriptionIds = [];
            var complete = utils.countDown(subscriptions.length, callback);
            for (var i = 0; i < subscriptions.length; ++i) {
                var subscriptionId = utils.guid();
                var subscription = subscriptions[i];

                this.subscriptions[subscriptionId] = {
                    id:subscriptionId,
                    clientId:subscription.clientId,
                    parameters:subscription.parameters,
                    callback:subscription.callback,
                    logNotifications:subscription.logNotifications,
                    onSubscribe:complete
                };

                var onDone = (function (s) {
                    return function () {
                        complete();
                    }
                })(this.subscriptions[subscriptionId]);

                this.hub.server.subscribe(this.subscriptions[subscriptionId])
                    .done(onDone)
                    .done(subscription.onSubscribe || function () {});

                subscriptionIds.push(subscriptionId);
            }
            return subscriptionIds;
        },

        unsubscribe:function (subscriptionIds, callback) {
            if (!this.connection.started) {
                throw new Error('Proxy should be started (on unsubscribe)');
            }
            var complete = utils.countDown(subscriptionIds.length, callback);
            if (subscriptionIds.length == 0) {
                complete();
            }
            else {
                for (var i = 0; i < subscriptionIds.length; i++) {
                    var subscriptionId = subscriptionIds[i];
                    this.hub.server.unsubscribe(subscriptionId).done(complete);
                    delete this.subscriptions[subscriptionId];
                }
            }
        },

        _notifyChanged:function (data) {
            var event = {
                sid:data.SubscriptionId,
                timestamp: dateUtils.parseTimestamp(data.Timestamp),
                data:JSON.parse(data.Data)
            };
            var subscription = this.subscriptions[data.SubscriptionId];

            data && data.hasOwnProperty('Trace') && console.log('raw comet: ', data);

            if (subscription && (!subscription.clientId || data.ClientId != subscription.clientId)) {
                subscription.callback(event);
            }
        }
    };

    return new Connection();
});
