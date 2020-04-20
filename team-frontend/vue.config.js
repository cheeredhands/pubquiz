module.exports = {
  configureWebpack: {
    devtool: 'source-map'
  },
  devServer: {
    proxy: {
      // setup a proxy indicating all calls made to /api...
      // should be made to process.env.VUE_APP_BACKEND_URI.
      '/api': {
        target: process.env.VUE_APP_BACKEND_URI,
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
