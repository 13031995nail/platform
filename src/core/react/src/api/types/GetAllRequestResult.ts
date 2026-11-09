import { IEntity } from '../../models/entity';

export interface GetAllRequestResult<T extends IEntity> {
  count: number;
  records: T[];
}
