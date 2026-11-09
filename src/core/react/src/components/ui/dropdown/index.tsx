'use client';

import type { VariantProps } from '@gluestack-ui/utils/nativewind-utils';
import { tva } from '@gluestack-ui/utils/nativewind-utils';
import React from 'react';
import {
	Pressable,
	ScrollView,
	Text,
	View,
	type LayoutRectangle,
} from 'react-native';
import { useGluestackColors } from '../../../gluestack-ui-provider/useGluestackColors';
import { Portal } from '../portal';

const PANEL_MAX_HEIGHT = 300;
const PANEL_GAP = 4;

const dropdownStyle = tva({
	base: 'relative w-full',
});

const dropdownBackdropStyle = tva({
	base: 'absolute inset-0',
});

const dropdownPanelStyle = tva({
	base: 'rounded-md border border-border bg-background p-1 shadow-hard-5 overflow-hidden',
	variants: {
		placement: {
			inline: 'absolute left-0 right-0 top-full mt-1 max-h-[300px]',
			portal: 'absolute max-h-[300px]',
		},
	},
});

const dropdownPanelScrollStyle = tva({
	base: 'max-h-[292px] bg-background',
});

const dropdownPanelItemStyle = tva({
	base: 'min-w-full p-3 flex-row items-center rounded data-[hover=true]:bg-accent/40 active:bg-accent/50',
	variants: {
		selected: {
			true: 'bg-accent/60',
			false: '',
		},
	},
});

const dropdownSheetItemStyle = tva({
	base: '',
	variants: {
		selected: {
			true: 'bg-accent/80',
			false: '',
		},
	},
});

const dropdownItemTextStyle = tva({
	base: 'text-foreground',
	variants: {
		selected: {
			true: 'text-primary font-semibold',
			false: '',
		},
	},
});

const dropdownEmptyStyle = tva({
	base: 'text-muted-foreground text-sm p-3',
});

const dropdownLoadingStyle = tva({
	base: 'items-center',
	variants: {
		size: {
			panel: 'py-2',
			sheet: 'py-3',
		},
	},
});

const dropdownItemContentStyle = tva({
	base: 'flex-row items-center w-full',
	variants: {
		variant: {
			panel: 'gap-2',
			sheet: 'gap-3',
		},
	},
});

type IDropdownProps = React.ComponentProps<typeof View> &
	VariantProps<typeof dropdownStyle> & { className?: string };

const Dropdown = React.forwardRef<React.ComponentRef<typeof View>, IDropdownProps>(
	function Dropdown({ className, ...props }, ref) {
		return (
			<View
				ref={ref}
				{...props}
				className={dropdownStyle({ class: className })}
			/>
		);
	}
);

type IDropdownOverlayProps = React.ComponentProps<typeof Portal> & {
	isOpen: boolean;
	onClose?: () => void;
};

const DropdownOverlay = React.forwardRef<
	React.ComponentRef<typeof Portal>,
	IDropdownOverlayProps
>(function DropdownOverlay({ isOpen, onClose, children, style, ...props }, ref) {
	return (
		<Portal
			ref={ref}
			isOpen={isOpen}
			onRequestClose={onClose}
			isKeyboardDismissable
			style={[
				{
					position: 'absolute',
					top: 0,
					left: 0,
					right: 0,
					bottom: 0,
				},
				style,
			]}
			{...props}
		>
			{children}
		</Portal>
	);
});

type IDropdownBackdropProps = React.ComponentProps<typeof Pressable> &
	VariantProps<typeof dropdownBackdropStyle> & { className?: string };

const DropdownBackdrop = React.forwardRef<
	React.ComponentRef<typeof Pressable>,
	IDropdownBackdropProps
>(function DropdownBackdrop({ className, ...props }, ref) {
	return (
		<Pressable
			ref={ref}
			{...props}
			className={dropdownBackdropStyle({ class: className })}
		/>
	);
});

type IDropdownPanelProps = React.ComponentProps<typeof View> &
	VariantProps<typeof dropdownPanelStyle> & {
		className?: string;
		anchor?: LayoutRectangle | null;
	};

const DropdownPanel = React.forwardRef<
	React.ComponentRef<typeof View>,
	IDropdownPanelProps
>(function DropdownPanel(
	{ className, style, placement = 'inline', anchor, ...props },
	ref
) {
	const colors = useGluestackColors();
	const isPortal = placement === 'portal';

	const portalStyle =
		isPortal && anchor
			? {
					top: anchor.y + anchor.height + PANEL_GAP,
					left: anchor.x,
					width: anchor.width,
				}
			: undefined;

	return (
		<View
			ref={ref}
			{...props}
			style={[
				{
					backgroundColor: colors.background || '#ffffff',
					maxHeight: PANEL_MAX_HEIGHT,
					overflow: 'hidden',
				},
				portalStyle,
				style,
			]}
			className={dropdownPanelStyle({
				placement,
				class: className,
			})}
		/>
	);
});

type IDropdownPanelItemProps = React.ComponentProps<typeof Pressable> &
	VariantProps<typeof dropdownPanelItemStyle> & { className?: string };

const DropdownPanelItem = React.forwardRef<
	React.ComponentRef<typeof Pressable>,
	IDropdownPanelItemProps
>(function DropdownPanelItem({ className, selected, ...props }, ref) {
	return (
		<Pressable
			ref={ref}
			{...props}
			className={dropdownPanelItemStyle({
				selected: selected as boolean,
				class: className,
			})}
		/>
	);
});

type IDropdownItemTextProps = React.ComponentProps<typeof Text> &
	VariantProps<typeof dropdownItemTextStyle> & { className?: string };

const DropdownItemText = React.forwardRef<
	React.ComponentRef<typeof Text>,
	IDropdownItemTextProps
>(function DropdownItemText({ className, selected, ...props }, ref) {
	return (
		<Text
			ref={ref}
			{...props}
			className={dropdownItemTextStyle({
				selected: selected as boolean,
				class: className,
			})}
		/>
	);
});

type IDropdownEmptyProps = React.ComponentProps<typeof Text> &
	VariantProps<typeof dropdownEmptyStyle> & { className?: string };

const DropdownEmpty = React.forwardRef<
	React.ComponentRef<typeof Text>,
	IDropdownEmptyProps
>(function DropdownEmpty({ className, ...props }, ref) {
	return (
		<Text
			ref={ref}
			{...props}
			className={dropdownEmptyStyle({ class: className })}
		/>
	);
});

type IDropdownLoadingProps = React.ComponentProps<typeof View> &
	VariantProps<typeof dropdownLoadingStyle> & { className?: string };

const DropdownLoading = React.forwardRef<
	React.ComponentRef<typeof View>,
	IDropdownLoadingProps
>(function DropdownLoading({ className, size = 'panel', ...props }, ref) {
	return (
		<View
			ref={ref}
			{...props}
			className={dropdownLoadingStyle({
				size,
				class: className,
			})}
		/>
	);
});

type IDropdownItemContentProps = React.ComponentProps<typeof View> &
	VariantProps<typeof dropdownItemContentStyle> & { className?: string };

const DropdownItemContent = React.forwardRef<
	React.ComponentRef<typeof View>,
	IDropdownItemContentProps
>(function DropdownItemContent({ className, variant = 'panel', ...props }, ref) {
	return (
		<View
			ref={ref}
			{...props}
			className={dropdownItemContentStyle({
				variant,
				class: className,
			})}
		/>
	);
});

const DropdownPanelScroll = React.forwardRef<
	React.ComponentRef<typeof ScrollView>,
	React.ComponentProps<typeof ScrollView> & { className?: string }
>(function DropdownPanelScroll({ className, style, ...props }, ref) {
	const colors = useGluestackColors();

	return (
		<ScrollView
			ref={ref}
			{...props}
			style={[
				{ maxHeight: PANEL_MAX_HEIGHT - 8, backgroundColor: colors.background },
				style,
			]}
			className={dropdownPanelScrollStyle({ class: className })}
		/>
	);
});

Dropdown.displayName = 'Dropdown';
DropdownOverlay.displayName = 'DropdownOverlay';
DropdownBackdrop.displayName = 'DropdownBackdrop';
DropdownPanel.displayName = 'DropdownPanel';
DropdownPanelItem.displayName = 'DropdownPanelItem';
DropdownItemText.displayName = 'DropdownItemText';
DropdownEmpty.displayName = 'DropdownEmpty';
DropdownLoading.displayName = 'DropdownLoading';
DropdownItemContent.displayName = 'DropdownItemContent';
DropdownPanelScroll.displayName = 'DropdownPanelScroll';

export {
	Dropdown,
	DropdownOverlay,
	DropdownBackdrop,
	DropdownEmpty,
	DropdownItemContent,
	DropdownItemText,
	DropdownLoading,
	DropdownPanel,
	DropdownPanelItem,
	DropdownPanelScroll,
	dropdownItemTextStyle,
	dropdownSheetItemStyle,
};
