import { type ReactNode } from 'react';

import { Toggler, type TogglerProps } from '@tmobis/core/components/toggler';

type SpoilerProps = {
  children: ReactNode;
  name?: string;
  isOpen?: boolean;
  isDisabled?: boolean;
  onExpandedChange?: TogglerProps['onExpandedChange'];
  className?: string;
};

/**
 * @deprecated Используйте `Toggler` из `@tmobis/core/components/toggler`
 */
const Spoiler = ({
  children,
  name,
  isOpen,
  isDisabled,
  onExpandedChange,
  className,
}: SpoilerProps) => {
  return (
    <Toggler
      title={name}
      defaultExpanded={isOpen}
      isDisabled={isDisabled}
      onExpandedChange={onExpandedChange}
      className={className}
    >
      {children}
    </Toggler>
  );
};

export { Spoiler };
export type { SpoilerProps };
