define(["../codemirror"],function(CodeMirror){CodeMirror.runMode=function(string,modespec,callback){var mode=CodeMirror.getMode({indentUnit:2},modespec),isNode=callback.nodeType==1;if(isNode){var node=callback,accum=[];callback=function(string,style){string=="\n"?accum.push("<br>"):style?accum.push('<span class="cm-'+CodeMirror.htmlEscape(style)+'">'+CodeMirror.htmlEscape(string)+"</span>"):accum.push(CodeMirror.htmlEscape(string))}}var lines=CodeMirror.splitLines(string),state=CodeMirror.startState(mode);for(var i=0,e=lines.length;i<e;++i){i&&callback("\n");var stream=new CodeMirror.StringStream(lines[i]);while(!stream.eol()){var style=mode.token(stream,state);callback(stream.current(),style,i,stream.start),stream.start=stream.pos}}isNode&&(node.innerHTML=accum.join(""))}})