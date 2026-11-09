import $ from 'jquery';
import TmobisActTypeLForm from './forms/act-type-l';
import TmobisActTypeEForm from './forms/act-type-e';
import TmobisActTypeModel from './models/act-type';

import ActStatus from './models/act-status';
import ActStatusesForm from './forms/act-statuses';
import ActStatusForm from './forms/act-status';
import DocumentsForm from './forms/documents';

import TmobisBaseActModel from './models/base-act';

const translations = {};

$.extend(true, translations, {
    models: {
        'base-act': TmobisBaseActModel,
        'act-type': TmobisActTypeModel,
        'act-status': ActStatus,
    },
    forms: {
        'act-type-l': TmobisActTypeLForm,
        'act-type-e': TmobisActTypeEForm,
        'act-statuses': ActStatusesForm,
        'act-status': ActStatusForm,
        'documents': DocumentsForm,
    }
});

export default translations;
