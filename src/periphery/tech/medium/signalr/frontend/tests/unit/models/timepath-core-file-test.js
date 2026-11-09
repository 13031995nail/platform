import { moduleForModel, test } from 'ember-qunit';

moduleForModel('timepath-core-file', 'Unit | Model | timepath-core-file', {
  // Specify the other units that are required for this test.
  needs: [
    'model:timepath-core-actual-object',
    'model:timepath-core-app-setting',
    'model:timepath-core-application-log',
    'model:timepath-core-changelog',
    'model:timepath-core-dictionary',
    'model:timepath-core-faq-entry',
    'model:timepath-core-faq-section',
    'model:timepath-core-file',
    'model:timepath-core-search-log',
    'model:timepath-core-session',
    'model:timepath-core-town',
    'model:timepath-core-user',
    'validator:ds-error',
    'validator:presence',
    'validator:number',
    'validator:date',
    'validator:belongs-to',
    'validator:has-many',
    'service:syncer',
  ],
});

test('it exists', function(assert) {
  let model = this.subject();

  assert.ok(!!model);
});
