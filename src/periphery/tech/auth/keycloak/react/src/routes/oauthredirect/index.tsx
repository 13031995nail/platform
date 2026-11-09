import { useEffect } from 'react';
import { View, ActivityIndicator, StyleSheet, Platform } from 'react-native';
import * as WebBrowser from 'expo-web-browser';
import { useRouter } from 'expo-router';

/**
 * Маршрут перенаправления OAuth.
 * Обрабатывает обратный вызов от провайдеров OAuth после аутентификации.
 * 
 * Запускается ссылками вида: схемаПриложения://oauthredirect?state=...
 * 
 * Завершает сеанс аутентификации для натива, после чего
 * переходит в основное приложение после завершения аутентификации
 */
export default function OAuthRedirectRoute() {
  const router = useRouter();

  useEffect(() => {
    if (Platform.OS !== 'web') {
      WebBrowser.maybeCompleteAuthSession();
    }

    router.replace('/(tabs)');
  }, [router]);

  return (
    <View style={styles.container}>
      <ActivityIndicator size="large" />
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: '#fff',
  },
});