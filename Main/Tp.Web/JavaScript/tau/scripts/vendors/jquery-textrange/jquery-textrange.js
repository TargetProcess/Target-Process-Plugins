!function(t){"function"==typeof define&&define.amd?define(["jquery"],t):t("object"==typeof exports?require("jquery"):jQuery)}(function(t){var e,n={get:function(t){return i[e].get.apply(this,[t])},set:function(t,n){var s,r=parseInt(t),o=parseInt(n);return"undefined"==typeof t?r=0:0>t&&(r=this[0].value.length+r),"undefined"!=typeof n&&(s=n>=0?r+o:this[0].value.length+o),i[e].set.apply(this,[r,s]),this},setcursor:function(t){return this.textrange("set",t,0)},replace:function(t){return i[e].replace.apply(this,[String(t)]),this},insert:function(t){return this.textrange("replace",t)}},i={xul:{get:function(t){var e={position:this[0].selectionStart,start:this[0].selectionStart,end:this[0].selectionEnd,length:this[0].selectionEnd-this[0].selectionStart,text:this.val().substring(this[0].selectionStart,this[0].selectionEnd)};return"undefined"==typeof t?e:e[t]},set:function(t,e){"undefined"==typeof e&&(e=this[0].value.length),this[0].selectionStart=t,this[0].selectionEnd=e},replace:function(t){var e=this[0].selectionStart,n=this[0].selectionEnd,i=this.val();this.val(i.substring(0,e)+t+i.substring(n,i.length)),this[0].selectionStart=e,this[0].selectionEnd=e+t.length}},msie:{get:function(t){var e=document.selection.createRange();if("undefined"==typeof e){var n={position:0,start:0,end:this.val().length,length:this.val().length,text:this.val()};return"undefined"==typeof t?n:n[t]}var i=0,s=0,r=this[0].value.length,o=this[0].value.replace(/\r\n/g,"\n"),a=this[0].createTextRange(),l=this[0].createTextRange();a.moveToBookmark(e.getBookmark()),l.collapse(!1),-1===a.compareEndPoints("StartToEnd",l)?(i=-a.moveStart("character",-r),i+=o.slice(0,i).split("\n").length-1,-1===a.compareEndPoints("EndToEnd",l)?(s=-a.moveEnd("character",-r),s+=o.slice(0,s).split("\n").length-1):s=r):(i=r,s=r);var n={position:i,start:i,end:s,length:r,text:e.text};return"undefined"==typeof t?n:n[t]},set:function(t,e){var n=this[0].createTextRange();if("undefined"!=typeof n){"undefined"==typeof e&&(e=this[0].value.length);
var i=t-(this[0].value.slice(0,t).split("\r\n").length-1),s=e-(this[0].value.slice(0,e).split("\r\n").length-1);n.collapse(!0),n.moveEnd("character",s),n.moveStart("character",i),n.select()}},replace:function(t){document.selection.createRange().text=t}}};t.fn.textrange=function(i){return"undefined"==typeof this[0]?this:("undefined"==typeof e&&(e="selectionStart"in this[0]?"xul":document.selection?"msie":"unknown"),"unknown"===e?this:(document.activeElement!==this[0]&&this[0].focus(),"undefined"==typeof i||"string"!=typeof i?n.get.apply(this):"function"==typeof n[i]?n[i].apply(this,Array.prototype.slice.call(arguments,1)):void t.error("Method "+i+" does not exist in jQuery.textrange")))}});