import React, { useMemo } from 'react';
import { createBrowserRouter } from 'react-router';
import { RouterProvider } from 'react-router';
import { paths } from '@/config/paths';
import LandingRoute from '@/app/routes/landing';
import EntryRoute from '@/app/routes/entry';
import CharacterRoute from '@/app/routes/character';

const createAppRouter = () => 
  createBrowserRouter([
    {
      path: paths.home.path,
      element: <LandingRoute />
    },
    {
      path: paths.entry.path,
      element: <EntryRoute />
    },
    {
      path: paths.character.path,
      element: <CharacterRoute />
    }
  ]);

export const AppRouter = () => {
  const router = useMemo(() => createAppRouter(), []);
  return <RouterProvider router={router} />;
};