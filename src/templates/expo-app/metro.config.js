const { getDefaultConfig } = require('expo/metro-config');
const { withNativewind } = require('nativewind/metro');
const path = require('path');

const projectRoot = __dirname;
const coreReactRoot = path.resolve(projectRoot, '../../core/react');

const config = getDefaultConfig(projectRoot);

config.resolver.unstable_enableSymlinks = true;
config.resolver.unstable_enablePackageExports = true;

config.watchFolders = [...(config.watchFolders ?? []), coreReactRoot];

config.resolver.nodeModulesPaths = [
  path.resolve(projectRoot, 'node_modules'),
  path.resolve(coreReactRoot, 'node_modules'),
];

module.exports = withNativewind(config);