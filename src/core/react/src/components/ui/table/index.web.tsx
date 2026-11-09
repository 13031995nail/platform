import React, { createContext, useMemo, useContext } from 'react';
import {
  tableStyle,
  tableHeaderStyle,
  tableBodyStyle,
  tableFooterStyle,
  tableHeadStyle,
  tableRowStyleStyle,
  tableDataStyle,
  tableCaptionStyle,
} from './styles';
import {TR as ExpoTR} from "@expo/html-elements/build/elements/Table";

const TableHeaderContext = createContext<{
  isHeaderRow: boolean;
}>({
  isHeaderRow: false,
});
const TableFooterContext = createContext<{
  isFooterRow: boolean;
}>({
  isFooterRow: false,
});

type ITableRowProps = React.ComponentProps<'tr'> & {
  data?: any | undefined;
  onPress?: (data?: any | undefined) => void;
};

type ITableDataProps = React.ComponentProps<'td'> & {
  data?: any | undefined;
  onPress?: (data?: any | undefined) => void;
};
type ITableHeadProps = React.ComponentProps<'th'> & {
  onPress?: () => void;
};

const Table = React.forwardRef<HTMLTableElement, React.ComponentProps<'table'>>(
  function Table({ className, ...props }, ref) {
    return (
      <table
        ref={ref}
        className={tableStyle({ class: className })}
        {...props}
      />
    );
  }
);

const TableHeader = React.forwardRef<
  HTMLTableSectionElement,
  React.ComponentProps<'thead'>
>(function TableHeader({ className, ...props }, ref) {
  const contextValue = useMemo(() => {
    return {
      isHeaderRow: true,
    };
  }, []);
  return (
    <TableHeaderContext.Provider value={contextValue}>
      <thead
        ref={ref}
        className={tableHeaderStyle({ class: className })}
        {...props}
      />
    </TableHeaderContext.Provider>
  );
});

const TableBody = React.forwardRef<
  HTMLTableSectionElement,
  React.ComponentProps<'tbody'>
>(function TableBody({ className, ...props }, ref) {
  return (
    <tbody
      ref={ref}
      className={tableBodyStyle({ class: className })}
      {...props}
    />
  );
});

const TableFooter = React.forwardRef<
  HTMLTableSectionElement,
  React.ComponentProps<'tfoot'>
>(function TableFooter({ className, ...props }, ref) {
  const contextValue = useMemo(() => {
    return {
      isFooterRow: true,
    };
  }, []);
  return (
    <TableFooterContext.Provider value={contextValue}>
      <tfoot
        ref={ref}
        className={tableFooterStyle({ class: className })}
        {...props}
      />
    </TableFooterContext.Provider>
  );
});

const TableHead = React.forwardRef<
  HTMLTableCellElement,
  ITableHeadProps
>(function TableHead({ className, onPress, onClick, ...props }, ref) {
  const handleClick = (event: React.MouseEvent<HTMLTableCellElement>) => {
    onClick?.(event);
    onPress?.();
  };

  return (
    <th
      ref={ref}
      className={tableHeadStyle({ class: className })}
      onClick={handleClick}
      {...props}
    />
  );
});

const TableRow = React.forwardRef<
  HTMLTableRowElement,
  ITableRowProps
>(function TableRow({ className, data, onPress, ...props }, ref) {
  const { isHeaderRow } = useContext(TableHeaderContext);
  const { isFooterRow } = useContext(TableFooterContext);
  const onPressInternal = (event: any) => { onPress?.(data); };
  
  return (
    <tr
      ref={ref}
      className={tableRowStyleStyle({
        isHeaderRow,
        isFooterRow,
        class: className,
      })}
      onClick={onPressInternal}
      {...props}
    />
  );
});

const TableData = React.forwardRef<
  HTMLTableCellElement,
  ITableDataProps
>(function TableData({ className, onPress, data, ...props }, ref) {
  const onPressInternal = (event: any) => { onPress?.(data); };
  return (
    <td
      ref={ref}
      className={tableDataStyle({ class: className })}
      onClick={onPressInternal}
      {...props} />
  );
});

const TableCaption = React.forwardRef<
  HTMLTableCaptionElement,
  React.ComponentProps<'caption'>
>(function TableCaption({ className, ...props }, ref) {
  return (
    <caption
      ref={ref}
      className={tableCaptionStyle({ class: className })}
      {...props}
    />
  );
});

Table.displayName = 'Table';
TableHeader.displayName = 'TableHeader';
TableBody.displayName = 'TableBody';
TableFooter.displayName = 'TableFooter';
TableHead.displayName = 'TableHead';
TableRow.displayName = 'TableRow';
TableData.displayName = 'TableData';
TableCaption.displayName = 'TableCaption';

export {
  Table,
  TableHeader,
  TableBody,
  TableFooter,
  TableHead,
  TableRow,
  TableData,
  TableCaption,
};
