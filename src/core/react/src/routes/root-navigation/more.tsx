import React, { useCallback } from 'react';
import { useFocusEffect } from '@react-navigation/native';
import { useModules } from '@tmobis/core/modules/provider';
import { MoreMenu } from '@tmobis/core/components/more-menu';
import { useHeader } from '@tmobis/core/components/header/context';

/**
 * Экран дополнительных разделов для вкладки «Ещё» на мобиле.
 */
export default function MoreScreen() {
  const { compositionRoot } = useModules();
  const { setHeaderData } = useHeader();
  const { secondary, secondaryGroups } = compositionRoot.getNavigationPlan();

  useFocusEffect(
    useCallback(() => {
      setHeaderData({ title: 'Ещё' });
    }, [setHeaderData])
  );

  return <MoreMenu groups={secondaryGroups} screens={secondary} />;
}
