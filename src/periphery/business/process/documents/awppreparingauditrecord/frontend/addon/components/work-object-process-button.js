import FlexberryButton from 'ember-flexberry/components/flexberry-button';

export default FlexberryButton.extend({
  tagName: 'button',
  layoutName: 'components/flexberry-button',
  attributeBindings: ['type', 'readonly:disabled', 'ariaPressed:aria-pressed'],
  type: 'button',
  ariaPressed: null
});
