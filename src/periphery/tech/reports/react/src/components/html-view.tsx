/**
 * Компонент для отображения HTML-отчетов.
 *
 * @example
 * <ReportView
 *   html="<div><h1>Отчет</h1></div>"
 * />
 *
 * @component
 * @param {HtmlViewProps} props - Свойства компонента
 * @returns {JSX.Element} WebView с HTML-контентом отчета
 */
import { useEffect, useState, useRef, useMemo } from 'react';
import {StyleSheet, Platform } from 'react-native';

interface HtmlViewProps {
  html: string;
}

export default function HtmlView({ html }: HtmlViewProps) {
  if (Platform.OS === 'web') {
    return <HtmlViewWeb html={html} />;
  }

  return <HtmlViewNative html={html} />;
}

// Рендерер для веба - рендерит HTML напрямую в контейнер
function HtmlViewWeb({ html }: { html: string }) {
  return (
    <div style={{ overflowY: 'auto', display: 'flex', justifyContent: 'center' }}>
      <div
        style={{ height: '100vh' }}
        dangerouslySetInnerHTML={{ __html: html }}
      />
    </div>
  );
}

// Рендерер для натива - использует WebView
function HtmlViewNative({ html }: { html: string }) {
  const { WebView } = require('react-native-webview');
  return <WebView source={{ html }} style={styles.webview} javaScriptEnabled={true} />;
}

const styles = StyleSheet.create({
  center: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    padding: 16,
  },
  error: {
    color: 'red',
    textAlign: 'center',
  },
  webview: {
    flex: 1,
    width: '100%',
  },
});
