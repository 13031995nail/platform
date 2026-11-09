import {Grid as GlueGrid} from "../ui/grid";
import {ReactNode} from "react";

interface Props {
  children?: ReactNode;
  columns?: number;
}

export function Grid({ children, columns = 1 }: Props) {
  return (
    <GlueGrid
      className="gap-4"
      _extra={{ className: `grid-cols-${columns}` }}
    >
      {children}
    </GlueGrid>
  );
}