import { EntriesTable } from '@/features/entries/components/EntriesTable';

const LandingRoute = () => {
  return (
    <div className='grid grid-cols-3 gap-4'>
      <div></div>
      <EntriesTable />
      <div></div>
    </div>
  );
};

export default LandingRoute;