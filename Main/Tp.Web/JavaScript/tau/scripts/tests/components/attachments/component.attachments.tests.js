define(["tau/core/class","tau/core/tau","tau/components/component.attachments","tau/components/component.attachmentsPreview","tests/common/testData","tests/components/common/common.setup","tests/components/component.specs"],function(a,b,c,d,e,f,g){var h=function(){function m(a,b){equal(a.find(".preview img").attr("src"),b.thumbnailUri,"thumbnailUrl is valid is valid"),equal(a.find(".name").text(),b.name,"Name is valid"),equal(a.find(".owner").text(),"by "+b.ownerName,"Owner is valid"),equal(a.find(".date").text(),"on "+b.date,"Date is valid"),equal(a.find(".toolbar a.download").attr("href"),b.uri,"URL to download attachment is valid")}function l(a){for(var b=0;b<a.length;b++){var c=a.eq(b),d=c.find(".ui-actions").find(".ui-actions-panel"),e=d.css("display")!=="none";ok(!e,"Action panel is hidden on "+b+" element")}}function k(a,b){for(var c=0;c<a.length;c++){var d=a.eq(c),e=d.find(".ui-actions").find(".ui-actions-panel"),f=e.css("display")!=="none";c===b?ok(f,"Action panel is visible on "+c+" element"):ok(!f,"Action panel is hidden on "+c+" element")}}var a=e.getDataForAttachments(),b=a[0],d={context:{type:"bug",id:b.id}},h={context:{type:"bug",id:b.id}},i=[Similar.to({name:"get",type:"bug",config:Like.is({id:b.id,fields:["id",Similar.to({attachments:["id","date","name","mimeType","uri","thumbnailUri",Similar.to({owner:["id","firstName","lastName"]})]})]})})],j={items:[{id:550,name:"Attachment1",mimeType:"image/jpg",ownerName:"John Brown",date:"4/12/2011",thumbnailUri:"#../../images/thumbnail_1.jpg",uri:"#../../images/1.jpg"},{id:551,name:"Attachment3",mimeType:"application/msword",ownerName:"Tod Black",date:"6/9/2011",thumbnailUri:"#../../images/thumbnail_2.jpg",uri:"#../../images/2.doc"},{id:552,name:"Attachment3",mimeType:"image/jpg",ownerName:"Andrew Gray",date:"6/9/2011",thumbnailUri:"#../../images/thumbnail_3.jpg",uri:"#../../images/3.jpg"},{id:553,name:"Attachment4",mimeType:"image/jpg",ownerName:"Andrew Gray",date:"6/9/2011",thumbnailUri:"#../../images/thumbnail_4.jpg",uri:"#../../images/4.jpg"}]},n=[{name:"should render valid markup",test:function(){var a=this.$el,b=a.find(".ui-attachments-content > .ui-attach"),c=this.data.items;equal(b.length,c.length,"Count of root comments is valid");for(var d=0;d<c.length;d++)m(b.eq(0),c[0])}},{name:"should render confirmation overlay on delete action",test:function(){var a=this.$el,b=a.find(".ui-attachments-content .ui-attach").first(),c=b.find(".toolbar .delete");c.click(),c.click(),c.click();var d=a.find(".confirmation");equal(d.size(),1,"Confirmation is rendered correctly")}}],o=f.create("[component.attachments]",a,c);g.create(o,d).viewShouldFollowDataComponentLifeCycle().modelShouldReturnData(j,[i,{_operation:"on",command:Similar.to({eventName:"afterRemove",type:"attachment"})}]).viewShouldPassTests(n).done()};return{run:h}})