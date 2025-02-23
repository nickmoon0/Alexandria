import React from 'react';
import { formatDateTime } from '@/lib/helpers';
import { User } from '@/types/app';

export interface MetadataTagProps {
  createdBy:User,
  createdAtUtc:Date,
  id:string
};

const MetadataTag = ({ createdBy, createdAtUtc, id }:MetadataTagProps) => {
  return (
    <div className="bg-gray-100 p-4 rounded-lg mt-6 text-gray-700 text-sm">
      <p className="mb-2">
        <span className="font-semibold">Created By: </span>{createdBy.firstName} {createdBy.lastName}
      </p>
      <p className="mb-2">
        <span className="font-semibold">Created At: </span>{formatDateTime(createdAtUtc)}
      </p>
      <p className="mb-2">
        <span className="font-semibold">Record ID: </span>{id}
      </p>
    </div>
  );
};

export default MetadataTag;