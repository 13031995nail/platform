import { IEntity } from './entity';

export interface ModelMeta<TEntity extends IEntity> {
  defaults: TEntity;
  className: string;
}