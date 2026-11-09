import {
	TmobisAsyncModal,
	TmobisModal,
	TmobisModalProps,
	TmobisModalPromise
} from '@tmobis/core/components/modal';

type TmobisAlertProps = Omit<
	TmobisModalProps,
	| 'size'
	| 'showCloseButton'
	| 'children'
>;

type TmobisAsyncAlertProps = TmobisAlertProps & {
	promise?: TmobisModalPromise,
}

const TmobisAlert = ({
	...modalProps
}: TmobisAlertProps) => {
	return (
		<TmobisModal {...modalProps} />
	);
};


const TmobisAsyncAlert = ({
	isDenyVisible = true,
	...modalProps
}: TmobisAsyncAlertProps) => {
	return (
		<TmobisAsyncModal
			isDenyVisible={isDenyVisible}
			{...modalProps}
		/>
	);
};

export { TmobisAlert, TmobisAsyncAlert };
