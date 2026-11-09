import { createEnum } from 'ember-flexberry-data/utils/enum-functions';

export default createEnum({
  Empty: '',
  Ready: 'Готов',
  ConditionReady: 'Готов с условиями',
  NotReady: 'Не готов',
});
