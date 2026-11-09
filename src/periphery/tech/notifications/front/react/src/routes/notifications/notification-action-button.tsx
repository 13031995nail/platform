import { useGluestackColors } from '@tmobis/core/gluestack-ui-provider';
import React from 'react';
import { Pressable, StyleSheet, Text } from 'react-native';

interface NotificationActionButtonProps {
  children: string;
  disabled?: boolean;
  onPress: () => void;
}

export function NotificationActionButton({
  children,
  disabled = false,
  onPress,
}: NotificationActionButtonProps) {
  const colors = useGluestackColors();

  return (
    <Pressable
      disabled={disabled}
      onPress={onPress}
      style={[
        styles.button,
        { backgroundColor: colors.primary },
        disabled && styles.disabled,
      ]}
    >
      <Text style={[styles.text, { color: colors.primaryForeground }]}>
        {children}
      </Text>
    </Pressable>
  );
}

const styles = StyleSheet.create({
  button: {
    borderRadius: 6,
    paddingHorizontal: 16,
    paddingVertical: 8,
  },
  disabled: {
    opacity: 0.4,
  },
  text: {
    fontWeight: '600',
  },
});
