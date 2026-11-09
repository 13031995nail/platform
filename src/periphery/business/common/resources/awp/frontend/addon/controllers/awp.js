import EditFormController from '@timepath/tmobis-core/controllers/base/edit-form-common';
import { inject as service } from '@ember/service';

import Builder from 'ember-flexberry-data/query/builder';
import Condition from 'ember-flexberry-data/query/condition';
import FilterOperator from 'ember-flexberry-data/query/filter-operator';
import { SimplePredicate, ComplexPredicate } from 'ember-flexberry-data/query/predicate';

export default EditFormController.extend({
  modals: service(),

  parentRoute: 'awps',

  saveAwp(close) {
    let self = this,
      modals = this.get('modals'),
      model = this.get('model'),
      builder = new Builder(self.store)
        .from('awp')
        .selectByProjection('AwpL')
        .where(new ComplexPredicate(
          Condition.And,
          new SimplePredicate('id', FilterOperator.Neq, model.get('id')),
          new SimplePredicate('actuality', FilterOperator.Eq, true),
          new SimplePredicate('isCurrent', FilterOperator.Eq, true)));
    modals.showLoading({ message: 'Проверка на существование текущего ОЗП...' });
    return self.store.query('awp', builder.build())
      .then(records => {
        if (records.get('length') > 0) {
          let awp = records.get('firstObject');
          awp.set('isCurrent', false);
          return awp.save().then(x => {
            return self.save(close);
          });
        } else {
          return self.save(close);
        }
      }).finally(function () {
        modals.hideLoading(true);
      });
  },

  saveByСondition(close) {
    let self = this,
      modals = this.get('modals'),
      model = this.get('model'),
      isActual = model.get('actuality'),
      isCurrent = model.get('isCurrent');
    if (isActual) {
      if (isCurrent) {
        return modals.showModal({
          message: 'Создать новый актуальный ОЗП?',
          onApprove: () => {
            return self.saveAwp(close);
          },
          onDeny: () => {
            model.set('isCurrent', false);
            return self.save(close);
          }
        });
      } else {
        return self.save(close)
      }
    } else {
      return self.save(close)
    }
  },

  actions: {
    save() {
      return this.saveByСondition(false);
    },

    saveAndClose() {
      return this.saveByСondition(true);
    },
  }
});