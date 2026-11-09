import Component from '@ember/component';
import { inject as service } from '@ember/service';
import { computed } from '@ember/object';
import config from 'ember-get-config';
import FilterOperator from 'ember-flexberry-data/query/filter-operator';
import Builder from 'ember-flexberry-data/query/builder';
import { SimplePredicate } from 'ember-flexberry-data/query/predicate';

export default Component.extend({
  store: service(),

  tmobisDashboardId: undefined,
  
  init() {
    this._super(...arguments);
    this.setProperties({
      loading: true,
      error: null,
      metabaseDashboardId: null,
      metabaseUrl: config.APP.metabaseURL || null
    });
    
    this.loadDashboard();
  },

  async loadDashboard() {
    try {
      const metabaseDashboardId = await this.getDashboardId();
      
      if (!metabaseDashboardId) {
        throw new Error(`Dashboard not found for id ${this.tmobisDashboardId}`);
      }
      
      this.set('metabaseDashboardId', metabaseDashboardId);
    } catch (error) {
      this.set('error', error.message);
      console.error('Dashboard load failed:', error);
    } finally {
      this.set('loading', false);
    }
  },

  async getDashboardId() {
    const store = this.store;
    const builder = new Builder(store)
      .from('dashboard')
      .selectByProjection('DashboardE')
      .where(new SimplePredicate('tmobisDashboardId', FilterOperator.Eq, this.tmobisDashboardId));
    
    try {
      const result = await store.query('dashboard', builder.build());
      if (result.get('length') > 0) {
        const dashboard = result.get('firstObject');
        return dashboard.get('metabaseDashboardId');
      }
      return null;
    } catch (error) {
      console.error('Error fetching dashboard:', error);
      throw error;
    }
  },

  dashboardUrl: computed('metabaseUrl', 'metabaseDashboardId', function() {
    const metabaseUrl = this.metabaseUrl;
    const metabaseDashboardId = this.metabaseDashboardId;

    if (!metabaseUrl || !metabaseDashboardId) {
      return null;
    }

    return `${metabaseUrl}/public/dashboard/${metabaseDashboardId}`;
  })
});