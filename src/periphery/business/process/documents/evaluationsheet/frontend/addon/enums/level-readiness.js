import { createEnum } from 'ember-flexberry-data/utils/enum-functions';

export default createEnum({
  NotReady: 'Не готов',
  ReadyWithConditions: 'Готов с условиями',
  Ready: 'Готов',
});