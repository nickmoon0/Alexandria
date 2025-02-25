import React, { createContext, ReactNode, useContext, useState } from 'react';

export interface CharactersContextProps {
  count:number;
  charactersRefresh:number;
  setCount: (count:number) => void;
  triggerCharactersRefresh: () => void;
};

export interface CharactersProviderProps {
  children: ReactNode;
}

const CharactersContext = createContext<CharactersContextProps>({
  charactersRefresh: 0,
  triggerCharactersRefresh: () => {},
  count: 0,
  setCount: () => {}
});

export const CharactersContextProvider = ({ children }:CharactersProviderProps) => {
  const [charactersRefresh, setCharactersRefresh] = useState<number>(0);
  const [count, setCount] = useState<number>(10);

  const triggerCharactersRefresh = () => setCharactersRefresh((prev) => prev + 1);

  return (
    <CharactersContext.Provider value={{
      count,
      charactersRefresh,
      setCount,
      triggerCharactersRefresh
    }}>
      {children}
    </CharactersContext.Provider>
  );
};

export const useCharactersContext = () => useContext(CharactersContext);