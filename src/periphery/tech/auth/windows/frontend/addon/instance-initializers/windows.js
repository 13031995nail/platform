export function initialize(application) {
  const loginRoute = application
    .lookup('route:login');
  const windowsAuthService = application
    .lookup('service:windows-auth');
  
  loginRoute.loginRouteInitialized
    .on(windowsAuthService.onLoginRouteInitialized.bind(windowsAuthService));
}

export default {
  initialize
};