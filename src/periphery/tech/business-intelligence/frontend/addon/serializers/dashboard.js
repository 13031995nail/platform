import { Serializer as DashboardSerializer } from
  '../mixins/regenerated/serializers/dashboard';
import __ApplicationSerializer
    from '@timepath/tmobis-core/serializers/application';

export default __ApplicationSerializer.extend(DashboardSerializer, {
  /**
  * Field name where object identifier is kept.
  */
  primaryKey: '__PrimaryKey'
});