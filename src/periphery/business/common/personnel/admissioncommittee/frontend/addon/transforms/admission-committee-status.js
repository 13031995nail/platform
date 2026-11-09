import FlexberryEnum from 'ember-flexberry-data/transforms/flexberry-enum';
import AdmissionCommitteeStatusEnum from '../enums/admission-committee-status';
import {
  SimplePredicate
} from 'ember-flexberry-data/query/predicate';
import { isEmpty } from '@ember/utils';

let AdmissionCommitteeEnum = FlexberryEnum.extend({
  enum: AdmissionCommitteeStatusEnum,
  sourceType: 'Tmobis.Business.Common.Personnel.AdmissionCommitteeStatus',
});

AdmissionCommitteeEnum.reopenClass({
  predicateForFilter (filter) {
    let pattern = !isEmpty(filter.pattern) ? this.create().serialize(filter.pattern) : null;
    switch (filter.condition) {
      case 'eq':
        return new SimplePredicate(filter.name, filter.condition, pattern);
      case 'neq':
        return new SimplePredicate(filter.name, filter.condition, pattern);
      default:
        return null;
    }
  }
});

export default AdmissionCommitteeEnum;
