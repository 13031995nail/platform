import { moduleForModel, test } from 'ember-qunit';

moduleForModel('timepath-core-search-log', 'Unit | Serializer | timepath-core-search-log', {
  // Specify the other units that are required for this test.
  needs: [
    'serializer:timepath-core-search-log',
    'service:syncer',
    'transform:file',
    'transform:decimal',
    'transform:guid',

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
  ],
});

// Replace this with your real tests.
test('it serializes records', function(assert) {
  let record = this.subject();

  let serializedRecord = record.serialize();

  assert.ok(serializedRecord);
});
