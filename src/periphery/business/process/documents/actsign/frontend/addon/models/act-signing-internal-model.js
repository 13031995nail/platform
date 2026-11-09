import DS from 'ember-data';
import EmberFlexberryDataModel from 'ember-flexberry-data/models/model';

/**
 * Модель для использования в модуле подписания mixins/act-signing-controller.
 */
let Model = EmberFlexberryDataModel.extend({
  /**
   * Поле, куда присваивается ссылка на сотрудника после его выбора из списка при добавлении подписанта.
   */
  addedEmployee: DS.belongsTo('place-of-employment', { inverse: null, async: false }),

  /**
   * Поле, хранящее ссылки на текущие объекты act-sign.
   */
  currentSigning: DS.belongsTo('act-sign', { inverse: null, async: false }),

  /**
   * Текущее состояние модуля. Возможные значения:
   * 'normal' - обычное состояние (отображение списка подписантов)
   * 'signerAdding' - нажата кнопка "Добавить подписанта", отображено окно лукапа для выбора сотрудника
   * 'signingTypeSelection' - произошло нажатие на строку списка, пользователю предлагается выбор способа подписания
   * 'signingSucceeded' - подписание выполнено успешно.
   */
  state: DS.attr('string')
});

export default Model;
