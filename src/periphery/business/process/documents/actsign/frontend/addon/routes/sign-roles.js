import DictionariesListFormRoute
  from '@timepath/tmobis-core/routes/base/list-form';
import config from 'ember-get-config';
import { computed } from '@ember/object';

export default DictionariesListFormRoute.extend({
  /**
    Name of model projection to be used as record's properties limitation.

    @property modelProjection
    @type String
    @default 'SignRoleL'
  */
  modelProjection: 'SignRoleL',

  /**
    Name of model to be used as list's records types.

    @property modelName
    @type String
    @default 'sign-role'
  */
  modelName: 'sign-role',

  /**
    Defined user settings developer.
    For default userSetting use empty name ('').
    Property `<componentName>` may contain any of properties: `colsOrder`, `sorting`, `colsWidth` or being empty.

    ```javascript
    {
      <componentName>: {
        <settingName>: {
          colsOrder: [ { propName :<colName>, hide: true|false }, ... ],
          sorting: [{ propName: <colName>, direction: "asc"|"desc" }, ... ],
          colsWidths: [ <colName>:<colWidth>, ... ],
        },
        ...
      },
      ...
    }
    ```

    @property developerUserSettings
    @type Object
  */
  developerUserSettings: computed(function() {
    return { objectlistviewComponentName: {
      "DEFAULT": {
        "perPage": config.APP.resultsPerPage,
        "columnWidths": [
          { "propName": "OlvRowMenu", "fixed": true },
          { "propName": "OlvRowToolbar", "fixed": true }
        ]
      }
    } }
  }),
});
