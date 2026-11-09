import FlexberryEnum from 'ember-flexberry-data/transforms/flexberry-enum';
import PartnerReadinessEnum from '../enums/partner-readiness-level';

export default FlexberryEnum.extend({
  enum: PartnerReadinessEnum,
  sourceType: 'Tmobis.Business.Process.Documents.PartnerReadinessLevel'
});
