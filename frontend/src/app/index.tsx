import { AppProvider } from '@/app/provider';
import { AppRouter } from '@/app/router';

export const App = () => {
  return (
    <AppProvider>
      <h1 className='pt-4 text-center text-3xl font-bold'>Alexandria</h1>
      <AppRouter />
    </AppProvider>
  );
};