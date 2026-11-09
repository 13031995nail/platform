import { Message, type MessageMetadata } from '@tmobis/core/models/message';

export interface TransferWorkShiftTaskResult {
  error: boolean;
  message?: string | null;
  newWorkShiftTaskId?: string | null;
  targetWorkShiftId?: string | null;
}

/**
 * Перенос пункта смены на будущую дату.
 */
export class TransferWorkShiftTask extends Message<TransferWorkShiftTaskResult> {
  static metadata = {
    type: 'event',
    group: 'WorkShift',
    name: 'TransferWorkShiftTask',
  } satisfies MessageMetadata;

  workShiftTaskId?: string;
  targetDate?: string;

  constructor(init?: Partial<Omit<TransferWorkShiftTask, 'metadata'>>) {
    super();
    Object.assign(this, init);
  }
}
