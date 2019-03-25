module.exports = {
  env: {
    browser: true,
    es6: true,
  },
  parser: "babel-eslint",
  extends: 'airbnb',
  globals: {
    Atomics: 'readonly',
    SharedArrayBuffer: 'readonly',
  },
  parserOptions: {
    ecmaFeatures: {
      jsx: true,
    },
    ecmaVersion: 2018,
    sourceType: 'module',
  },
  plugins: [
    'react',
  ],
  rules: {
    'no-continue': 'off',
    'comma-dangle': 'off',
    'no-plusplus': 'off',
    'import/no-extraneous-dependencies': 'off',
    'jsx-a11y/label-has-associated-control': 'off',
    'jsx-a11y/label-has-for': 'off',
    'class-methods-use-this': 'off',
    'linebreak-style': 'off',
    'global-require': 'off',
    'no-underscore-dangle': 'off',
    'jsx-a11y/anchor-is-valid': 'off',
    'import/no-dynamic-require': 'off',
    'no-tabs': 0,
    'no-mixed-spaces-and-tabs': 'off'
  },
};
