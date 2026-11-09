import { Platform } from 'react-native';
import { TmobisAlert } from '@tmobis/core/components/alert';
import { TmobisButton } from '@tmobis/core/components/button';
import { Box } from '../ui/box';
import { Heading } from '../ui/heading';
import { HStack } from '../ui/hstack';
import { Text } from '../ui/text';
import { VStack } from '../ui/vstack';
import type { FormHeaderProps } from './types';
import { useFormLeaveGuard } from './use-form-leave-guard';

export type { FormHeaderProps } from './types';

/**
 * Шапка формы с кнопками действий.
 * Мобилка: фиксирована сверху. Веб: на всю ширину.
 */
export default function FormHeader({
	title = 'Редактирование',
	isDirty,
	isSubmitting,
	submit,
	back,
	discardAndBack,
	saveError,
	backLabel = 'Назад к списку',
	saveLabel = 'Сохранить',
	children,
}: FormHeaderProps) {
	const isWeb = Platform.OS === 'web';
	const guard = useFormLeaveGuard({
		isDirty,
		isSubmitting,
		submit,
		back,
		discardAndBack,
		saveError,
	});

	const titleBlock = (
		<VStack className="flex-1 min-w-0">
			<Heading size={isWeb ? 'xl' : 'lg'}>{title}</Heading>
			<Text className="text-sm text-muted-foreground mt-1">
				{isDirty
					? 'Есть несохраненные изменения'
					: 'Все изменения сохранены'}
			</Text>
		</VStack>
	);

	const actionsBlock = (
		<HStack className={isWeb ? 'ml-4 gap-3 shrink-0' : 'w-full gap-3 justify-end'}>
			{children}
			<TmobisButton
				label={backLabel}
				action="secondary"
				onPress={guard.handleBack}
			/>
			<TmobisButton
				label={saveLabel}
				onPress={() => submit()}
				isDisabled={isSubmitting}
			/>
		</HStack>
	);

	return (
		<>
			<Box
				className={
					isWeb
						? 'w-full border-b border-border bg-background px-6 py-4 lg:px-8'
						: 'w-full z-10 border-b border-border bg-background px-4 py-3'
				}
				style={!isWeb ? { elevation: 2 } : undefined}
			>
				{isWeb ? (
					<HStack className="items-center justify-between gap-4">
						{titleBlock}
						{actionsBlock}
					</HStack>
				) : (
					<VStack className="gap-3">
						{titleBlock}
						{actionsBlock}
					</VStack>
				)}
			</Box>

			<TmobisAlert
				isOpen={guard.isConfirmationOpened}
				message={guard.alertMessage}
				isLoading={isSubmitting}
				onClose={guard.handleClose}
				isApproveVisible={!guard.isSaveErrorVisible}
				onApprove={guard.handleApprove}
				isDenyVisible={!guard.isSaveErrorVisible}
				onDeny={guard.handleDeny}
			/>
		</>
	);
}
