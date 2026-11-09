import { Message } from '@tmobis/core/models/message';

interface MessageWorkShiftTaskResult {
    error: boolean;
}

export class MessageWorkShiftTask extends Message<MessageWorkShiftTaskResult> {
    latitude?: number;
    longitude?: number;
    time?: string;
    workShiftTaskId?: string;
}
