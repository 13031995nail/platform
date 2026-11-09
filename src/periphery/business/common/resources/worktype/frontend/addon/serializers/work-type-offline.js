import { OfflineSerializer as WorkTypeSerializer } from
  '../mixins/regenerated/serializers/work-type-offline';
import __ApplicationSerializer 
  from '@timepath/tmobis-core/serializers/application-offline';

export default __ApplicationSerializer.extend(WorkTypeSerializer, {
});
