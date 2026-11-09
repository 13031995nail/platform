import { useLocalSearchParams, useRouter } from "expo-router";
import { useCallback, useEffect, useState } from "react";
import { normalizePositiveIntegerParam } from "./paging";

export const DEFAULT_ENTITY_LIST_PER_PAGE = 10;
export const MAX_ENTITY_LIST_PER_PAGE = 100;

export interface EntityListPagingData {
  page: number;
  setPage: (page: number) => void;
  perPage: number;
  setPerPage: (perPage: number | undefined) => void;
  totalCount?: number;
  canGoNext: boolean;
}

interface Props {
  useUrlParams?: boolean;
  defaultPerPage?: number;
}

export default function useEntityListPaging({
  useUrlParams = true, defaultPerPage
}: Props): EntityListPagingData {
  const router = useRouter();
  const params = useLocalSearchParams<{
    page?: string | string[];
    perPage?: string | string[];
  }>();
  const [page, setPageState] = useState<number>(() =>
    useUrlParams ? normalizePositiveIntegerParam(params.page, 1) ?? 1 : 1
  );
  const [perPage, setPerPageState] = useState<number>(() =>
    normalizePerPage(useUrlParams ? params.perPage : undefined, defaultPerPage)
  );

  useEffect(() => {
    if (!useUrlParams) {
      return;
    }
    setPageState(normalizePositiveIntegerParam(params.page, 1) ?? 1);
    setPerPageState(normalizePerPage(params.perPage, defaultPerPage));
  }, [defaultPerPage, params.page, params.perPage, useUrlParams]);

  const setPage = useCallback((newPage: number) => {
    const nextPage = newPage < 1 ? 1 : newPage;
    setPageState(nextPage);
    if (useUrlParams) {
      router.setParams({ page: nextPage.toString() });
    }
  }, [router, useUrlParams]);

  const setPerPage = useCallback((newPerPage: number | undefined) => {
    const nextPerPage = normalizePerPage(newPerPage?.toString());
    setPerPageState(nextPerPage);
    setPage(1);
    if (useUrlParams) {
      router.setParams({ perPage: nextPerPage.toString() });
    }
  }, [router, setPage, useUrlParams]);

  return {
    page,
    setPage,
    perPage,
    setPerPage,
    canGoNext: true,
  };
}

function normalizePerPage(value: string | string[] | undefined, defaultPerPage?: number) {
  const finalDefaultPerPage = defaultPerPage ?? DEFAULT_ENTITY_LIST_PER_PAGE;
  const perPage = normalizePositiveIntegerParam(value, finalDefaultPerPage)
    ?? finalDefaultPerPage;

  return Math.min(perPage, MAX_ENTITY_LIST_PER_PAGE);
}
