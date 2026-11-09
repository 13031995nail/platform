import $ from 'jquery';
import TmobisAdmissionCommitteesForm from './forms/admission-committees';
import TmobisAdmissionCommitteeForm from './forms/admission-committee';
import TmobisAdmissionCommitteeModel from './models/admission-committee';
import TmobisAdmissionCommitteeEmployeeModel from './models/admission-committee-employee';

const translations = {};

$.extend(true, translations, {
    models: {
        'admission-committee': TmobisAdmissionCommitteeModel,
        'admission-committee-employee': TmobisAdmissionCommitteeEmployeeModel,
    },
    
    forms: {
        'admission-committees': TmobisAdmissionCommitteesForm,
        'admission-committee': TmobisAdmissionCommitteeForm,
    },
});

export default translations;