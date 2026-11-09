import { NIL, v4 } from 'uuid';

import { toNonEmptyString } from './nonEmptyString';

export const createUuid = (): NonEmptyString => toNonEmptyString(v4());

export const emptyGuid: NonEmptyString = NIL as NonEmptyString;
