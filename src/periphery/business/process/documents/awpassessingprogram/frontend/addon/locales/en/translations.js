import $ from 'jquery';
import AwpAssessingProgramsForm from './forms/awpAssessingPrograms';
import AwpAssessingProgramForm from './forms/awpAssessingProgram';
import AwpAssessingProgram from './models/awpAssessingProgram';

const translations = {};

$.extend(true, translations, {
	models: {
		'awp-assessing-program': AwpAssessingProgram,
	},
	forms: {
		'awp-assessing-program': AwpAssessingProgramForm,
		'awp-assessing-programs': AwpAssessingProgramsForm,
	}
});

export default translations;
