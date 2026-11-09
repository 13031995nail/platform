module.exports = {
  root: true,
  parserOptions: {
    ecmaVersion: 2018,
    sourceType: 'module'
  },
  plugins: [
    'ember'
  ],
  extends: [
    'eslint:recommended',
    'plugin:ember/recommended'
  ],
  env: {
    browser: true,
    commonjs: true,
    es6: true
  },
  globals: {
    cordova: 'readonly',
    CameraPreview: 'readonly'
  },
  rules: {
    // Логирование ошибок камеры/распознавания на устройстве - допустимо (можно смотреть через условный logcat в Android Studio)
    'no-console': 'off',
    'ember/no-new-mixins': 'off'
  }
};
