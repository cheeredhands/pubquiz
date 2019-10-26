module.exports = {
  configureWebpack: {
    devtool: 'source-map'
  },
  devServer: {
    proxy: {
      // setup a proxy indicating all calls made to /api...
      // should be made to http://localhost:5000.
      '/api': {
        target: 'http://localhost:5000',
        changeOrigin: true
      }
    }
  },
  pluginOptions: {
    i18n: {
      locale: 'nl',             // The locale of project localization
      fallbackLocale: 'en',     // The fallback locale of project localization
      localeDir: 'locales',     // The directory where store localization messages of project
      enableInSFC: false        // Enable locale messages in Single file components
    }
  }
};
