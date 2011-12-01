function RegisterTpCheckBoxList(controlID, checkBoxListID){
	var checkBoxes = GetCheckBoxes(checkBoxListID);
	for (var i = 0; i < checkBoxes.length; i++) {
		var checkBox = Ext.get(checkBoxes[i].id);

		checkBox.on('click', CheckSelection.createCallback(controlID, checkBoxListID));
	}

	CheckSelection(controlID, checkBoxListID);				
}

function CheckSelection(controlID, checkBoxListID)
	{
		var checkedCount = 0;
		var uncheckedCount = 0;
		var disabledCount = 0;
		
		var checkBoxes = GetCheckBoxes(checkBoxListID);
		for (var i = 0; i < checkBoxes.length; i++) 
		{
			var checkBox = checkBoxes[i];

		if(checkBox.style.display == 'none')
		{
			continue;
		}

		if(checkBox.checked)
		{
			checkedCount++;
		}
		else
		{
			uncheckedCount++;
		}

		if(checkBox.disabled)
		{
			disabledCount++;
		}
	}

	if(disabledCount == checkBoxes.length)
	{
		DisableLinks(controlID);
	}
	else
	{
		SetEnablingToLinks(controlID, uncheckedCount > 0, checkedCount > 0);
	}
}

function SetFilterStatuses(controlID, checkBoxListID, isChecked)
{
	var checkBoxes = GetCheckBoxes(checkBoxListID);
	for (var i = 0; i < checkBoxes.length; i++) {
		if(checkBoxes[i].style.display == 'none')
		{
			continue;
		}

		checkBoxes[i].checked = isChecked;
	}

	SetEnablingToLinks(controlID, !isChecked, isChecked);
}

function DisableLinks(controlID)
{
	GetCheckLink(controlID).dom.style.display = 'none';
	GetUnCheckLink(controlID).dom.style.display = 'none';
	GetCheckSpan(controlID).dom.style.display = '';
	GetUnCheckSpan(controlID).dom.style.display = '';
}

function SetEnablingToLinks(controlID, checkEnabling, uncheckEnabling)	
{
	GetCheckLink(controlID).dom.style.display = checkEnabling ? '' : 'none';
	GetUnCheckLink(controlID).dom.style.display = uncheckEnabling ? '' : 'none';
	GetCheckSpan(controlID).dom.style.display = !checkEnabling ? '' : 'none';
	GetUnCheckSpan(controlID).dom.style.display = !uncheckEnabling ? '' : 'none';
}

function GetCheckBoxes(checkBoxListID)
{
	var chbList = document.getElementById(checkBoxListID);
	return chbList.getElementsByTagName('input');
}

function GetCheckLink(controlID)
{
	return Ext.get('checkA' + controlID);
}

function GetUnCheckLink(controlID)
{
	return Ext.get('uncheckA' + controlID);
}

function GetCheckSpan(controlID)
{
	return Ext.get('checkSpan' + controlID);
}

function GetUnCheckSpan(controlID)
{
	return Ext.get('uncheckSpan' + controlID);
}

//=========================================
/*Ext.onReady(function(){
	var checkBoxes = GetCheckBoxes{0}();
	for (var i = 0; i < checkBoxes.length; i++) {
		var checkBox = Ext.get(checkBoxes[i].id);

		checkBox.on('change', CheckSelection{0});		
	}

	CheckSelection{0}();				
	})

	function CheckSelection{0}()
	{
		var checkedCount = 0;
		var uncheckedCount = 0;
		var disabledCount = 0;
		
		var checkBoxes = GetCheckBoxes{0}();
		for (var i = 0; i < checkBoxes.length; i++) 
		{
			var checkBox = checkBoxes[i];

		if(checkBox.style.display == 'none')
		{
			continue;
		}

		if(checkBox.checked)
		{
			checkedCount++;
		}
		else
		{
			uncheckedCount++;
		}

		if(checkBox.disabled)
		{
			disabledCount++;
		}
	}

	if(disabledCount == checkBoxes.length)
	{
		DisableLinks{0}();
	}
	else
	{
		SetEnablingToLinks{0}(uncheckedCount > 0, checkedCount > 0);
	}		
}

function SetFilterStatuses{0}(isChecked)
{
	var checkBoxes = GetCheckBoxes{0}();
	for (var i = 0; i < checkBoxes.length; i++) {
		if(checkBoxes[i].style.display == 'none')
		{
			continue;
		}

		checkBoxes[i].checked = isChecked;
	}		

	SetEnablingToLinks{0}(!isChecked, isChecked);
}

function DisableLinks{0}()
{
	GetCheckLink{0}().dom.style.display = 'none';
	GetUnCheckLink{0}().dom.style.display = 'none';
	GetCheckSpan{0}().dom.style.display = '';
	GetUnCheckSpan{0}().dom.style.display = '';
}

function SetEnablingToLinks{0}(checkEnabling, uncheckEnabling)	
{
	GetCheckLink{0}().dom.style.display = checkEnabling ? '' : 'none';
	GetUnCheckLink{0}().dom.style.display = uncheckEnabling ? '' : 'none';
	GetCheckSpan{0}().dom.style.display = !checkEnabling ? '' : 'none';
	GetUnCheckSpan{0}().dom.style.display = !uncheckEnabling ? '' : 'none';
}

function GetCheckBoxes{0}()
{
	var chbList = document.getElementById('{1}');
	return chbList.getElementsByTagName('input');
}

function GetCheckLink{0}()
{
	return Ext.get('checkA{0}');
}

function GetUnCheckLink{0}()
{
	return Ext.get('uncheckA{0}');
}

function GetCheckSpan{0}()
{
	return Ext.get('checkSpan{0}');
}

function GetUnCheckSpan{0}()
{
	return Ext.get('uncheckSpan{0}');
}*/