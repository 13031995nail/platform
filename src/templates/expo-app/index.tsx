import { registerRootComponent } from 'expo';
import { ExpoRoot } from 'expo-router';
import Head from 'expo-router/head';
import type { RequireContext } from 'expo-router';
import ModuleBase from "@tmobis/core/modules/module";
import CoreModule from '@tmobis/core/modules/core-module';
import AppModule from "@/modules/app-module";

import BusinessCoreModule from '@tmobis/core-business/module';
import WorkShiftModule from '@tmobis/business-process-workshift/module';
import { CompositionRoot, initializeModulesStatic, getRouteImports } from '@tmobis/core/modules/composition-root';
import { ModulesProvider } from '@tmobis/core/modules/provider';
import { AuthProvider } from '@tmobis/auth-keycloak/contexts/AuthProvider';
import KeycloakModule from "@tmobis/auth-keycloak/module";
import {NotificationWebSocketModule} from '@tmobis/notification-websocket';
import {SignalRModule} from '@tmobis/signalr';
import NotificationModule from "@tmobis/notification/module";
import ReportsModule from "@tmobis/reports/module";

const modules: ModuleBase[] = [
  new CoreModule(),
  new AppModule(),
  new BusinessCoreModule(),
  new WorkShiftModule(),
  new KeycloakModule(),
  new SignalRModule(),
  new NotificationWebSocketModule(),
  new ReportsModule(),
  new NotificationModule(),
];
initializeModulesStatic(modules);
const compositionRoot = new CompositionRoot(modules);

function prefixContext(ctx: RequireContext, prefix: string): RequireContext {
  // "./profile.tsx" -> "./(tabs)/profile.tsx"
  const toNewKey = (key: string) => 
    `./${prefix}/${key.replace(/^\.\//, '')}`;
  const toOldKey = new Map<string, string>();

  const newKeys = ctx.keys().map((oldKey) => {
    const newKey = toNewKey(oldKey);
    toOldKey.set(newKey, oldKey);
    return newKey;
  });

  const wrapped = ((newKey: string) => ctx(toOldKey.get(newKey)!)) as RequireContext;
  wrapped.keys = () => newKeys;
  wrapped.resolve = (newKey: string) => ctx.resolve(toOldKey.get(newKey)!);
  wrapped.id = ctx.id;

  return wrapped;
}

function mergeContexts(contexts: RequireContext[]): RequireContext {
  const keyMap = new Map<string, RequireContext>();
  for (const ctx of contexts) {
    for (const key of ctx.keys()) {
      if (keyMap.has(key)) {
        console.warn(`[routes] duplicate route key "${key}" — keeping first`);
        continue;
      }
      keyMap.set(key, ctx);
    }
  }
  
  const merged = ((key: string) => keyMap.get(key)!(key)) as RequireContext;
  merged.keys = () => Array.from(keyMap.keys());
  merged.resolve = (key: string) => keyMap.get(key)!.resolve(key);
  merged.id = contexts[0].id;
  return merged;
}

export function App() {
  const appCtx = require.context('./src/app');
  const moduleRoutes = getRouteImports(modules)
    .map(value => prefixContext(value, '(tabs)'));
  const ctx = mergeContexts([appCtx, ...moduleRoutes]);

  return (
      <AuthProvider client={compositionRoot.getApiClient()}>
        <ModulesProvider compositionRoot={compositionRoot}>
          <Head.Provider>
            <ExpoRoot context={ctx}/>
          </Head.Provider>
        </ModulesProvider>
      </AuthProvider>
  );
}

registerRootComponent(App);
