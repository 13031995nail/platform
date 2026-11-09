declare const require: (moduleName: string) => any;

let webBrowser: any;

export function getWebBrowser(): any {
  if (!webBrowser) {
    webBrowser = require('expo-web-browser');
  }
  return webBrowser;
}

export function maybeCompleteAuthSession(): void {
  const WebBrowser = getWebBrowser();
  if (WebBrowser.maybeCompleteAuthSession) {
    WebBrowser.maybeCompleteAuthSession();
  }
}