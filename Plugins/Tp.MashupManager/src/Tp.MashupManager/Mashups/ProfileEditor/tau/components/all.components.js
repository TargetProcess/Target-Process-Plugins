define([
    'tau/mashup.manager/components/component.mashup.manager.library'
    , 'tau/mashup.manager/components/component.mashup.manager.list'
    , 'tau/mashup.manager/components/component.mashup.manager.mashup'
    , 'tau/mashup.manager/components/component.mashup.manager.package'
], function(mashupLibrary, mashupList, mashupEditor, mashupPackage) {
    return [mashupLibrary, mashupList, mashupEditor, mashupPackage];
});
