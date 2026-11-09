import {useSignalRHandler} from '@tmobis/signalr';
import type {ReactNode} from 'react';
import {Alert} from 'react-native';

export const sendNotificationEventType =
  'Tmobis.Notification.WebSocket.SendNotification';

export interface WebSocketNotification {
  title?: string | null;
  content?: string | null;
}

export interface NotificationWebSocketProviderProps {
  children?: ReactNode;
}

const showNotification = (title: string, content: string): void =>
  Alert.alert(title || 'Уведомление', content);

export function NotificationWebSocketProvider({
  children,
}: NotificationWebSocketProviderProps): ReactNode {
  useSignalRHandler<WebSocketNotification>(
    sendNotificationEventType,
    ({title, content}) => showNotification(title ?? '', content ?? '')
  );

  return children;
}
