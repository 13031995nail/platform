import DS from 'ember-data';
import BaseModel from 'ember-flexberry-data/models/model';

let Model = BaseModel.extend({
  showArchived: DS.attr('boolean', { default: false }),
  sessionDateBegin: DS.attr('date'),
  sessionDateEnd: DS.attr('date'),
  records: DS.hasMany('user-session',
    { inverse: null, async: false, polymorphic: true  }),
  sorting: undefined
});

export default Model;
