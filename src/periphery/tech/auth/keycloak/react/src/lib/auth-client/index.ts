import * as WebBrowser from 'expo-web-browser';
import type { ApiClient } from '@tmobis/core/api/create-api-client';
import * as Crypto from 'expo-crypto';
import { Platform } from 'react-native';
import { keycloakConfig, getOpenIdConfigUrl } from '../config';
import {BackendAccessToken, StoredTokens} from '../types';
import {getBackendToken} from "@tmobis/auth-keycloak/lib/auth-service";
import tokenStorage from "@tmobis/auth-keycloak/lib/storage";

async function getDiscovery(issuerUrl: string): Promise<Record<string, unknown>> {
  let discoveryUrl = issuerUrl;
  if (!discoveryUrl.endsWith('/.well-known/openid-configuration')) {
    discoveryUrl = `${discoveryUrl}/.well-known/openid-configuration`;
  }
  const response = await fetch(discoveryUrl);
  if (!response.ok) {
    throw new Error(`Discovery fetch failed: ${response.status}`);
  }
  return response.json();
}

function mapToStoredTokens(tokenResponse: any, backendToken: BackendAccessToken): StoredTokens {
  return {
    accessToken: tokenResponse.access_token || '',
    idToken: tokenResponse.id_token || '',
    backendAccessToken: backendToken
  };
}

function generateRandomString(length: number): string {
  const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
  let result = '';
  for (let i = 0; i < length; i++) {
    result += chars.charAt(Math.floor(Math.random() * chars.length));
  }
  return result;
}

function generateCodeVerifier(): string {
  const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-._~';
  let result = '';
  for (let i = 0; i < 43; i++) {
    result += chars.charAt(Math.floor(Math.random() * chars.length));
  }
  return result;
}

async function generateCodeChallenge(verifier: string): Promise<string> {
  if (Platform.OS === 'web') {
    const encoder = new TextEncoder();
    const data = encoder.encode(verifier);
    const hash = await window.crypto.subtle.digest('SHA-256', data);
    let base64 = btoa(String.fromCharCode(...new Uint8Array(hash)));
    base64 = base64.replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/, '');
    return base64;
  }

  const hashHex = await Crypto.digestStringAsync(Crypto.CryptoDigestAlgorithm.SHA256, verifier);
  const byteArray = [];
  for (let i = 0; i < hashHex.length; i += 2) {
    byteArray.push(parseInt(hashHex.substring(i, i + 2), 16));
  }
  let base64 = btoa(String.fromCharCode(...byteArray));
  base64 = base64.replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/, '');
  return base64;
}

export const authClient = {
  async login(client: ApiClient): Promise<StoredTokens> {
    const discovery = await getDiscovery(getOpenIdConfigUrl());
    const authorizationEndpoint = (discovery as any).authorization_endpoint;
    const tokenEndpoint = (discovery as any).token_endpoint;

    const codeVerifier = generateCodeVerifier();
    const codeChallenge = await generateCodeChallenge(codeVerifier);

    const state = generateRandomString(32);
    const nonce = generateRandomString(32);

    const authUrl = new URL(authorizationEndpoint);
    authUrl.searchParams.set('client_id', keycloakConfig.clientId);
    authUrl.searchParams.set('redirect_uri', keycloakConfig.redirectUrl);
    authUrl.searchParams.set('response_type', 'code');
    authUrl.searchParams.set('scope', keycloakConfig.scopes.join(' '));
    authUrl.searchParams.set('state', state);
    authUrl.searchParams.set('nonce', nonce);
    authUrl.searchParams.set('code_challenge', codeChallenge);
    authUrl.searchParams.set('code_challenge_method', 'S256');

    let result: any;
    try {
      result = await WebBrowser.openAuthSessionAsync(
        authUrl.toString(),
        keycloakConfig.redirectUrl
      );
    } catch (error: any) {
      throw error;
    }

    if (result?.type === 'cancel') {
      throw new Error('authorization_failed');
    }

    if (!result?.url) {
      throw new Error('authorization_failed');
    }

    try {
      const url = new URL(result.url);
      const code = url.searchParams.get('code');
      const responseState = url.searchParams.get('state');

      if (!code) {
        throw new Error('authorization_failed: No code received');
      }

      if (responseState !== state) {
        throw new Error('authorization_failed: Invalid state');
      }
    } catch (urlError: any) {
      throw new Error('authorization_failed');
    }

    const url = new URL(result.url);
    const code = url.searchParams.get('code');

    const formData = new URLSearchParams();
    formData.set('client_id', keycloakConfig.clientId);
    formData.set('redirect_uri', keycloakConfig.redirectUrl);
    formData.set('code', code || '');
    formData.set('grant_type', 'authorization_code');
    formData.set('code_verifier', codeVerifier);

    try {
      const tokenResponse = await fetch(tokenEndpoint, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/x-www-form-urlencoded',
        },
        body: formData.toString(),
      });

      if (!tokenResponse.ok) {
        const errorData = await tokenResponse.text().catch(() => '');
        throw new Error(`Token exchange failed: ${tokenResponse.status} ${errorData}`);
      }

      const tokenData = await tokenResponse.json();
      const backendToken = await getBackendToken(tokenData.access_token, client);
      const mapped = mapToStoredTokens(tokenData, backendToken);
      await tokenStorage.saveTokens(mapped);
      return mapped;
    } catch (error: any) {
      throw error;
    }
  },

  async logout(idToken: string): Promise<void> {
    if (Platform.OS === 'web') {
      const discovery = await getDiscovery(getOpenIdConfigUrl());
      const endSessionEndpoint = typeof discovery === 'object' && discovery !== null && 'endSessionEndpoint' in discovery
        ? (discovery.endSessionEndpoint as string)
        : '';
      const endSessionUrl = `${endSessionEndpoint}?id_token_hint=${idToken}&post_logout_redirect_uri=${encodeURIComponent(keycloakConfig.webRedirectUrl)}`;
      globalThis.location.href = endSessionUrl;
    }
  },
};

const _checkAuthClient = authClient;

export default authClient;