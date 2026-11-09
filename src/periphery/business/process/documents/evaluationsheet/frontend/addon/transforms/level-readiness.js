import FlexberryEnum from 'ember-flexberry-data/transforms/flexberry-enum';
import LevelReadinessEnum from '../enums/level-readiness';

export default FlexberryEnum.extend({
  enum: LevelReadinessEnum,
  sourceType: 'Tmobis.Business.Process.Documents.LevelReadiness'
});
