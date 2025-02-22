import { createContext, ReactNode, useContext, useState } from "react";

export interface EntriesContextProps {
  count:number;
  entriesRefresh:number;
  setCount: (count:number) => void;
  triggerEntriesRefresh: () => void;
};

export interface EntriesRefreshProviderProps {
  children: ReactNode;
};

const EntriesContext = createContext<EntriesContextProps>({ 
  entriesRefresh:0,
  triggerEntriesRefresh: () => {},
  count:0,
  setCount: () => {}
});

export const EntriesRefreshProvider = ({ children }:EntriesRefreshProviderProps) => {
  const [entriesRefresh, setEntriesRefresh] = useState<number>(0);
  const [count, setCount] = useState<number>(25);

  const triggerEntriesRefresh = () => setEntriesRefresh((prev) => prev + 1);

  return (
    <EntriesContext.Provider value={{
      count,
      entriesRefresh,
      setCount,
      triggerEntriesRefresh
    }}>
      {children}
    </EntriesContext.Provider>
  );
};

export const useEntriesRefresh = () => useContext(EntriesContext);
