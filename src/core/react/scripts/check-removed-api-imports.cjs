#!/usr/bin/env node
'use strict';

// Выявление устаревших импортов во всех frontend-пакетах до запуска tsc/Metro.
const fs = require('node:fs');
const path = require('node:path');

const srcRoot = path.resolve(__dirname, '../../..');
const obsoleteNames = ['default-client', 'entity', 'file', 'messageApi', 'query-policy'];
const obsoletePaths = new Set(
  obsoleteNames.map(name => path.join(srcRoot, 'core/react/src/api', name + '.ts'))
);
const oldAlias = /^@tmobis\/core\/api\/(default-client|entity|file|messageApi|query-policy)$/;
const importPattern = /\b(?:from\s*|import\s*\(\s*|require\s*\(\s*)['"]([^'"]+)['"]/g;
const defaultImportPattern = /\bimport\s+[a-zA-Z_$][\w$]*(?:\s*,\s*\{[^}]*\})?\s+from\s*['"]@tmobis\/core\/api['"]/g;
const ignoredDirectories = new Set([
  'node_modules', '.git', '.yarn', '.expo', '.next', 'build', 'dist', 'coverage', 'obj', 'bin',
]);
const extensions = new Set(['.js', '.jsx', '.ts', '.tsx', '.mjs', '.cjs']);
const violations = [];

function scan(directory) {
  for (const entry of fs.readdirSync(directory, { withFileTypes: true })) {
    const file = path.join(directory, entry.name);
    if (entry.isDirectory()) {
      if (!ignoredDirectories.has(entry.name)) scan(file);
      continue;
    }
    if (!entry.isFile() || !extensions.has(path.extname(entry.name))) continue;
    const contents = fs.readFileSync(file, 'utf8');
    const displayPath = path.relative(srcRoot, file);
    for (const match of contents.matchAll(importPattern)) {
      const specifier = match[1];
      const isRemovedAlias = oldAlias.test(specifier);
      const isRemovedRelativePath = specifier.startsWith('.')
        && obsoletePaths.has(path.resolve(directory, specifier + '.ts'));
      if (isRemovedAlias || isRemovedRelativePath) {
        violations.push(displayPath + ': ' + specifier);
      }
    }
    for (const match of contents.matchAll(defaultImportPattern)) {
      violations.push(displayPath + ': default import from @tmobis/core/api');
    }
  }
}

scan(srcRoot);
if (violations.length) {
  process.stderr.write('Removed API imports found:\n' + violations.join('\n') + '\n');
  process.exitCode = 1;
} else {
  process.stdout.write('No removed API imports found.\n');
}
