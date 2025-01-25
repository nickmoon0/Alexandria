import { Entry } from '@/types/app';
import { useEffect, useState } from 'react';
import { getEntries } from '@/features/entries/api/get-entries';

export const EntriesTable = () => {
  const [entries, setEntries] = useState<Entry[]>([]);
  const [count, setCount] = useState<number>(5);
  const [nextCursor, setNextCursor] = useState<string | null>(null);
  const [cursorStack, setCursorStack] = useState<string[]>([]);

  useEffect(() => {
    // Clear stack and cursor on each refresh to prevent place-tracking problems
    setCursorStack([]);
    setNextCursor(null);
    fetchEntries(null);
  }, [count]);
  
  const fetchEntries = async (cursorId:string | null, previous:boolean = false) => {
    const pageRequest = {
      PageSize: count,
      CursorId: cursorId,
    };

    const response = await getEntries({ pageRequest });

    setEntries(response.data);
    setNextCursor(response.paging.nextCursor);

    // Dont add cursor to stack if moving backwards
    if (!previous && cursorId !== null) {
      setCursorStack((prevStack) => [...prevStack, cursorId]);
    }
  };

  const handleItemsPerPageChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setCount(Number(e.target.value));
  };

  const handleNextPage = () => {
    if (nextCursor) {
      fetchEntries(nextCursor);
    }
  };

  const handlePreviousPage = () => {
    if (cursorStack.length > 0) {
      // Get the second last cursor for going back
      const prevCursor = cursorStack[cursorStack.length - 2];
      setCursorStack((prevStack) => prevStack.slice(0, -1));
      fetchEntries(prevCursor, true);
    }
  };

  return (
    <div className="container mx-auto px-4">
      {/* Page records drop down */}
      <div className="flex justify-between items-center mb-4">
        <div className="flex items-center space-x-2">
          <label htmlFor="itemsPerPage" className="text-sm font-medium text-gray-600">
            Items per page:
          </label>
          <select
            id="itemsPerPage"
            className="bg-white border border-gray-300 rounded-md py-1 px-3 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            onChange={handleItemsPerPageChange}
            value={count}
          >
            <option value="1">1</option>
            <option value="2">2</option>
            <option value="5">5</option>
          </select>
        </div>
      </div>

      {/* Table */}
      <div className="overflow-x-auto rounded-lg shadow-lg">
        <table className="min-w-full bg-white border border-gray-200">
          <thead className="bg-gray-50 border-b">
            <tr>
              <th className="py-4 px-6 text-left text-sm font-semibold text-gray-600 tracking-wider">ID</th>
              <th className="py-4 px-6 text-left text-sm font-semibold text-gray-600 tracking-wider">Name</th>
              <th className="py-4 px-6 text-left text-sm font-semibold text-gray-600 tracking-wider">Description</th>
              <th className="py-4 px-6 text-left text-sm font-semibold text-gray-600 tracking-wider">Created At</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-200">
            {entries.map(entry => (
              <tr className="hover:bg-gray-50" key={entry.id}>
                <td className="py-4 px-6 text-gray-600">{entry.id}</td>
                <td className="py-4 px-6 text-gray-600">{entry.name}</td>
                <td className="py-4 px-6 text-gray-600">{entry.description}</td>
                <td className="py-4 px-6 text-gray-600">{entry.createdAtUtc.toString()}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* Pagination controls */}
      <div className="flex justify-between items-center mt-4">
        <button
          className="bg-gray-200 text-gray-600 py-2 px-4 rounded-md hover:bg-gray-300 disabled:bg-gray-100 disabled:text-gray-400"
          onClick={handlePreviousPage}
          disabled={cursorStack.length <= 0}>
          Previous Page
        </button>
        <button
          className="bg-gray-200 text-gray-600 py-2 px-4 rounded-md hover:bg-gray-300 disabled:bg-gray-100 disabled:text-gray-400"
          onClick={handleNextPage}
          disabled={!nextCursor}>
          Next Page
        </button>
      </div>
    </div>
  );
};