tau.mashups
    .addDependency("libs/jquery/jquery")
    .addDependency("libs/jquery/jquery.tmpl")
    .addModule("Bugzilla/MappingTemplates", function () {
        var batchedLineTemplateName = 'batched-mapping-line-template';
        var batchedMappingTemplateName = 'batched-mapping-template';
        
        $.template(batchedLineTemplateName, '<ul class="mapping-block">\n    <li class="map-key"><label class="externalvalue" value="${Key}">${Key}</label></li>\n    <li class="chain"></li>\n    <li>\n        <select class="tpvalue select">\n            <option value="0">-Select-</option>\n            {{each Value}}\n            <option value="${Id}" {{if Selected}}selected="selected"{{/if}}>${Name}</option>\n            {{/each}}\n        </select>\n    </li>\n</ul>');
        $.template(batchedMappingTemplateName, '<div class="inner-content">\n    <p class="note"><span class="small">{{html Description}}</span></p>\n    <ul class="mapping-block">\n        <li class="map-key"><p class="label pt-10">${KeyName}</p></li>\n        <li class="chain-no"></li>\n        <li><p class="label pt-10">${ValueName}</p></li>\n    </ul>\n    <div class=\'mappings-blocks\'></div>\n    <p class="note"><span class="small">{{html HowTo}}</span> </p>\n</div>');

        var standaloneLineTemplateName = 'standalone-mapping-line-template';
        var standaloneMappingTemplateName = 'standalone-mapping-template';

        $.template(standaloneLineTemplateName, '<ul class="mapping-block">\n    <li class="map-key"><label class="externalvalue" value="${Key}">${Key}</label></li>\n    <li class="chain"></li>\n    <li>\n        <select class="tpvalue select">\n            <option value="0">-Select-</option>\n            {{each Value}}\n            <option value="${Id}" {{if Selected}}selected="selected"{{/if}}>${Name}</option>\n            {{/each}}\n        </select>\n    </li>\n</ul>');
        $.template(standaloneMappingTemplateName, '<div class="bugzilla-map">\n    <div class="pad-box">\n        <h3 class="h3">${Caption}</h3>\n        <p class="label"><span name="${Key}ErrorLabel" class="error"/></p>\n        <p class="note"><span class="small">{{html Description}}</span></p>\n        <ul class="mapping-block">\n            <li class="map-key"><p>${KeyName}</p></li>\n            <li class="chain-no"></li>\n            <li><p class="label pt-10">${ValueName}</p></li>\n        </ul>\n        <div class=\'mappings-blocks\'></div>\n    </div>\n</div>');

        return {
            "batched": {lineTemplateName: batchedLineTemplateName, mappingTemplateName: batchedMappingTemplateName},
            "standalone": {lineTemplateName: standaloneLineTemplateName, mappingTemplateName: standaloneMappingTemplateName}
        };
    });
