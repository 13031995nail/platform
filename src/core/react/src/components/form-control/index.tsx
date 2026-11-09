import {
	FormControlError,
	FormControlErrorIcon,
	FormControlErrorText,
	FormControlHelper,
	FormControlHelperText,
	FormControlLabel,
	FormControlLabelText
} from '@tmobis/core/components/ui/form-control';
import { AlertCircleIcon } from '@tmobis/core/components/ui/icon';

type TmobisFormControlLabelProps = {
	label?: string;
}

type TmobisFormControlMessageProps = {
	helperText?: string;
	isInvalid?: boolean;
	errorText?: string;
}

const TmobisFormControlLabel = ({
	label
}: TmobisFormControlLabelProps) => {
	return (
		<>
			{label
				? <FormControlLabel>
					<FormControlLabelText>
						{label}
					</FormControlLabelText>
				</FormControlLabel>
				: null
			}
		</>
	)
};

const TmobisFormControlMessage = ({
	helperText,
	isInvalid = false,
	errorText,
}: TmobisFormControlMessageProps) => {
	const ErrorTextComponent =
		<FormControlError>
			<FormControlErrorIcon as={AlertCircleIcon} />
			<FormControlErrorText>
				{errorText}
			</FormControlErrorText>
		</FormControlError>;
	const HelperTextComponent =
		<FormControlHelper>
			<FormControlHelperText>
				{helperText}
			</FormControlHelperText>
		</FormControlHelper>;
	return (
		<>
			{isInvalid
				? ErrorTextComponent
				: helperText
					? HelperTextComponent
					: null
			}
		</>
	)
};

export { TmobisFormControlLabel, TmobisFormControlMessage };
