import Button from '@/components/button';
import { Entry } from "@/types/app";
import React, { useEffect, useState } from "react";
import { getFile } from '../api/get-file';
import MediaViewer from '@/components/MediaViewer';

export interface EntryPopupProps {
  entry: Entry;
  onClose: () => void; // Function to close popup
}

const EntryPopup: React.FC<EntryPopupProps> = ({ entry, onClose }) => {
  if (!entry) return null; // Prevent rendering if no entry

  return (
    <div className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50 z-50">
      {/* Popup Card */}
      <div className="bg-white p-6 rounded-2xl shadow-2xl max-w-lg w-full relative animate-fadeIn">
        {/* Close Button */}
        <button
          className="absolute top-4 right-4 text-gray-500 hover:text-gray-800 text-lg"
          onClick={onClose}
        >
          ✖
        </button>

        {/* Content */}
        <h1 className="text-2xl font-bold text-gray-800 mb-3">{entry.name}</h1>
        <p className="text-gray-600 mb-4">Description: {entry.description}</p>
        <p className="text-gray-600 mb-4">Created By: {entry.createdBy.firstName} {entry.createdBy.lastName}</p>
        <p className="text-gray-600 mb-4">Created At: {entry.createdAtUtc.toString()}</p>
        <MediaViewer documentId={entry.document.id} />
        <div className="flex justify-end">
          <Button onClick={onClose}>
            Close
          </Button>
        </div>
      </div>
    </div>
  );
};

export default EntryPopup;
