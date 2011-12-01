Ext.ns('Tp.util');
Tp.util.validateForNulls = function(args) {
	for (var i = 0; i < args.length; i++) {
		if (args[i] == null){
			throw new Error(String.format('Argument is null. Caller: "{0}"', Tp.util.validateForNulls.caller.toString()));
        }
	}
};

Tp.util.clone = function(o) {
	if (!o || 'object' !== typeof o)
		return o;

	var c = 'function' === typeof o.pop ? [] : {};

	var p, v;

	for (p in o) {
		if (o.hasOwnProperty(p)) {
			v = o[p];

			if (v && 'object' === typeof v)
				c[p] = Tp.util.clone(v);
			else
				c[p] = v;
		}
	}
	return c;
};

Ext.ns('Tp');

Tp.NullObject = function(ot, overrides){
    var nullFunction = function() {        
        return {};
    };
    
    var newType = nullFunction;
    var op = ot.prototype;
    var methods = {};
    for(var p in op){
        if(typeof(op[p]) == 'function')
            methods[p] = nullFunction;
    }

    var type = function(){
        for(var f in methods){
            this[f] = methods[f];
        }

        if(overrides){
            for(var f in overrides){
                this[f] = overrides[f];
            }
        }

    };    
    //Ext.apply(type.prototype, methods);
    //type = Ext.apply(type.prototype, overrides);
    return new type();
};

function nullToString(){
    return '';
}


