Ext.ns('Tp.collections');

Tp.collections.createMixedCollection = function(array) {
    Tp.util.validateForNulls([array]);
    var collection = new Ext.util.MixedCollection();
    collection.addAll(array);
    return collection;
};
