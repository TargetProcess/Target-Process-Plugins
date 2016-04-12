tau.mashups
    .addDependency('tau/mashup.manager/components/component.mashup.manager.library')
    .addDependency('tau/mashup.manager/components/component.mashup.manager.list')
    .addDependency('tau/mashup.manager/components/component.mashup.manager.mashup')
    .addDependency('tau/mashup.manager/components/component.mashup.manager.package')
    .addModule('tau/mashup.manager/components/all.components', function(mashupLibrary, mashupList, mashupEditor, mashupPackage) {
        return [mashupLibrary, mashupList, mashupEditor, mashupPackage];
    });
