(function(){function stopEvent(){this.preventDefault?(this.preventDefault(),this.stopPropagation()):(this.returnValue=!1,this.cancelBubble=!0)}function connect(node,type,handler){function wrapHandler(event){handler(event||window.event)}typeof node.addEventListener=="function"?node.addEventListener(type,wrapHandler,!1):node.attachEvent("on"+type,wrapHandler)}CodeMirror.simpleHint=function(editor,getHints){function insert(str){editor.replaceRange(str,result.from,result.to)}function close(){if(done)return;done=!0,complete.parentNode.removeChild(complete)}function pick(){insert(completions[sel.selectedIndex]),close(),setTimeout(function(){editor.focus()},50)}if(editor.somethingSelected())return;var result=getHints(editor);if(!result||!result.list.length)return;var completions=result.list;if(completions.length==1)return insert(completions[0]),!0;var complete=document.createElement("div");complete.className="CodeMirror-completions";var sel=complete.appendChild(document.createElement("select"));window.opera||(sel.multiple=!0);for(var i=0;i<completions.length;++i){var opt=sel.appendChild(document.createElement("option"));opt.appendChild(document.createTextNode(completions[i]))}sel.firstChild.selected=!0,sel.size=Math.min(10,completions.length);var pos=editor.cursorCoords();complete.style.left=pos.x+"px",complete.style.top=pos.yBot+"px",document.body.appendChild(complete),completions.length<=10&&(complete.style.width=sel.clientWidth-1+"px");var done=!1;return connect(sel,"blur",close),connect(sel,"keydown",function(event){var code=event.keyCode;code==13?(stopEvent(event),pick()):code==27?(stopEvent(event),close(),editor.focus()):code!=38&&code!=40&&(close(),editor.focus(),setTimeout(function(){CodeMirror.simpleHint(editor,getHints)},50))}),connect(sel,"dblclick",pick),sel.focus(),window.opera&&setTimeout(function(){done||sel.focus()},100),!0}})()