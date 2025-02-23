import globals from 'globals';
import pluginJs from '@eslint/js';
import tseslint from 'typescript-eslint';
import pluginReact from 'eslint-plugin-react';

/** @type {import('eslint').Linter.Config[]} */
export default [
  { 
    files: ['**/*.{js,mjs,cjs,ts,jsx,tsx}'],
    ignores: ['node_modules', 'dist', 'build'],
  },
  { 
    languageOptions: { globals: globals.browser } 
  },
  pluginJs.configs.recommended,
  ...tseslint.configs.recommended,
  pluginReact.configs.flat.recommended,
  {
    settings: {
      react: {
        version: 'detect' // Automatically detects React version
      }
    },
    rules: {
      'quotes': ['error', 'single'], // Enforce single quotes
      'semi': ['error', 'always'], // Require semicolons at the end of lines
      'no-restricted-imports': [
        'error',
        {
          patterns: ['../*', './*'], // Restricts relative imports that do not start with '@'
        },
      ],
    },
  }
];