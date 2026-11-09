import type { ApiClient } from '@tmobis/core/api/create-api-client';
import { downloadFileFromUrl } from '@tmobis/core/utils/download-file';

/** HTTP-операции с отчётами, независимые от React. */
export function createReportApi(client: ApiClient) {
  return {
    async getHtml(
      templateName: string,
      params: Record<string, string>,
      subTypeId?: string,
      signal?: AbortSignal
    ): Promise<string> {
      const response = await client.post<string>(
        '/api/report/GetHtml',
        { TemplateName: templateName, Params: params, Format: 'html' },
        { params: { subTypeId }, signal }
      );
      return String(response.data);
    },
    download(
      templateName: string,
      params: Record<string, string>,
      format: string,
      subTypeId?: string
    ): Promise<void> {
      return downloadFileFromUrl(
        client,
        '/api/report/GetFile',
        'POST',
        { templateName, params, format },
        subTypeId ? { subTypeId } : undefined,
        `${templateName}.${format}`
      );
    },
  };
}
