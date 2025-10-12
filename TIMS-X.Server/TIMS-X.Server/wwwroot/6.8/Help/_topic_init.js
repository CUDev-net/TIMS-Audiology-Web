

$(function() {
    // Create the app
    var app = new Hnd.App({
      searchEngineMinChars: 4
    });
    // Update translations
    hnd_ut(app);
    // Instanciate imageMapResizer
    imageMapResize();
    // Custom JS
    
    // Boot the app
    app.Boot();
});