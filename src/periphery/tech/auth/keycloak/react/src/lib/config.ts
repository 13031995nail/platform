import { Platform } from 'react-native';
import Constants from 'expo-constants';

export const keycloakConfig = {
  issuer: (() => {
    const keycloakUrl = Constants.expoConfig?.extra?.keycloakUrl;
    const keycloakRealm = Constants.expoConfig?.extra?.keycloakRealm;
    let issuer = Constants.expoConfig?.extra?.keycloakIssuer;

    if (!issuer) {
      issuer = `${keycloakUrl}/realms/${keycloakRealm}`;
    }

    return issuer.replace(/\/$/, '').replace('/.well-known/openid-configuration', '');
  })(),
  clientId: Constants.expoConfig?.extra?.keycloakClientId,
  nativeRedirectUrl: Constants.expoConfig?.extra?.keycloakRedirectUrl,
  webRedirectUrl: Constants.expoConfig?.extra?.keycloakWebRedirectUrl,
  redirectUrl: Platform.OS === 'web'
    ? Constants.expoConfig?.extra?.keycloakWebRedirectUrl
    : Constants.expoConfig?.extra?.keycloakRedirectUrl,
  scopes: ['openid', 'profile', 'email'],
};

export const getKeycloakIssuer = (): string => {
  return keycloakConfig.issuer;
};

export const getOpenIdConfigUrl = (): string => {
  return keycloakConfig.issuer;
};