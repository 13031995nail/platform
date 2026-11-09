import { Serializer as ToolTypeSerializer } from
  '../mixins/regenerated/serializers/tool-type';
import __ApplicationSerializer 
  from '@timepath/tmobis-core/serializers/application';

export default __ApplicationSerializer.extend(ToolTypeSerializer, {
  /**
  * Field name where object identifier is kept.
  */
  primaryKey: '__PrimaryKey'
});
