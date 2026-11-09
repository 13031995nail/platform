import { useLocalSearchParams, useRouter } from "expo-router";
import { useCallback, useEffect, useState } from "react";
import { getNextSorting, normalizeSortingParam } from "./sorting";

interface Props {
  onSortingChanged: () => void;
  useUrlParams: boolean;
}

export default function useEntityListSorting(
  { onSortingChanged, useUrlParams }: Props
  ) {
  const router = useRouter();
  const params = useLocalSearchParams<{ sorting?: string | string[] }>();
  const [sorting, setSorting] = useState<string | undefined>(() =>
    normalizeSortingParam(useUrlParams ? params.sorting : undefined)
  );

  useEffect(() => {
    if (useUrlParams) {
      setSorting(normalizeSortingParam(params.sorting));
    }
  }, [params.sorting, useUrlParams]);

  const toggleSorting = useCallback((column: string) => {
    const nextSorting = getNextSorting(sorting, column);
    setSorting(nextSorting);
    if (useUrlParams) {
      router.setParams({ sorting: nextSorting });
    }
    onSortingChanged();
  }, [onSortingChanged, router, sorting, useUrlParams]);

  return {
    sorting,
    toggleSorting,
  };
}
