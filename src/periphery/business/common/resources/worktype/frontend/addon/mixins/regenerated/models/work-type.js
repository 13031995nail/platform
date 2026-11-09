import Mixin from '@ember/object/mixin';
import $ from 'jquery';
import DS from 'ember-data';
import { validator } from 'ember-cp-validations';
import { attr, belongsTo, hasMany } from 'ember-flexberry-data/utils/attributes';

export let Model = Mixin.create({
  createTime: DS.attr('date'),
  creator: DS.attr('string'),
  editor: DS.attr('string'),
  editTime: DS.attr('date'),
});

export let ValidationRules = {
  createTime: {
    descriptionKey: 'models.work-type.validations.createTime.__caption__',
    validators: [
      validator('ds-error'),
      validator('date'),
    ],
  },
  creator: {
    descriptionKey: 'models.work-type.validations.creator.__caption__',
    validators: [
      validator('ds-error'),
    ],
  },
  editor: {
    descriptionKey: 'models.work-type.validations.editor.__caption__',
    validators: [
      validator('ds-error'),
    ],
  },
  editTime: {
    descriptionKey: 'models.work-type.validations.editTime.__caption__',
    validators: [
      validator('ds-error'),
      validator('date'),
    ],
  },
};

export let defineBaseModel = function (modelClass) {
  modelClass.reopenClass({
    _parentModelName: 'tmobis-core-objects-dictionary'
  });
};

export let defineProjections = function (modelClass) {
  modelClass.defineProjection('AuditView', 'work-type', {
    createTime: attr('', { index: 0 }),
    creator: attr('', { index: 1 }),
    editTime: attr('', { index: 2 }),
    editor: attr('', { index: 3 }),
    name: attr('', { index: 4 }),
    actuality: attr('', { index: 5 })
  });

  modelClass.defineProjection('WorkTypeE', 'work-type', {
    name: attr('', { index: 0 }),
    actuality: attr('', { index: 1 })
  });

  modelClass.defineProjection('WorkTypeL', 'work-type', {
    name: attr('', { index: 0 }),
    actuality: attr('', { index: 1 })
  });
};
