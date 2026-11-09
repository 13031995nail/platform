/**
 * Роут для просмотра и скачивания отчетов.
 * Обрабатывает URL параметры и передает их в компонент ReportView.
 *
 * URL параметры:
 * --------------
 * - `template` (string, required) - Имя шаблона
 * - `subType` (string, optional) - Подтип печатной формы
 * - `p_*` (string, optional) - Параметры отчета, где p_ - префикс
 *
 * Пример URL:
 * ```
 * /reports/view?template=Contract&p_clientId=123&p_date=2026-08-03
 * ```
 *
 * Префикс `p_` удаляется из параметров перед отправкой на бэкенд.
 * Например, `p_clientId` становится `clientId` в запросе API.
 *
 * @component
 */
import { useEffect, useMemo, useCallback, useState } from 'react';
import { View, StyleSheet, Text, ActivityIndicator } from 'react-native';
import { useLocalSearchParams, useRouter } from 'expo-router';
import { useHeader } from '@tmobis/core/components/header/context';
import HtmlView from '../../components/html-view';
import { TmobisButton } from '@tmobis/core/components/button';
import { useQuery } from '@tmobis/core/api/query';
import { useApiQueryScope } from '@tmobis/core/api/query/use-api-query-scope';
import { shouldRetryQuery } from '@tmobis/core/api/query/query-policy';
import { createReportApi } from '../../api/report-api';

export default function ViewReport() {
  const { setHeaderData } = useHeader();
  const router = useRouter();
  const rawParams = useLocalSearchParams();
  const [downloading, setDownloading] = useState(false);
  const { client, makeQueryKey } = useApiQueryScope();
  const reportApi = useMemo(() => createReportApi(client), [client]);

  const templateName = rawParams.template as string;
  const subTypeId = rawParams.subType as string | undefined;

  const paramEntries = useMemo(() => {
    const entries: [string, string][] = [];
    for (const [key, value] of Object.entries(rawParams)) {
      if (key !== 'template' && key !== 'subType') {
        const paramKey = key.startsWith('p_') ? key.substring(2) : key;
        entries.push([paramKey, String(value)]);
      }
    }
    return entries;
  }, [rawParams]);

  const params = useMemo(() => {
    const result: Record<string, string> = {};
    for (const [key, value] of paramEntries) {
      result[key] = value;
    }
    return result;
  }, [paramEntries]);

  const handleBack = useCallback(() => {
    router.back();
  }, [router]);

  const htmlQuery = useQuery({
    queryKey: makeQueryKey(['report', 'html', templateName, subTypeId, params]),
    queryFn: ({ signal }) => reportApi.getHtml(templateName, params, subTypeId, signal),
    retry: shouldRetryQuery,
  });
  const html = htmlQuery.data;

  const handleDownload = useCallback(async (format: string) => {
    setDownloading(true);
    try {
      await reportApi.download(templateName, params, format, subTypeId);
    } catch (err: any) {
      const errorData = err.response?.data;
      const errorMessage = typeof errorData === 'string' ? errorData : err.message;
      alert(errorMessage || 'Ошибка при скачивании отчета');
    } finally {
      setDownloading(false);
    }
  }, [reportApi, templateName, params, subTypeId]);

  useEffect(() => {
    setHeaderData({ title: 'Просмотр отчета' });
  }, [setHeaderData]);

  if (downloading) {
    return (
      <View style={styles.center}>
        <ActivityIndicator size="large" />
        <Text>Скачивание отчета...</Text>
      </View>
    );
  }

  if (htmlQuery.isError) {
    return (
      <View style={styles.center}>
        <Text>{htmlQuery.error?.message ?? 'Ошибка загрузки отчёта'}</Text>
      </View>
    );
  }

  if (!html) {
    return (
      <View style={styles.center}>
        <ActivityIndicator size="large" />
        <Text>Загрузка отчета...</Text>
      </View>
    );
  }

  return (
    <View style={styles.container}>
      {/* Хедер */}
      <View style={styles.header}>
        <View style={styles.headerActions}>
          <TmobisButton
            label="Назад"
            action="secondary"
            onPress={handleBack}
          />
          <TmobisButton
            label="Скачать PDF"
            onPress={() => handleDownload('pdf')}
          />
        </View>
      </View>

      {/* Контент отчета */}
      <View style={styles.reportContainer}>
        <HtmlView html={html} />
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#fff',
  },
  center: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    padding: 16,
  },
  header: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'flex-end',
    paddingVertical: 12,
    paddingHorizontal: 16,
    borderBottomWidth: 1,
    borderBottomColor: '#e5e5e5',
    gap: 8,
  },
  reportContainer: {
    flex: 1,
  },
  headerActions: {
    flexDirection: 'row',
    gap: 8,
  },
});
