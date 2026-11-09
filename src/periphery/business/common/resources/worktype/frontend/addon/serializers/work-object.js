import { Serializer as WorkTypeSerializer } from
  '../mixins/regenerated/serializers/work-type';
import __ApplicationSerializer 
  from '@timepath/tmobis-core/serializers/application';

export default __ApplicationSerializer.extend(WorkTypeSerializer, {
  /**
  * Field name where object identifier is kept.
  */
  primaryKey: '__PrimaryKey'
});
