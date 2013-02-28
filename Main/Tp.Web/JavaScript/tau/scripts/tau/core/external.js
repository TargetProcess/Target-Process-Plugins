define(["Underscore","jQuery","tau/core/class","libs/json2"],function(_,$,Class){var DEFAULT_HASH_ENCODER={encode:function(value){var v=value;return _.isObject(value)&&(v=JSON.stringify(value)),v},decode:function(str){var v=str;return v.charAt(0)==="{"&&v.charAt(v.length-1)==="}"&&(v=JSON.parse(v)),v}},mergeHashParams=function(hash,id,value){var self=this;hash=hash.charAt(0)!=="&"?"&"+hash:hash;var index=hash.indexOf("&"+id+"="),left=index!==-1?index:hash.length;index=hash.indexOf("&",1+left);var right=index!==-1?index:hash.length,serializedParam="";if(value!==null){var v=self.paramsEncoder.encode(value),prefix=hash==="&"?"":"&";serializedParam=prefix+id+"="+v}var head=hash.substring(0,left),tail=hash.substring(right),newHashString=head+serializedParam+tail;return newHashString=newHashString.charAt(0)!=="&"?newHashString:newHashString.substring(1),newHashString},tryDecodeUri=function(hash){var r=hash;try{r=decodeURIComponent(hash)}catch(e){}return r};return Class.extend({init:function(external,paramsEncoder){this.handlersRegistry=[],this.external=_.defaults(external,{location:{hash:""}}),this.paramsEncoder=paramsEncoder||DEFAULT_HASH_ENCODER},getHash:function(){var hash=this.external.location.hash;while(hash.indexOf("#")===0)hash=hash.substr(1);return tryDecodeUri(hash)},getHref:function(){return this.external.location.href},setHash:function(hashStr,forceReplace){forceReplace?this.external.location.replace("#"+hashStr):this.external.location.hash="#"+hashStr},hashParams:function(){var result={},hash=this.getHash(),parts=hash.split("&");for(var i=0,len=parts.length;i<len;i++){var token=parts[i],pos=token.indexOf("=");if(pos!==-1){var n=token.substr(0,pos),v=token.substr(pos+1);result[n]=this.paramsEncoder.decode(v)}}return result},getHashParam:function(id,defaultValue){var hash=this.hashParams(),result=hash[id]||(arguments.length>1?defaultValue:{});return result},setHashParam:function(id,value,forceReplace){var hash=this.getHash(),newHashString=mergeHashParams.call(this,hash,id,value);this.setHash(newHashString,forceReplace)},applyHashObject:function(obj,forceReplace){var self=this,newHashString=this.getHash();_.each(obj,function(val,id){newHashString=mergeHashParams.call(this,newHashString,id,val)},self),self.setHash(newHashString,forceReplace)},unbindHashChange:function(handler){var self=this,matchingPredicate=arguments.length===0?function(item){return!0}:function(item){return item.src===handler},$external=$(self.external);for(var i=0;i<self.handlersRegistry.length;){var item=self.handlersRegistry[i];matchingPredicate(item)?($external.unbind("hashchange",item.wrapper),self.handlersRegistry.splice(i,1)):++i}},createHashChangeHandler:function(handler){var self=this,h=function(e){handler(self)};return self.handlersRegistry.push({src:handler,wrapper:h}),h},onHashChange:function(handler){var self=this,hashChangeHandler=self.createHashChangeHandler(handler);$(this.external).bind("hashchange",hashChangeHandler)},triggerHashChange:function(hash){$(this.external).trigger("hashchange",{hash:hash})}})})