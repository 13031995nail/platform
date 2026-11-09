import Service from '@ember/service';

/**
 * Сервис определения сущностей для офлайн работы
 */
export default Service.extend({
  backupEntities: undefined,
  clearIgnoreEntities: undefined,

  init() {
    this._super(...arguments);
    
    if (!this.backupEntities)
    {
      this.backupEntities = [];
    }
    
    if (!this.clearIgnoreEntities)
    {
      this.clearIgnoreEntities = [];
    }
  },

  // eslint-disable-next-line no-unused-vars
  getConditionalEntities(params) {
    return [];
  }
});