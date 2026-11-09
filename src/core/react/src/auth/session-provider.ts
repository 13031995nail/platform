export interface AuthSession {
  accessToken: string;
}

export type SessionListener = (
  session: Readonly<AuthSession> | undefined
) => void;

export interface SessionProvider {
  getSession(): Promise<Readonly<AuthSession> | undefined>;
  subscribe(listener: SessionListener): () => void;
}
