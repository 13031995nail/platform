import { ReactNode } from 'react';
import { Box } from '../ui/box';

export interface ScreenContainerProps {
	children: ReactNode;
	className?: string;
}

/**
 * Контейнер экрана с адаптивными отступами:
 * мобилка — 16px (p-4), веб — 24–32px (md:p-6 / lg:p-8).
 */
export default function ScreenContainer({
	children,
	className,
}: ScreenContainerProps) {
	return (
		<Box
			className={[
				'flex-1 w-full p-4 md:p-6 lg:p-8',
				className,
			].filter(Boolean).join(' ')}
		>
			{children}
		</Box>
	);
}
