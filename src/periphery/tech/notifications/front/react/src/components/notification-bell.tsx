import { Ionicons } from '@expo/vector-icons';
import { useFocusEffect, useTheme } from '@react-navigation/native';
import { useQuery } from '@tmobis/core/api/query';
import { createMessageApi } from '@tmobis/core/api/services/message-api';
import { useApiQueryScope } from '@tmobis/core/api/query/use-api-query-scope';
import { shouldRetryQuery } from '@tmobis/core/api/query/query-policy';
import { useGluestackColors } from '@tmobis/core/gluestack-ui-provider';
import { useRouter } from 'expo-router';
import React, { useCallback, useMemo } from 'react';
import { Pressable, StyleSheet, Text, View } from 'react-native';
import {
  GetUnreadNotificationCount,
  getUnreadNotificationCountQueryKey,
} from '../messages/get-unread-notification-count';

const BELL_REFETCH_INTERVAL_MS = 60_000;
const BADGE_COUNT_LIMIT = 99;
const BELL_ICON_SIZE = 25;

export default function NotificationBell() {
  const router = useRouter();
  const { colors } = useTheme();
  const palette = useGluestackColors();
  const { client, makeQueryKey } = useApiQueryScope();
  const api = useMemo(() => createMessageApi(client), [client]);
  const unreadQuery = useQuery({
    queryKey: makeQueryKey(getUnreadNotificationCountQueryKey()),
    queryFn: ({signal}) => api.sendMessage(new GetUnreadNotificationCount(), undefined, signal),
    retry: shouldRetryQuery,
    refetchInterval: BELL_REFETCH_INTERVAL_MS,
  });

  useFocusEffect(
    useCallback(() => {
      void unreadQuery.refetch();
    }, [unreadQuery.refetch])
  );

  const unreadCount = unreadQuery.data?.count ?? 0;
  const badgeText = unreadCount > BADGE_COUNT_LIMIT
    ? `${BADGE_COUNT_LIMIT}+`
    : String(unreadCount);

  return (
    <Pressable
      accessibilityLabel="Уведомления"
      accessibilityRole="button"
      onPress={() => router.navigate('/notifications')}
      style={styles.button}
    >
      <Ionicons
        name="notifications-outline"
        size={BELL_ICON_SIZE}
        color={colors.text}
      />
      {unreadCount > 0 && (
        <View
          style={[styles.badge, { backgroundColor: palette.destructive }]}
        >
          <Text
            style={[
              styles.badgeText,
              { color: palette.destructiveForeground },
            ]}
          >
            {badgeText}
          </Text>
        </View>
      )}
    </Pressable>
  );
}

const styles = StyleSheet.create({
  button: {
    marginRight: 12,
    padding: 8,
  },
  badge: {
    alignItems: 'center',
    borderRadius: 9,
    justifyContent: 'center',
    minHeight: 18,
    minWidth: 18,
    paddingHorizontal: 4,
    position: 'absolute',
    right: 0,
    top: 1,
  },
  badgeText: {
    fontSize: 10,
    fontWeight: '700',
  },
});
