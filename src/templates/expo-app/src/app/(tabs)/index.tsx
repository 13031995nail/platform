import { Redirect } from 'expo-router';

/**
 * Лендинг приложения — рабочий стол.
 */
export default function Index() {
  return <Redirect href="/desktop" />;
}
