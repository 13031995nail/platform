import $ from 'jquery';
import EvaluationSheetForm from './forms/evaluationSheet';
import EvaluationSheetsForm from './forms/evaluationSheets';

const translations = {};

$.extend(true, translations, {
  forms: {
    'evaluation-sheet': EvaluationSheetForm,
    'evaluation-sheets': EvaluationSheetsForm
  },
  models: {
    'evaluation-sheet': {
      validations: {
        awp: {
          __caption__: 'Осенне-зимний период'
        },
        organization: {
          __caption__: 'Организация'
        },
        workObject: {
          __caption__: 'Объект'
        },
      }
    }
  }
});

export default translations;
