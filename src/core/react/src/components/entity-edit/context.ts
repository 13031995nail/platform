import { createContext, useContext } from 'react';
import {
	FieldPath,
	FieldValues,
	PathValue,
	RegisterOptions,
	useController,
	UseFormReturn
} from 'react-hook-form';

interface ContextProps {
	form: UseFormReturn<any>;
}

export const EntityEditContext = createContext<ContextProps | null>(null);

export const useEntityEditContext = () => useContext(EntityEditContext);

interface UseEntityEditFieldProps<
	TFieldValues extends FieldValues = FieldValues,
	TName extends FieldPath<TFieldValues> = FieldPath<TFieldValues>
> {
	fieldName?: TName;
	isRequired?: boolean;
	defaultValue?: PathValue<TFieldValues, TName>;
	validationRules?: Omit<
		RegisterOptions<TFieldValues, TName>,
		'valueAsNumber' | 'valueAsDate' | 'setValueAs' | 'disabled'
	>;
}

export const useEntityEditController = ({
	fieldName,
	isRequired,
	defaultValue,
	validationRules
}: UseEntityEditFieldProps) => {
	const context = useEntityEditContext();
	const rules = {
		required: isRequired
			? validationRules?.required ?? 'Поле обязательно для заполнения'
			: validationRules?.required,
		...validationRules,
	};

	return context && fieldName
		? useController({
			name: fieldName,
			control: context.form.control,
			defaultValue,
			rules
		})
		: undefined;
}

export const useEntityEditField = (props: UseEntityEditFieldProps) => useEntityEditController(props)?.field;

export const useEntityEditFieldState = ({
	...props
}: UseEntityEditFieldProps) => {
	const controller = useEntityEditController(props);
	const field = controller?.field;
	const error = controller?.fieldState.error;
	const errorText = error?.message;
	const isInvalid = Boolean(error);

	return {
		controller,
		field,
		error,
		errorText,
		isInvalid
	};
};
