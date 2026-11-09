import { useTheme } from '@react-navigation/native';
import { useGluestackColors } from '@tmobis/core/gluestack-ui-provider';
import React from 'react';
import { StyleSheet, Text, View } from 'react-native';
import type { Notification } from '../../models/notification';
import { NotificationStatus } from '../../models/notification-status';

interface NotificationListItemProps {
  notification: Notification;
}

export function NotificationListItem({
  notification,
}: NotificationListItemProps) {
  const { colors } = useTheme();
  const palette = useGluestackColors();
  const content = notification.notificationContents?.[0];
  const isRead = notification.status === NotificationStatus.Read;

  return (
    <View
      style={[
        styles.notification,
        {
          backgroundColor: isRead
            ? colors.card
            : palette.backgroundMuted,
          borderColor: colors.border,
        },
      ]}
    >
      <View style={styles.notificationHeader}>
        <Text style={[styles.title, { color: colors.text }]}>
          {content?.title || notification.type?.name || 'Уведомление'}
        </Text>
        {!isRead && (
          <View
            style={[styles.unreadMarker, { backgroundColor: palette.info }]}
          />
        )}
      </View>
      {!!content?.content && (
        <Text style={[styles.content, { color: colors.text }]}>
          {content.content}
        </Text>
      )}
      {!!notification.createTime && (
        <Text style={[styles.date, { color: colors.text }]}>
          {new Date(notification.createTime).toLocaleString('ru-RU')}
        </Text>
      )}
    </View>
  );
}

const styles = StyleSheet.create({
  notification: {
    borderRadius: 8,
    borderWidth: 1,
    padding: 14,
  },
  notificationHeader: {
    alignItems: 'center',
    flexDirection: 'row',
    gap: 8,
  },
  title: {
    flex: 1,
    fontSize: 16,
    fontWeight: '600',
  },
  unreadMarker: {
    borderRadius: 4,
    height: 8,
    width: 8,
  },
  content: {
    fontSize: 14,
    marginTop: 8,
    opacity: 0.9,
  },
  date: {
    fontSize: 12,
    marginTop: 10,
    opacity: 0.65,
  },
});
