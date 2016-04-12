tau.mashups
    .addDependency("libs/jquery/jquery")
    .addModule("emailIntegration/jquery/defaultvalue.source",

function (jQuery) {
/**
*	@name							Defaultvalue
*	@descripton						Gives value to empty inputs
*	@version						1.4.2
*	@requires						Jquery 1.3.2
*
*	@author							Jan Jarfalk
*	@author-email					jan.jarfalk@unwrongest.com
*	@author-twitter					janjarfalk
*	@author-website					http://www.unwrongest.com
*
*	@licens							MIT License - http://www.opensource.org/licenses/mit-license.php
*
*	@param {Function} callback		Callback function
*/

(function($){
     $.fn.extend({
         defaultValue: function(callback) {

			var nativePlaceholderSupport = (function(){
				var i = document.createElement('input');
				return ('placeholder' in i);
			})();
				
			// Default Value will halt here if the browser
			// has native support for the placeholder attribute
			if(nativePlaceholderSupport){
				//return false;
			}
			
            return this.each(function(index, element) {
				
				// Executing Default Value twice on an element will lead to trouble
				if($(this).data('defaultValued')){
					return false;
				}
				
				var $input				=	$(this),
					defaultValue		=	$input.attr('hint');
				var	callbackArguments 	=	{'input':$input};
				
				// Mark as defaultvalued
				$input.data('defaultValued', true);
					
				// Create clone and switch
				var $clone = createClone();
				
				// Add clone to callback arguments
				callbackArguments.clone = $clone;
				
				$clone.insertAfter($input);
				
				var setState = function() {
					if( $input.val().length <= 0 ){
						$clone.show();
						$input.hide();
					} else {
						$clone.hide();
						$input.show().trigger('click');
					}
				};
				
				// Events for password fields
				$input.bind('blur', setState);
				
				// Create a input element clone
				function createClone(){
					
					var $el;
					
					if($input.context.nodeName.toLowerCase() == 'input') {
						$el = $("<input />").attr({
							'type'	: 'text'
						});
					} else if($input.context.nodeName.toLowerCase() == 'textarea') {
						$el = $("<textarea />");	
					} else {
						throw 'DefaultValue only works with input and textareas'; 
					}
					
					$el.attr({
						'id'		: $input.attr('id') + "clone",
						'value'		: defaultValue,
						'class'		: $input.attr('class'),
						'size'		: $input.attr('size'),
						'style'		: $input.attr('style') + ';color:#666;',
						'tabindex' 	: $input.attr('tabindex'),
						'rows' 		: $input.attr('rows'),
						'cols'		: $input.attr('cols'),
						'name'		: $input.attr('name')
					});
					
					$el.focus(function(){
					
						// Hide text clone and show real password field
						$el.hide();
						$input.show();
						
						// Webkit and Moz need some extra time
						// BTW $input.show(0,function(){$input.focus();}); doesn't work.
						setTimeout(function () {
							$input.focus();
						}, 1);
					
					});				
					
					return $el;
				}

				setState();
				
				if(callback){
					callback(callbackArguments);
				}	
				
            });
        }
    });
    })(jQuery)

});
