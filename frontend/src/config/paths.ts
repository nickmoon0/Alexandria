import { Route } from '@/types/config';

export const paths: Record<string, Route> = {
  home: {
    path: '/',
    getHref: () => '/',
  }
} as const;