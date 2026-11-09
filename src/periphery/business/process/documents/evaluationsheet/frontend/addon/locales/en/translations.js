import $ from 'jquery';
import EvaluationSheetForm from './forms/evaluationSheet';
import EvaluationSheetsForm from './forms/evaluationSheets';

const translations = {};

$.extend(true, translations, {
  forms: {
    'evaluation-sheet': EvaluationSheetForm,
    'evaluation-sheets': EvaluationSheetsForm
  }
});

export default translations;
