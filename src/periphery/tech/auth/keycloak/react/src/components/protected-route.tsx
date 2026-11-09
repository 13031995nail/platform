import React from 'react';
import { View, ActivityIndicator, Pressable, Text, StyleSheet } from 'react-native';
import { useAuth } from '../contexts/AuthProvider';
import { AuthState } from '../lib/types';

interface ProtectedRouteProps {
  children: React.ReactNode;
}

/**
 * Защита маршрутов от неавторизованного доступа.
 * Использует state machine для управления состоянием аутентификации.
 */
export function ProtectedRoute({ children }: ProtectedRouteProps) {
  const { authState, login, logout } = useAuth();
  const state = authState as AuthState;

  console.log('[ProtectedRoute] Rendering with authState.status:', authState.status);

  switch (state.status) {
    case 'authenticated':
      return <>{children}</>;
    case 'idle':
      // При idle показываем кнопку входа на весь экран
      return (
        <View style={styles.container}>
          <Pressable onPress={login} style={styles.loginButton}>
            <Text style={styles.loginButtonText}>Аутентифицироваться</Text>
          </Pressable>
        </View>
      );
    case 'authenticating':
      // В процессе аутентификации - показываем загрузку
      console.log('[ProtectedRoute] Showing spinner for authenticating state');
      return (
        <View style={{ flex: 1, justifyContent: 'center', alignItems: 'center' }}>
          <ActivityIndicator size="large" color="#208AEF" />
        </View>
      );
    case 'error':
      // Ошибка аутентификации - показываем кнопку повтора
      console.log('[ProtectedRoute] Error state:', state.error);
      return (
        <View style={{ flex: 1, justifyContent: 'center', alignItems: 'center' }}>
          <Text style={styles.errorText}>Ошибка: {state.error.message}</Text>
          <Pressable onPress={login} style={styles.retryButton}>
            <Text style={styles.retryButtonText}>Попробовать снова</Text>
          </Pressable>
          <Pressable onPress={logout} style={styles.logoutButton}>
            <Text style={styles.logoutButtonText}>Выйти</Text>
          </Pressable>
        </View>
      );
    default:
      // Exhaustive check
      const _exhaustive: never = state;
      throw new Error(`Unhandled auth status: ${_exhaustive}`);
  }
}

export default ProtectedRoute;

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: '#fff',
  },
  loginButton: {
    backgroundColor: '#208AEF',
    padding: 15,
    borderRadius: 8,
    marginTop: 20,
  },
  loginButtonText: {
    color: '#FFF',
    fontSize: 16,
    fontWeight: '600',
  },
  retryButton: {
    backgroundColor: '#208AEF',
    padding: 15,
    borderRadius: 8,
    marginTop: 10,
  },
  retryButtonText: {
    color: '#FFF',
    fontSize: 16,
    fontWeight: '600',
  },
  logoutButton: {
    backgroundColor: '#dc3545',
    padding: 10,
    borderRadius: 8,
    marginTop: 10,
  },
  logoutButtonText: {
    color: '#FFF',
    fontSize: 14,
  },
  errorText: {
    color: '#dc3545',
    fontSize: 16,
    marginBottom: 20,
  },
});