import React, {
  createContext,
  useContext,
  useState,
  type ReactNode,
} from 'react';
import type { ImageSourcePropType } from 'react-native';

type HeaderData = {
  title: string;
  subtitle?: string;
};

type HeaderContextType = {
  headerData: HeaderData;
  setHeaderData: (data: HeaderData) => void;
  logoSource?: ImageSourcePropType;
  setLogoSource: (source: ImageSourcePropType | undefined) => void;
};

const HeaderContext = createContext<HeaderContextType>({
  headerData: { title: '' },
  setHeaderData: () => { },
  setLogoSource: () => { },
});

const HeaderProvider = ({
  children,
  logoSource: initialLogoSource,
}: {
  children: ReactNode;
  logoSource?: ImageSourcePropType;
}) => {
  const [headerData, setHeaderData] = useState<HeaderData>({ title: '' });
  const [logoSource, setLogoSource] = useState<ImageSourcePropType | undefined>(
    initialLogoSource
  );

  return (
    <HeaderContext.Provider
      value={{
        headerData,
        setHeaderData,
        logoSource,
        setLogoSource,
      }}
    >
      {children}
    </HeaderContext.Provider>
  );
};

const useHeader = () => {
  return useContext(HeaderContext);
};

export {
  HeaderProvider,
  useHeader,
};