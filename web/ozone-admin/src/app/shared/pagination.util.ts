export const DEFAULT_PAGE_SIZE = 10;

export function paginateSlice<T>(items: readonly T[], page: number, pageSize: number): T[] {
  const start = (page - 1) * pageSize;
  return items.slice(start, start + pageSize);
}

export function clampPage(page: number, totalCount: number, pageSize: number): number {
  const maxPage = Math.max(1, Math.ceil(totalCount / pageSize) || 1);
  return Math.min(Math.max(1, page), maxPage);
}
