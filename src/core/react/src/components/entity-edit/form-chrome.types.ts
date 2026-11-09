import type { ReactNode } from 'react';

export type FormTabView = {
  id: string;
  title: string;
  content: ReactNode;
};

export type FormChromeProps = {
  isDirty: boolean;
  isSubmitting: boolean;
  submit: (closeAfterSave?: boolean) => Promise<boolean>;
  back: () => void;
  discardAndBack: () => void;
  saveError?: string;
  /** Склейка вкладок в одно полотно, как useTabsAnchorMode в МИМК. */
  flattenTabs?: boolean;
  hasTabs: boolean;
  tabs: FormTabView[];
};
