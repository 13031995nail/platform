import {useTranslation} from "react-i18next";

export default function useLocale() {
  const { t } = useTranslation();
  return {
    t: (path: string): string => t(path)
  }
}