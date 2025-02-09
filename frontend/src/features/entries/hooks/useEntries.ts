import { useEffect, useState } from "react";
import { getEntries } from "@/features/entries/api/get-entries";
import { Entry } from "@/types/app";

export const useEntries = () => {
  const [entries, setEntries] = useState<Entry[]>([]);
  const [count, setCount] = useState<number>(25);
  const [nextCursor, setNextCursor] = useState<string | null>(null);
  const [cursorStack, setCursorStack] = useState<string[]>([]);
  const [entryPopup, setEntryPopup] = useState<Entry | null>(null);

  // Fetch Entries
  const fetchEntries = async (cursorId: string | null, previous: boolean = false) => {
    const pageRequest = { PageSize: count, CursorId: cursorId };

    const response = await getEntries({ pageRequest });

    setEntries(response.data);
    setNextCursor(response.paging.nextCursor);

    // Don't add cursor to stack if moving backwards
    if (!previous && cursorId !== null) {
      setCursorStack((prevStack) => [...prevStack, cursorId]);
    }
  };

  // Refresh entries when count changes
  useEffect(() => {
    setCursorStack([]);
    setNextCursor(null);
    fetchEntries(null);
  }, [count]);

  // Handle Entry Click
  const handleEntryClick = (rowId: string) => {
    const entry = entries.find((entry) => entry.id === rowId);
    if (!entry) return console.error("Entry not found");
    setEntryPopup(entry);
  };

  const handlePopupClose = () => {
    setEntryPopup(null);
  }


  return {
    entries,
    count,
    nextCursor,
    cursorStack,
    entryPopup,
    setCount,
    handleEntryClick,
    handlePopupClose,
    fetchEntries,
    setCursorStack
  };
};
