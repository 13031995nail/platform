import axios from 'axios';
import type { ApiClient } from '../create-api-client';

export interface FileMetadata {
  id: string;
  url: string;
  name: string;
  mimeType: string;
}

/**
 * Загрузка файла на сервер через API
 * @param file - Файл (Blob для web, URI для mobile)
 * @param fileName - Имя файла
 * @returns ID загруженного файла (GUID)
 */
export const uploadFile = async (file: Blob | string, fileName: string, client: ApiClient): Promise<string> => {
  let blob: Blob;

  if (typeof file === 'string') {
    const localResponse = await axios.get<Blob>(file, { responseType: 'blob' });
    blob = localResponse.data;
  } else {
    blob = file;
  }

  const response = await client.put<string>('/api/File', blob, {
    params: { fileName },
    headers: {
      'Content-Type': blob.type || 'application/octet-stream',
    },
  });

  return response.data.trim();
};

/**
 * Получение файла с сервера
 * @param fileId - ID файла
 * @returns Blob с содержимым файла
 */
export const downloadFile = async (fileId: string, client: ApiClient): Promise<Blob> => {
  const response = await client.get<Blob>(`/api/File/${fileId}`, {
    responseType: 'blob',
  });
  return response.data;
};

/**
 * Получение информации о файле по ID через GET запрос к /metadata
 * Возвращает { id, url, name, mimeType } для инициализации
 */
export const getFileById = async (fileId: string, client: ApiClient, signal?: AbortSignal): Promise<FileMetadata> => {
  const response = await client.get(`/api/File/${fileId}/metadata`, { signal });
  
  return {
    id: fileId,
    url: response.data.url,
    name: response.data.name,
    mimeType: response.data.mimeType,
  };
};

/**
 * Получение списка всех файлов
 * @param fileIds - Список ID файлов
 * @returns Массив объектов { url, name, mimeType, id } для каждого файла
 */
export const getFiles = async (fileIds: string[], client: ApiClient, signal?: AbortSignal): Promise<FileMetadata[]> => {
  const validIds = fileIds.map(id => id.trim()).filter(Boolean);
  
  const promises = validIds.map(id => getFileById(id, client, signal));
  const results = await Promise.allSettled(promises);

  return results
    .filter((result): result is PromiseFulfilledResult<FileMetadata> => result.status === 'fulfilled')
    .map(result => result.value);
};

/** Сервис файловых операций с заданным транспортом. */
export function createFileApi(client: ApiClient) {
  return {
    uploadFile: (file: Blob | string, name: string) => uploadFile(file, name, client),
    downloadFile: (id: string) => downloadFile(id, client),
    getFileById: (id: string, signal?: AbortSignal) => getFileById(id, client, signal),
    getFiles: (ids: string[], signal?: AbortSignal) => getFiles(ids, client, signal),
    getFileBlob: async (url: string): Promise<Blob> => {
      const response = await client.get<Blob>(url, { responseType: 'blob' });
      return response.data;
    },
  };
}
export type FileApi = ReturnType<typeof createFileApi>;
