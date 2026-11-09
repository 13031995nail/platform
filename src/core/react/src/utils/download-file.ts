import { Platform } from 'react-native';
import { Directory, File, Paths } from 'expo-file-system';
import type { AxiosRequestConfig } from 'axios';
import type { ApiClient } from '@tmobis/core/api/create-api-client';

/**
 * Скачивает файл в браузере через Blob
 */
export const downloadFileToWeb = async (blob: Blob, fileName: string, mimeType?: string) => {
  try {
    const blobWithMimeType = mimeType ? new Blob([blob], { type: mimeType }) : blob;
    const downloadUrl = window.URL.createObjectURL(blobWithMimeType);
    const link = document.createElement('a');
    link.href = downloadUrl;
    link.download = fileName;

    document.body.appendChild(link);
    link.click();

    document.body.removeChild(link);
    window.URL.revokeObjectURL(downloadUrl);
  } catch (error) {
    console.error('Ошибка при скачивании:', error);
  }
};

/**
 * Скачивает файл на устройство через expo-file-system
 */
export const downloadFileToPhone = async (url: string, fileName: string) => {
  const destination = new Directory(Paths.cache);
  try {
    if (!destination.exists) {
      destination.create();
    }

    const targetFile = new File(destination, fileName);

    await File.downloadFileAsync(url, targetFile);
  } catch (error) {
    console.error('Ошибка при скачивании:', error);
  }
};

/**
 * Скачивает файл из Blob (для web) или из URL (для native)
 */
export const downloadFile = async (
  blobOrUrl: Blob | string,
  fileName: string,
  mimeType?: string
) => {
  if (Platform.OS === 'web') {
    if (blobOrUrl instanceof Blob) {
      await downloadFileToWeb(blobOrUrl, fileName, mimeType);
    }
  } else {
    if (typeof blobOrUrl === 'string') {
      await downloadFileToPhone(blobOrUrl, fileName);
    }
  }
};

/**
 * Извлекает имя файла из header content-disposition
 */
const getFileNameFromContentDisposition = (header: string, defaultName: string): string => {
  const match = header.match(/filename\*=?(?:UTF-8)?['"]?(.*)/);
  if (match && match[1]) {
    return match[1].replace(/"/g, '');
  }
  return defaultName;
};

/**
 * Скачивает файл по URL с автоматическим определением имени файла из content-disposition.
 * Поддерживает передачу произвольных данных в теле запроса.
 *
 * @param url - URL для скачивания файла
 * @param method - HTTP метод (GET или POST)
 * @param data - Данные для отправки в теле запроса (для POST)
 * @param params - URL параметры запроса
 * @param fileName - Имя файла по умолчанию (если не удалось получить из заголовка)
 *
 * @example
 * // Скачивание через POST с телом запроса
 * await downloadFileFromUrl('/api/report/GetFile', 'POST', {
 *   templateName: 'Contract',
 *   params: { clientId: '123' },
 *   format: 'pdf'
 * }, { subTypeId: 'abc' });
 *
 * @example
 * // Скачивание через GET
 * await downloadFileFromUrl('/api/file/download/123', 'GET');
 */
export const downloadFileFromUrl = async (
  client: ApiClient,
  url: string,
  method: 'GET' | 'POST' = 'GET',
  data?: unknown,
  params?: Record<string, string | number>,
  defaultFileName: string = 'file'
) => {
  const config: AxiosRequestConfig = {
    url,
    method,
    params,
    responseType: 'blob',
  };

  if (method === 'POST' && data) {
    config.data = data;
  }

  const response = await client.request<Blob>(config);

  // Получение имени файла из content-disposition или использование имени по умолчанию
  const cdHeader = response.headers['content-disposition'] as string | undefined;
  const fileName = cdHeader
    ? getFileNameFromContentDisposition(cdHeader, defaultFileName)
    : defaultFileName;

  await downloadFile(response.data, fileName, response.headers['content-type'] as string | undefined);
};
