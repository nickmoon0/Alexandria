import { Route } from '@/types/config';

export const paths: Record<string, Route> = {
  home: {
    path: '/',
    getHref: () => '/',
  },
  entry: {
    path: '/entry/:entryId',
    getHref: (entryId?:string) => `/entry/${entryId}`
  },
  character: {
    path: '/character/:characterId',
    getHref: (characterId?:string) => `/character/${characterId}`
  }
} as const;