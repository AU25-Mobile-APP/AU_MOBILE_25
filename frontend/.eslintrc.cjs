/* eslint-env node */
require('@rushstack/eslint-patch/modern-module-resolution');

module.exports = {
  root: true,
  'extends': [
    'plugin:vue/vue3-essential',
    'eslint:recommended',
    '@vue/eslint-config-prettier/skip-formatting'
  ],
  parserOptions: {
    ecmaVersion: 'latest'
  },
  rules: {
    'vue/attribute-hyphenation': ['error', 'always'],
    'vue/block-tag-newline': 'off',
    'vue/component-tags-order': ['error', {
      order: ['template', 'script', 'style'],
    }],
    'vue/html-closing-bracket-newline': 'off',
    'vue/html-indent': ['error', 2, {
      alignAttributesVertically: false,
    }],
    'vue/html-self-closing': [
      "error",
      {
        "html": {
          "void": "never",
          "normal": "never",
          "component": "always"
        },
        "svg": "always",
        "math": "always"
      }
    ],
    'vue/match-component-file-name': 'error',
    'vue/max-attributes-per-line': [
      'error',
      {
        'singleline': 10,
        'multiline': 1
      }
    ],
    'vue/first-attribute-linebreak': [
      'error',
      {
        'singleline': 'ignore',
        'multiline': 'below'
      }
    ],
    'vue/multi-word-component-names': 'off',
    'vue/mustache-interpolation-spacing': ['error', 'always'],
    'vue/no-spaces-around-equal-signs-in-attribute': 'error',
    'vue/order-in-components': 'error',
    'vue/no-multi-spaces': ['error', {
      ignoreProperties: true,
    }],
    'vue/no-mutating-props': ['error', {
      shallowOnly: true,
    }],
    'vue/no-reserved-component-names': 'off',
    'vue/no-setup-props-destructure': 'error',
    'vue/no-static-inline-styles': ['error', {
      allowBinding: true,
    }],
    'vue/no-useless-mustaches': ['error', {
      ignoreIncludesComment: true,
      ignoreStringEscape: true,
    }],
    'vue/singleline-html-element-content-newline': 'off',
    'semi': [2, 'always']
  },
};
