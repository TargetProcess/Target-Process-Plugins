tau.mashups
    .addDependency("MashupManager/MashupManagerEditor")
    .addDependency("libs/jquery/jquery")
    .addMashup(function (editor, $, config) {
        var placeholder = $('#' + config.placeholderId);
        new editor({ placeholder: placeholder }).initialize();
    });