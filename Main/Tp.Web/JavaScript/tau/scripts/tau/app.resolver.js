define(["Underscore"],function(_){var analyzeAppToken=function(token){var parts=token.split("/");if(parts.length<2)return null;var pageType=parts[0],tail=parts[1],length=tail.length,specSymbolsIndices=_.without([tail.indexOf("&"),tail.indexOf("||"),length],-1),rightBound=_.min(specSymbolsIndices),pageId=tail.substring(0,rightBound);return{id:pageId,type:pageType}};return{resolvePageInfo:function(config,defaultPageInfo,delegate){var ext=config.configurator.getExternal(),targetValue=ext.getHashParam(config.appId,null),appIdParamValue=targetValue||ext.getHash(),params=analyzeAppToken(appIdParamValue),result=params;return params||(result=defaultPageInfo,_.isFunction(delegate)&&delegate.call(ext,result)),result},getQueryParam:function(queryString,paramName){var start=queryString.indexOf(paramName+"=");if(start===-1)throw'Param "'+paramName+'" not found';start=start+paramName.length+1;var end=queryString.indexOf("&",start);end=end!==-1?end:queryString.length;var paramValue=queryString.substring(start,end);return paramValue}}})