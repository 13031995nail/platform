import { Message, type MessageMetadata } from '@tmobis/core/models/message';

export interface GetTodayWorkShiftIdResult {
  workShiftId?: string | null;
}

export class GetTodayWorkShiftId extends Message<GetTodayWorkShiftIdResult> {
  static metadata = {
    type: 'query',
    group: 'WorkShift',
    name: 'GetTodayWorkShiftId',
  } satisfies MessageMetadata;
}
