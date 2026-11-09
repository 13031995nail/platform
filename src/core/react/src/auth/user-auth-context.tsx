import {
	createContext,
	useContext,
	type ReactNode,
} from 'react';

export type UserAuth = {
	userName?: string;
	logout: () => void | Promise<void>;
};

const UserAuthContext = createContext<UserAuth | null>(null);

export type UserAuthProviderProps = {
	value: UserAuth | null;
	children?: ReactNode;
};

/**
 * Данные текущего пользователя для UI ядра (хедер и т.п.).
 * Провайдер аутентификации (Keycloak и др.) заполняет значение.
 */
const UserAuthProvider = ({
	value,
	children,
}: UserAuthProviderProps) => {
	return (
		<UserAuthContext.Provider value={value}>
			{children}
		</UserAuthContext.Provider>
	);
};

const useUserAuth = (): UserAuth | null => {
	return useContext(UserAuthContext);
};

export {
	UserAuthProvider,
	useUserAuth,
};
