import React from 'react';

export interface TableRow<T = unknown> {
  [key: string]: T;
}

export interface Column<T> {
  key: keyof T | string;
  label: string;
  render?: (item: T) => React.ReactNode;
}

interface TableProps<T> {
  columns: Column<T>[];
  data: T[];
  onRowClick?: (item: T) => void;
}

const Table = <T extends TableRow>({ columns, data, onRowClick }: TableProps<T>) => {
  return (
    <div className='overflow-x-auto rounded-lg shadow-lg'>
      <table className='min-w-full bg-white border border-gray-200'>
        <thead className='bg-gray-50 border-b'>
          <tr>
            {columns.map((col) => (
              <th key={col.key as string} className='py-4 px-6 text-left text-sm font-semibold text-gray-600 tracking-wider'>
                {col.label}
              </th>
            ))}
          </tr>
        </thead>
        <tbody className='divide-y divide-gray-200'>
          {data.map((item, index) => (
            <tr
              key={index}
              onClick={() => onRowClick && onRowClick(item)}
              className='hover:bg-gray-50'
            >
              {columns.map((col) => (
                <td key={col.key as string} className='py-4 px-6 text-gray-600'>
                  {col.render ? col.render(item) : String(item[col.key as keyof T])}
                </td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default Table;
