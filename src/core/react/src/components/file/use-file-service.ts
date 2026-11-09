import { Directory, File, Paths } from 'expo-file-system';
import { ImageItem } from '../ui/image-viewer';
import { Platform } from 'react-native';
import * as DocumentPicker from 'expo-document-picker';
import { useState, useEffect, useMemo, useRef } from 'react';
import { useApiQueryScope } from '@tmobis/core/api/query/use-api-query-scope';
import { createFileApi, type FileApi } from '@tmobis/core/api/services/file-api';
import type { ApiClient } from '@tmobis/core/api/create-api-client';

interface useFileServiceProps {
/**
   * ids файлов, разделенные точкой с запятой
   */
  value?: string;

  /**
   * Имя поля формы для обновления
   */
  fieldName?: string;

  /**
   * Функция для обновления значения поля формы
   */
  onChange?: (value: string) => void;
}

/**
 * Свойства файла
 */
export interface FileInfoProps extends ImageItem {
  /**
   * id файла
   */
  id: string;

  /**
   * url файла
   */
  url: string;

  /**
   * Название файла
   */
  name: string;

  /**
   * Тип файла
   */
  type: string;
}

/**
 * Данные, возвращаемые хуком {@link useFileService}
 */
export interface FileServiceData {
  /**
   * Функция скачивания файла
   */
  downloadFile: (fileInfo: FileInfoProps) => Promise<void>;

  /**
   * Функция загрузки файла с устройства
   */
  pickDocuments: () => Promise<void>;

  /**
   * Выбранные файлы
   */
  fileInfos: FileInfoProps[];

  /**
   * Удаление файла
   */
  deleteFile: (fileInfo: FileInfoProps) => void;
  updateFieldValue: (files: FileInfoProps[]) => void;
}

const loadExistingFiles = async (ids: string[], service: FileApi, signal: AbortSignal): Promise<FileInfoProps[]> => {
  try {
    const filesData = await service.getFiles(ids, signal);
    return filesData.map(f => ({
      id: f.id,
      url: f.url,
      name: f.name,
      type: f.mimeType
    }));
  } catch (error) {
    console.error('Не удалось загрузить файлы:', error);
    return [];
  }
};

export default function useFileService({ value, fieldName, onChange }: useFileServiceProps): FileServiceData {
  const { client, revision } = useApiQueryScope();
  const service = useMemo(() => createFileApi(client), [client]);
  const [fileInfos, setFileInfos] = useState<FileInfoProps[]>([]);
  const lastProcessedValueRef = useRef<string>('');
  const lastProcessedScopeRef = useRef<{ client: ApiClient; revision: number } | null>(null);

  // Загрузка существующих файлов при инициализации и внешнем изменении
  useEffect(() => {
    const normalizedValue = value || '';
    
    // Предотвращаем повторную загрузку, если значение изменилось изнутри хука
    const previous = lastProcessedScopeRef.current;
    if (normalizedValue === lastProcessedValueRef.current
      && previous?.client === client && previous.revision === revision) {
      return;
    }
    
    lastProcessedValueRef.current = normalizedValue;
    lastProcessedScopeRef.current = { client, revision };

    let active = true;
    const controller = new AbortController();
    const loadFiles = async () => {
      if (normalizedValue) {
        const ids = normalizedValue.split(';').filter(id => id.trim());
        const files = await loadExistingFiles(ids, service, controller.signal);
        if (active) setFileInfos(files);
      } else {
        if (active) setFileInfos([]);
      }
    };
    void loadFiles();
    return () => { active = false; controller.abort(); };
  }, [value, client, revision, service]);

  const downloadFileToWeb = async (fileInfo: FileInfoProps) => {
    try {
      const blob = await service.getFileBlob(fileInfo.url);

      const downloadUrl = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = downloadUrl;
      link.download = fileInfo.name;

      document.body.appendChild(link);
      link.click();

      document.body.removeChild(link);
      window.URL.revokeObjectURL(downloadUrl);
    } catch (error) {
      console.error('Ошибка при скачивании:', error);
    }
  };

  const downloadFileToPhone = async (fileInfo: FileInfoProps) => {
    const url = fileInfo.url;
    const destination = new Directory(Paths.cache);
    try {
      if (!destination.exists) {
        destination.create();
      }
      
      const targetFile = new File(destination, fileInfo.name);
      
      await File.downloadFileAsync(url, targetFile);
    } catch (error) {
      console.error('Ошибка при скачивании:', error);
    }
  };

  const downloadFile = async (fileInfo: FileInfoProps) => {
    if (Platform.OS === 'web') {
      await downloadFileToWeb(fileInfo);
    } else {
      await downloadFileToPhone(fileInfo);
    }
  };

  const updateFieldValue = (files: FileInfoProps[]) => {
    const fileIds = files.map(f => f.id).join(';');
    lastProcessedValueRef.current = fileIds;
    if (onChange) {
      onChange(fileIds);
    }
  };

  const pickDocuments = async () => {
    try {
      const result = await DocumentPicker.getDocumentAsync({
        type: '*/*',
        multiple: true,
        copyToCacheDirectory: true,
      });

      if (!result.canceled && result.assets?.length > 0) {
        const newFiles: FileInfoProps[] = [];

        for (const asset of result.assets) {
          try {
            const fileId = await service.uploadFile(asset.uri, asset.name);
            const fileData = await service.getFileById(fileId);
            newFiles.push({
              id: fileId,
              url: fileData.url,
              name: asset.name,
              type: asset.mimeType || 'application/octet-stream',
            });
          } catch (error) {
            console.error(`Ошибка загрузки файла ${asset.name}:`, error);
          }
        }

        const updatedFiles = [...fileInfos, ...newFiles];
        setFileInfos(updatedFiles);
        updateFieldValue(updatedFiles);
      }
    } catch (error) {
      console.error('Ошибка выбора файлов:', error);
    }
  };

  const deleteFile = (fileInfo: FileInfoProps) => {
    const updatedFiles = fileInfos.filter(file => file.id !== fileInfo.id);
    setFileInfos(updatedFiles);
    updateFieldValue(updatedFiles);
  };

  return {
    fileInfos,
    downloadFile,
    pickDocuments,
    deleteFile,
    updateFieldValue
  };
}
