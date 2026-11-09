export function initialize(application) {
  application.inject('controller', 'offlineBackupManager', 'service:offline/offline-backup-manager');
  application.inject('controller', 'modals', 'service:modals');

  const applicationRoute = application
    .lookup('route:application');
  const offlineUser = application
    .lookup('service:offline/offline-user');
  const offlineState = application
    .lookup('service:offline-globals');
  const authenticate = application.lookup('service:auth/authenticate');
  const offlineBackupManager = application
    .lookup('service:offline/offline-backup-manager');
  const offlineBackupJob = application
    .lookup('service:offline/offline-backup-job');
  authenticate.authSuccessful
    .on(offlineState.onAuthSuccessful.bind(offlineState));
  authenticate.authSuccessful
    .on(offlineUser.onAuthSuccessful.bind(offlineUser));
  authenticate.sessionInvalidated
    .on(offlineUser.onSessionInvalidated.bind(offlineUser));
  applicationRoute.appRouteInitialized
    .on(offlineState.onAppRouteInitialized.bind(offlineState));
  authenticate.authSuccessful
    .on(offlineBackupManager.onAuthSuccessful.bind(offlineBackupManager));
  applicationRoute.appRouteInitialized
    .on(offlineBackupJob.initializeTimer.bind(offlineBackupJob));
}

export default {
  initialize
};