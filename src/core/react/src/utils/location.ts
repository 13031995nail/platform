import * as Location from 'expo-location';

export type Coords  = { latitude: number, longitude: number } | null;

export const getCurrentLocation = async (): Promise<Coords> => {
    let { status } = await Location.requestForegroundPermissionsAsync();
    if (status !== 'granted') {
        return null;
    }
    let loc = await Location.getCurrentPositionAsync({});
    return { latitude: loc.coords.latitude, longitude: loc.coords.longitude };
};