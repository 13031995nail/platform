import { tva } from '@gluestack-ui/utils/nativewind-utils';
import { isWeb } from '@gluestack-ui/utils/nativewind-utils';

const captionTableStyle = isWeb ? 'caption-bottom' : '';

export const tableStyle = tva({
  base: `table border-collapse w-[800px]`,
});

export const tableHeaderStyle = tva({
  base: 'bg-background-muted',
});

export const tableBodyStyle = tva({
  base: '',
});

export const tableFooterStyle = tva({
  base: '',
});

export const tableHeadStyle = tva({
  base: 'flex-1 px-3 py-2.5 text-left text-sm font-medium leading-5 text-foreground-secondary bg-background-muted font-roboto whitespace-nowrap align-middle',
});

export const tableRowStyleStyle = tva({
  base: 'border-0 border-b border-solid border-border bg-background hover:bg-background-secondary odd:bg-background-secondary/50 min-h-12',
  variants: {
    isHeaderRow: {
      true: 'border-b border-border bg-background-muted hover:bg-background-muted odd:bg-background-muted min-h-0',
    },
    isFooterRow: {
      true: 'border-b-0 min-h-0',
    },
  },
});

export const tableDataStyle = tva({
  base: 'flex-1 px-3 py-2 text-left text-sm font-normal leading-5 text-foreground align-middle font-roboto',
});

export const tableCaptionStyle = tva({
  base: `${captionTableStyle} px-3 py-3 text-sm font-normal leading-5 text-foreground-secondary bg-background font-roboto`,
});
