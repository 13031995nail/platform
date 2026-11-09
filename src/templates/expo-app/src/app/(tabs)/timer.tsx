import React, { useState } from 'react';
import { View, Text, StyleSheet } from 'react-native';
import { useTimer } from 'react-timer-hook';
import Constants from 'expo-constants';
import { useFocusEffect } from '@react-navigation/native';
import { useCallback } from 'react';
import { useHeader } from "@tmobis/core/components/header/context";

const INITIAL_SECONDS = 10; // 10 секунд

const BACKEND_URL = Constants.expoConfig?.extra?.backendUrl + '/api/service/interfaceVersion';

export default function TimerWithStringResponse() {
  const [status, setStatus] = useState('timer'); // 'timer' | 'loading' | 'data' | 'error'
  const [responseText, setResponseText] = useState('');
  const { setHeaderData } = useHeader();

  const getExpiry = () => {
    const now = new Date();
    now.setSeconds(now.getSeconds() + INITIAL_SECONDS);
    return now;
  };

  useFocusEffect(
    useCallback(() => {
          setHeaderData({ title: 'Таймер' });
    }, [setHeaderData])
  )

  const {
    seconds,
    minutes,
    restart
  } = useTimer({
    expiryTimestamp: getExpiry(),
    onExpire: async () => {
      if (!BACKEND_URL) {
        console.error('backendUrl не найден в app.json');
        setStatus('error');
        setTimeout(() => {
          setStatus('timer');
          restart(getExpiry(), true);
        }, 1000);
        return;
      }

      setStatus('loading');
      setResponseText('');

      try {
        const response = await fetch(BACKEND_URL, {
          method: 'GET',
          headers: {
            'Content-Type': 'application/json'
          }
        });

        if (!response.ok) {
          throw new Error(`HTTP ${response.status}`);
        }

        // Получаем строку из body
        const text = await response.text();
        setResponseText(text);
        setStatus('data');
      } catch (err) {
        console.error('GET request failed:', err);
        setStatus('error');
      }

      setTimeout(() => {
        setStatus('timer');
        restart(getExpiry(), true); // снова запускаем таймер
      }, 2000); // показываем строку 2 секунды
    }
  });

  const formatTime = (m, s) =>
    `${String(m).padStart(2, '0')}:${String(s).padStart(2, '0')}`;

  let content;
  if (status === 'timer') {
    content = <Text style={styles.timerText}>{formatTime(minutes, seconds)}</Text>;
  } else if (status === 'loading') {
    content = <Text style={styles.statusText}>Загрузка...</Text>;
  } else if (status === 'data') {
    content = (
      <View style={styles.dataContainer}>
        <Text style={styles.dataLabel}>Данные с бэкенда:</Text>
        <Text style={styles.dataText}>{responseText}</Text>
      </View>
    );
  } else {
    content = <Text style={styles.errorText}>Ошибка запроса</Text>;
  }

  return (
    <View style={styles.container}>
      {content}
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    padding: 20
  },
  timerText: {
    fontSize: 64,
    fontWeight: '600'
  },
  statusText: {
    fontSize: 28,
    color: '#3498db'
  },
  dataContainer: {
    alignItems: 'center',
    maxWidth: '90%'
  },
  dataLabel: {
    fontSize: 20,
    fontWeight: '600',
    marginBottom: 8,
    color: '#27ae60'
  },
  dataText: {
    fontSize: 18,
    textAlign: 'center',
    color: '#2c3e50'
  },
  errorText: {
    fontSize: 28,
    color: '#e74c3c'
  }
});
