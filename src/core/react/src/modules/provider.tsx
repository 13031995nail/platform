import React, {createContext, useContext, useEffect} from 'react';
import {CompositionRoot} from "@tmobis/core/modules/composition-root";
import {MessageDispatcher} from '@tmobis/core/messages/dispatcher';

interface ModulesContextType {
  compositionRoot: CompositionRoot;
}

interface ModulesProviderProps {
  compositionRoot: CompositionRoot;
  children: React.ReactNode;
}

const ModulesContext
  = createContext<ModulesContextType | undefined>(undefined);

export function ModulesProvider({ compositionRoot, children }: ModulesProviderProps) {
  const wrappedChildren = compositionRoot.modules.reduceRight(
    (currentChildren, module) =>
      module.wrapProvider?.(currentChildren, {
        modules: compositionRoot.modules,
        apiClient: compositionRoot.getApiClient(),
      }) ?? currentChildren,
    children
  );

  useEffect(() => {
    void compositionRoot.initialize().catch(() => {
      console.error('[modules] Module initialization failed');
    });

    return () => {
      void compositionRoot.dispose().catch(() => {
        console.error('[modules] Module cleanup failed');
      });
    };
  }, [compositionRoot]);

  return (
    <ModulesContext.Provider value={{ compositionRoot }}>
      {wrappedChildren}
    </ModulesContext.Provider>
  );
}

export function useModules() {
  const context = useContext(ModulesContext);
  if (context === undefined) {
    throw new Error('useModules must be used within a ModulesProvider');
  }
  return context;
}

export function useOptionalCompositionRoot() {
  return useContext(ModulesContext)?.compositionRoot;
}

export function useMessageDispatcher(): MessageDispatcher {
  return useModules().compositionRoot.getMessageDispatcher();
}

export function useOptionalMessageDispatcher(): MessageDispatcher | undefined {
  return useContext(ModulesContext)?.compositionRoot.getMessageDispatcher();
}
