Ext.ns('UseCaseHelp.ToolTipLayout');
 
UseCaseHelp.ToolTipLayout.getInstance = function(tooltip)
{        
         tooltip.show();    
         var layoutHandler;
         var toolTipHeightWithArrow = tooltip.getEl().getHeight() + UseCaseHelp.ImageHelper.getInstance().getImageHeight(tooltip.arrowUpId);
         if(tooltip.target.getXY()[1] < toolTipHeightWithArrow)
          { 
              layoutHandler = new UseCaseHelp.ToolTipLayout.ArrowUpStrategy();
              layoutHandler.initialize(tooltip, tooltip.arrowUpId);
          }  
          else
          {
              layoutHandler = new UseCaseHelp.ToolTipLayout.ArrowDownStrategy();
              layoutHandler.initialize(tooltip, tooltip.arrowDownId);
          }
          tooltip.hide();
          return layoutHandler;
}


UseCaseHelp.ToolTipLayout.Strategy = Ext.extend(Object,{
           
            roundCornerFault:6,
            
            tooltipX:0,
            
            tooltipY:0,
            
            arrowX:0,
            
            arrowY:0,
           
            tooltip:null, 
            
            arrowOffsetHeight:null,
            
            arrowOffsetWidth:null,
            
            helpElementOffsetHeight:0,
            
            scrollX:0,
            
            scrollY:0,
        
            initialize:function(tooltip, arrowId)
            {  
               this.arrowId = arrowId;
               this.tooltip = tooltip;
               this.tooltipOffsetHeight = this.tooltip.getEl().getHeight();
               this.tooltipOffsetWidth = this.tooltip.getEl().getWidth();
               this.arrowOffsetHeight = UseCaseHelp.ImageHelper.getInstance().getImageHeight(this.arrowId);
               this.arrowOffsetWidth = UseCaseHelp.ImageHelper.getInstance().getImageWidth(this.arrowId);
               this.helpElementOffsetHeight = this.tooltip.target.getHeight();
               this.helpElementXY = this.tooltip.target.getXY();
               this.initializeToolTipOffset();
               this.initializeArrowOffset();
               this.initializeScrolling();
            },
            
            
            adjustScrolling:function(elementX, elementY){
          
               var scrollTop = Ext.getBody().getScroll().top;
               var scrollLeft = Ext.getBody().getScroll().left;
               if(elementX < scrollLeft || elementY < scrollTop)
                  window.scrollTo(elementX, elementY); 
            
            },
            
            show:function()
            {   
               this.adjustScrolling(this.scrollX, this.scrollY);
               this.tooltip.showAt([this.tooltipX, this.tooltipY]);
               UseCaseHelp.ImageHelper.getInstance().showImage(this.arrowId,[this.arrowX, this.arrowY]);
           
            },    
            
            dispose:function()
            {  
                UseCaseHelp.ImageHelper.getInstance().hideImage(this.arrowId);
           
            }
      }
)


UseCaseHelp.ToolTipLayout.ArrowDownStrategy = Ext.extend(UseCaseHelp.ToolTipLayout.Strategy , {

        
        initializeToolTipOffset:function()
        {
              if(document.body.offsetWidth < this.helpElementXY[0] + this.tooltipOffsetWidth)
                this.tooltipX  = this.helpElementXY[0] + this.roundCornerFault  - this.tooltipOffsetWidth + this.arrowOffsetWidth;
              else
                 this.tooltipX = this.helpElementXY[0] - this.roundCornerFault;
              this.tooltipY = this.helpElementXY[1] - (this.tooltipOffsetHeight + this.arrowOffsetHeight); 
        },
            
        initializeArrowOffset:function()
        {
               this.arrowX =  this.helpElementXY[0];
               this.arrowY = this.helpElementXY[1] - this.arrowOffsetHeight;
               this.arrowY -= 1;
        },
        
        initializeScrolling:function()
        {
              this.scrollX = this.tooltipX;
              this.scrollY = this.tooltipY;
        }
  }
)



UseCaseHelp.ToolTipLayout.ArrowUpStrategy = Ext.extend(UseCaseHelp.ToolTipLayout.Strategy , {

        initializeToolTipOffset:function()
        {
             this.tooltipX = this.helpElementXY[0];
             if(document.body.offsetWidth < this.tooltipX + this.tooltipOffsetWidth)
                this.tooltipX  = this.helpElementXY[0] + this.roundCornerFault - this.tooltipOffsetWidth + this.arrowOffsetWidth;
             else
               this.tooltipX = this.helpElementXY[0] - this.roundCornerFault;
             this.tooltipY = this.helpElementXY[1] + this.helpElementOffsetHeight + this.arrowOffsetHeight; 
        },
            
        initializeArrowOffset:function()
        {
           this.arrowX = this.helpElementXY[0]; 
           this.arrowY = this.helpElementXY[1] + this.helpElementOffsetHeight;
           this.arrowY += 1;
        
        },
        
        initializeScrolling:function()
        {
           this.scrollX = this.helpElementXY[0];
           this.scrollY = this.helpElementXY[1];
        }
 }
)
