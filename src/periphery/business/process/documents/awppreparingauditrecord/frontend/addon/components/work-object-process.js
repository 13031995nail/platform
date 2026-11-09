import Component from '@ember/component';
import { computed } from '@ember/object';
import { inject as service } from '@ember/service';
import { preparationStates, selectRequirements } from '../utils/work-object-process';

export default Component.extend({
  processProvider: service('work-object-process'),
  rolesManager: service(),
  classNames: ['new-white-panel'],
  section: 'requirements',
  process: null,
  loadError: null,
  loading: false,
  busy: false,
  actionError: null,

  stateCaption: computed('process.preparation.state', function () {
    return preparationStates[this.get('process.preparation.state')];
  }),

  requirements: computed('process.requirements.[]', 'section', function () {
    return selectRequirements(this.get('process.requirements'), this.get('section'));
  }),

  visibleDocuments: computed('process.documents.[]', 'workspace', function () {
    const roles = this.get('rolesManager');
    return (this.get('process.documents') || []).filter(document =>
      !roles.canAccessRoute || roles.canAccessRoute(document.route));
  }),

  showReadiness: computed('workspace', function () {
    return this.get('workspace') !== 'consumer';
  }),

  canReview: computed('process.canReview', 'workspace', function () {
    return this.get('process.canReview') && (this.get('workspace') !== 'consumer' ||
      this.get('rolesManager').hasRole('Член комиссии'));
  }),

  canFormAct: computed('canReview', 'process.canFormAct', function () {
    return this.get('canReview') && this.get('process.canFormAct');
  }),

  canCreateEvaluationSheet: computed('rolesManager.{activeWorkspace,roles.[]}', function () {
    return this.canCreateDocument('evaluation-sheet.new');
  }),

  canCreateReadinessAct: computed('process.documentActions.canCreateReadinessAct',
    'rolesManager.{activeWorkspace,roles.[]}', function () {
      return this.get('process.documentActions.canCreateReadinessAct') &&
        this.canCreateDocument('rediness-act.new');
    }),

  canCreateReadinessPassport: computed('process.documentActions.canCreateReadinessPassport',
    'rolesManager.{activeWorkspace,roles.[]}', function () {
      return this.get('process.documentActions.canCreateReadinessPassport') &&
        this.canCreateDocument('awp-readiness-passport.new');
    }),

  canCreateDocument(routeName) {
    const roles = this.get('rolesManager');
    return !roles.canAccessRoute || roles.canAccessRoute(routeName);
  },

  canSubmit: computed('process.canSubmit', 'workspace', function () {
    return this.get('process.canSubmit') && this.get('workspace') === 'consumer';
  }),

  canStartReview: computed('canReview', 'process.preparation.canStartReview', function () {
    return this.get('canReview') && this.get('process.preparation.canStartReview');
  }),

  canAccept: computed('canReview', 'process.preparation.canAccept', function () {
    return this.get('canReview') && this.get('process.preparation.canAccept');
  }),

  reviewDisabled: computed('busy', 'process.preparation.canChangeEvaluationSheet', function () {
    return this.get('busy') || !this.get('process.preparation.canChangeEvaluationSheet');
  }),

  didReceiveAttrs() {
    this._super(...arguments);
    const key = `${this.get('workObjectId') || ''}:${this.get('workspace') || ''}:${this.get('awpId') || ''}`;
    if (this._loadedKey !== key) {
      this._loadedKey = key;
      this.loadProcess();
    }
  },

  loadProcess() {
    if (this.isDestroyed || this.isDestroying) {
      return;
    }
    const objectId = this.get('workObjectId');
    if (!objectId) {
      return;
    }
    const request = this._request = (this._request || 0) + 1;
    this.setProperties({ loading: true, loadError: null, process: null });
    return this.get('processProvider').load(objectId, this.get('awpId')).then(process => {
      if (!this.isDestroyed && !this.isDestroying && request === this._request) {
        this.setProperties({ process, loading: false });
      }
    }).catch(() => {
      if (!this.isDestroyed && !this.isDestroying && request === this._request) {
        this.setProperties({ loading: false, loadError: 'Не удалось загрузить процесс подготовки объекта.' });
      }
    });
  },

  actions: {
    selectSection(section) {
      this.set('section', section);
    },
    refresh() {
      return this.loadProcess();
    },
    review(operation) {
      const permission = { submit: 'canSubmit', review: 'canStartReview', accept: 'canAccept' }[operation];
      if (!permission || !this.get(permission) || this.get('reviewDisabled')) {
        return;
      }
      this.setProperties({ busy: true, actionError: null });
      return this.get('processProvider').review(
        this.get('workObjectId'), this.get('process.awpId'), operation
      ).then(() => this.loadProcess()).catch(error => {
        if (!this.isDestroyed && !this.isDestroying) {
          const message = error && error.errors && error.errors[0];
          this.set('actionError', message && message.detail ||
            'Действие не выполнено. Проверьте заполнение требований, замечания и права доступа.');
        }
      }).finally(() => {
        if (!this.isDestroyed && !this.isDestroying) {
          this.set('busy', false);
        }
      });
    }
  }
});
