import { tva } from '@gluestack-ui/utils/nativewind-utils';

export const dateTimePickerStyle = tva({
	base: '',
});

export const dateTimePickerTriggerStyle = tva({
	base: 'w-full min-h-10 px-3 flex-row items-center gap-3 bg-input/50 border border-transparent rounded-md overflow-hidden transition-[color,box-shadow] data-[focus=true]:outline-none data-[focus=true]:border-ring data-[focus=true]:web:ring-[3px] data-[focus=true]:web:ring-ring/30 data-[invalid=true]:border-destructive group-data-[invalid=true]/form-control:web:ring-destructive/30 data-[disabled=true]:pointer-events-none data-[disabled=true]:cursor-auto data-[disabled=true]:opacity-50',
});

export const dateTimePickerInputStyle = tva({
	base: 'flex-1 text-foreground text-sm md:text-base h-full placeholder:text-muted-foreground web:outline-none ios:leading-[0px] web:cursor-text web:data-[disabled=true]:cursor-auto',
});

export const dateTimePickerIconStyle = tva({
	base: 'text-foreground/50 fill-none',
	parentVariants: {
		size: {
			'2xs': 'h-3 w-3',
			'xs': 'h-3.5 w-3.5',
			'sm': 'h-4 w-4',
			'md': 'h-[18px] w-[18px]',
			'lg': 'h-5 w-5',
			'xl': 'h-6 w-6',
		},
	},
});

export const webTriggerInputStyle = 'w-full min-h-10 px-3 flex-row items-center gap-3 bg-input/50 border border-transparent rounded-md overflow-hidden transition-[color,box-shadow] group-data-[invalid=true]/form-control:border-destructive group-data-[invalid=true]/form-control:web:ring-destructive/30 group-data-[disabled=true]/form-control:pointer-events-none group-data-[disabled=true]/form-control:cursor-auto group-data-[disabled=true]/form-control:opacity-50 web:outline-none';

export const webTriggerInputPlaceholderStyle = 'text-muted-foreground text-sm md:text-base';

export const webTriggerInputTextStyle = 'text-foreground text-sm md:text-base';

export const dateTimePickerPopupOverlayStyle = tva({
	base: 'absolute inset-0 z-40',
});

export const dateTimePickerPopupStyle = tva({
	base: 'z-50 shadow-soft-4',
});

export const dateTimePickerTimeInputStyle = tva({
	base: 'w-full mt-2 px-3 py-1.5 border border-border rounded-md bg-input/50 text-foreground text-sm web:outline-none',
});
