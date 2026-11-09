require('dotenv').config();

const APP_SCHEME = 'expoapp';
const backendUrl = process.env.REACT_BACKEND_URL || "http://localhost:5000";

module.exports = {
  expo: {
    name: "expo-app",
    slug: "expo-app",
    version: "1.0.0",
    orientation: "portrait",
    icon: "./assets/images/icon.png",
    scheme: APP_SCHEME,
    userInterfaceStyle: "automatic",
    extra: {
      backendUrl,
      signalrUrl: process.env.REACT_SIGNALR_URL || `${backendUrl.replace(/\/$/, '')}/signalr`,
      keycloakUrl: process.env.EXPO_PUBLIC_KEYCLOAK_URL || "https://kk.timepath.ru",
      keycloakRealm: process.env.EXPO_PUBLIC_KEYCLOAK_REALM || "2234",
      keycloakIssuer: process.env.EXPO_PUBLIC_KEYCLOAK_ISSUER || "https://kk.timepath.ru/realms/2234",
      keycloakClientId: process.env.EXPO_PUBLIC_KEYCLOAK_CLIENT_ID || "api",
      keycloakRedirectUrl: APP_SCHEME + "://oauthredirect",
      keycloakWebRedirectUrl: process.env.EXPO_PUBLIC_KEYCLOAK_WEB_REDIRECT_URL || "http://localhost:8081/",
    },
    ios: {
      icon: "./assets/expo.icon",
      bundleIdentifier: "com.anonymous.expoapp",
      schemes: [APP_SCHEME]
    },
    android: {
      adaptiveIcon: {
        backgroundColor: "#E6F4FE",
        foregroundImage: "./assets/images/android-icon-foreground.png",
        backgroundImage: "./assets/images/android-icon-background.png",
        monochromeImage: "./assets/images/android-icon-monochrome.png"
      },
      predictiveBackGestureEnabled: false,
      package: "com.anonymous.expoapp",
      intentFilters: [
        {
          action: "VIEW",
          autoVerify: true,
          data: [
            {
              scheme: "expoapp",
              host: "oauthredirect",
            },
          ],
          category: ["BROWSABLE", "DEFAULT"],
        },
      ],
    },
    web: {
      output: "single",
      favicon: "./assets/images/favicon.png",
      lang: "ru",
      themeColor: "#208AEF"
    },
    plugins: [
      "expo-router",
      [
        "expo-splash-screen",
        {
          "backgroundColor": "#208AEF",
          "android": {
            "image": "./assets/images/splash-icon.png",
            "imageWidth": 76
          }
        }
      ],
      "expo-font",
      "expo-image",
      ["expo-network-security-config", {
        "networkSecurityConfig": "./network_security_config.xml",
        "enable": true
      }]
    ],
    experiments: {
      typedRoutes: true,
      reactCompiler: true
    }
  }
}
