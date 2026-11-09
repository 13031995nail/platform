import EditFormNewRoute from '@timepath/tmobis-core/routes/base/edit-form-new';

import { translationMacro as t } from 'ember-i18n';

import QueryBuilder from 'ember-flexberry-data/query/builder';
import FilterOperator from 'ember-flexberry-data/query/filter-operator';
import Condition from 'ember-flexberry-data/query/condition';
import { ComplexPredicate, SimplePredicate } from 'ember-flexberry-data/query/predicate';

export default EditFormNewRoute.extend({
  modelProjection: 'AdmissionCommitteeE',
  modelName: 'admission-committee',
  templateName: 'admission-committee',

  masterTitle: t('forms.admission-committee.caption'),

  afterModel(model) {
    this._super(...arguments);
    model.set('regDate', new Date());
    let promises = [],
      builder = new QueryBuilder(this.store)
        .from('awp')
        .selectByProjection('AwpE')
        .where(new ComplexPredicate(Condition.And, ...[
          new SimplePredicate('isCurrent', FilterOperator.Eq, true),
          new SimplePredicate('actuality', FilterOperator.Eq, true)
        ]));
    promises.push(this.store.query('awp', builder.build()).then(data =>
      model.set('awp', data.firstObject))
    );
    return Promise.all(promises);
  }
});
