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
      locale: 'nl',
      fallbackLocale: 'en',
      localeDir: 'locales',
      enableInSFC: false
    }
  }
};
