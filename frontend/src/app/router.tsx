import { useMemo } from 'react';
import { createBrowserRouter } from 'react-router';
import { RouterProvider } from 'react-router';
import { paths } from '../config/paths';
import LandingRoute from './routes/landing';

const createAppRouter = () => 
  createBrowserRouter([
    {
      path: paths.home.path,
      element: <LandingRoute />
    }
  ]);

export const AppRouter = () => {
  const router = useMemo(() => createAppRouter(), []);
  return <RouterProvider router={router} />;
};