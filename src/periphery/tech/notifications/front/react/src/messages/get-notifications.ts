import {
  getMessageQueryKey,
} from '@tmobis/core/api/services/message-api';
import {
  Message,
  type MessageMetadata,
} from '@tmobis/core/models/message';
import type { Notification } from '../models/notification';

export const NOTIFICATIONS_PAGE_SIZE = 20;

export interface GetNotificationsResult {
  data: Notification[];
  page?: number | null;
  perPage?: number | null;
  totalCount?: number | null;
}

export class GetNotifications extends Message<GetNotificationsResult> {
  static metadata = {
    type: 'query',
    group: 'Notification',
    name: 'GetNotifications',
  } satisfies MessageMetadata;

  constructor(
    public page = 1,
    public perPage = NOTIFICATIONS_PAGE_SIZE
  ) {
    super();
  }
}

export const getNotificationsQueryKey = (
  page: number,
  perPage = NOTIFICATIONS_PAGE_SIZE
) => getMessageQueryKey(new GetNotifications(page, perPage));
