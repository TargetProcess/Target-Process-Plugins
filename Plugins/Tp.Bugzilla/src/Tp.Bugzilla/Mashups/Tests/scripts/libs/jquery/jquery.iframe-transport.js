define(["libs/jquery/jquery"],function(a){(function(a){var b=0;a.ajaxTransport("iframe",function(c,d,e){if(c.type==="POST"||c.type==="GET"){var f,g;return{send:function(d,e){f=a('<form style="display:none;"></form>'),g=a('<iframe src="javascript:false;" name="iframe-transport-'+(b+=1)+'"></iframe>').bind("load",function(){var b;g.unbind("load").bind("load",function(){var b;try{b=g.contents();if(!b.length||!b[0].firstChild)throw new Error}catch(c){b=undefined}e(200,"success",{iframe:b}),a('<iframe src="javascript:false;"></iframe>').appendTo(f),f.remove()}),f.prop("target",g.prop("name")).prop("action",c.url).prop("method",c.type),c.formData&&a.each(c.formData,function(b,c){a('<input type="hidden"/>').prop("name",c.name).val(c.value).appendTo(f)}),c.fileInput&&c.fileInput.length&&c.type==="POST"&&(b=c.fileInput.clone(),c.fileInput.after(function(a){return b[a]}),c.paramName&&c.fileInput.each(function(){a(this).prop("name",c.paramName)}),f.append(c.fileInput).prop("enctype","multipart/form-data").prop("encoding","multipart/form-data")),f.submit(),b&&b.length&&c.fileInput.each(function(c,d){var e=a(b[c]);a(d).prop("name",e.prop("name")),e.replaceWith(d)})}),f.append(g).appendTo("body")},abort:function(){g&&g.unbind("load").prop("src","javascript".concat(":false;")),f&&f.remove()}}}}),a.ajaxSetup({converters:{"iframe text":function(a){return a.text()},"iframe json":function(b){return a.parseJSON(b.text())},"iframe html":function(a){return a.find("body").html()},"iframe script":function(b){return a.globalEval(b.text())}}})})(a);return{}})