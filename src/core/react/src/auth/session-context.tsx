import {
  createContext,
  useContext,
  type ReactNode,
} from 'react';
import type {SessionProvider} from './session-provider';

export type UserAuth = {
	userName?: string;
	logout: () => void | Promise<void>;
};

type SessionContextValue = {
	provider: SessionProvider;
	userAuth: UserAuth | null;
};

const SessionContext = createContext<SessionContextValue | undefined>(undefined);

export interface SessionContextProviderProps {
  provider: SessionProvider;
  userAuth?: UserAuth | null;
  children?: ReactNode;
}

export function SessionContextProvider({
  provider,
  userAuth = null,
  children,
}: SessionContextProviderProps): ReactNode {
  return (
    <SessionContext.Provider value={{ provider, userAuth }}>
      {children}
    </SessionContext.Provider>
  );
}

export function useSessionProvider(): SessionProvider {
  const context = useContext(SessionContext);
  if (!context) {
    throw new Error(
      'useSessionProvider must be used within a SessionContextProvider'
    );
  }
  return context.provider;
}

/**
 * Данные текущего пользователя для UI ядра (хедер и т.п.).
 */
export function useUserAuth(): UserAuth | null {
	const context = useContext(SessionContext);
	return context?.userAuth ?? null;
}
