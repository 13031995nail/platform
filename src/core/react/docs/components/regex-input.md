# TmobisRegexInput
Текстовое поле с ограничением ввода по регулярному выражению.
Аналог `tmobis-regex-input` (Ember): встроенные типы масок и произвольный `regexpr`.

## Пример использования
### На форме EntityEdit
```tsx
	<TmobisRegexInput
		fieldName="amount"
		label="Сумма"
		defaultRegexType="decimal20,2"
		placeholder="0,00"
	/>
```

### Автономное использование
```tsx
	<TmobisRegexInput
		label="Код"
		regexpr="^[A-Z0-9]{0,8}$"
		value={code}
		onChangeText={setCode}
	/>
```