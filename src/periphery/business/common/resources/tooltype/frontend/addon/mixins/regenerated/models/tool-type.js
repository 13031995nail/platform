import Mixin from '@ember/object/mixin';
import DS from 'ember-data';
import { validator } from 'ember-cp-validations';
import { attr } from 'ember-flexberry-data/utils/attributes';

export let Model = Mixin.create({
  createTime: DS.attr('date'),
  creator: DS.attr('string'),
  editor: DS.attr('string'),
  editTime: DS.attr('date'),
});

export let ValidationRules = {
  createTime: {
    descriptionKey: 'models.tool-type.validations.createTime.__caption__',
    validators: [
      validator('ds-error'),
      validator('date'),
    ],
  },
  creator: {
    descriptionKey: 'models.tool-type.validations.creator.__caption__',
    validators: [
      validator('ds-error'),
    ],
  },
  editor: {
    descriptionKey: 'models.tool-type.validations.editor.__caption__',
    validators: [
      validator('ds-error'),
    ],
  },
  editTime: {
    descriptionKey: 'models.tool-type.validations.editTime.__caption__',
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
  modelClass.defineProjection('AuditView', 'tool-type', {
    createTime: attr('', { index: 0 }),
    creator: attr('', { index: 1 }),
    editTime: attr('', { index: 2 }),
    editor: attr('', { index: 3 }),
    name: attr('', { index: 4 }),
    actuality: attr('', { index: 5 })
  });

  modelClass.defineProjection('ToolTypeE', 'tool-type', {
    name: attr('', { index: 0 }),
    actuality: attr('', { index: 1 })
  });

  modelClass.defineProjection('ToolTypeL', 'tool-type', {
    name: attr('', { index: 0 }),
    actuality: attr('', { index: 1 })
  });
};
