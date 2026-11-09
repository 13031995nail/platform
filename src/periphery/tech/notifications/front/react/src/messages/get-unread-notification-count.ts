import {
  getMessageQueryKey,
} from '@tmobis/core/api/services/message-api';
import {
  Message,
  type MessageMetadata,
} from '@tmobis/core/models/message';

export interface GetUnreadNotificationCountResult {
  count: number;
}

export class GetUnreadNotificationCount
  extends Message<GetUnreadNotificationCountResult> {
  static metadata = {
    type: 'query',
    group: 'Notification',
    name: 'GetUnreadNotificationCount',
  } satisfies MessageMetadata;
}

export const getUnreadNotificationCountQueryKey = () =>
  getMessageQueryKey(new GetUnreadNotificationCount());
