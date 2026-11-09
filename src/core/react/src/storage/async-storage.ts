import AsyncStorage from "@react-native-async-storage/async-storage";

export const getStoredValue = async (key: string): Promise<string | undefined> => {
  const value = await AsyncStorage.getItem(key);
  return !value || value === 'undefined' ? undefined : value;
};

export const setStoredValue = async (key: string, value: string): Promise<void> => {
  await AsyncStorage.setItem(key, value);
};

export const removeStoredValue = async (key: string): Promise<void> => {
  await AsyncStorage.removeItem(key);
};
