define([],function(){return{create:function(c){var navigator=c.navigator;return{dblclick:function(r){navigator.to(r.type+"/"+r.id)}}}}})