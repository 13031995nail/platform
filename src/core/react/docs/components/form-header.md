# FormHeader

Переиспользуемая шапка формы с кнопками действий и подтверждением при несохранённых изменениях.

## Импорт

```tsx
import FormHeader from '@tmobis/core/components/form-header';
```

## Поведение

- **Мобилка**: фиксирована сверху (вне `ScrollView`), кнопки под заголовком.
- **Веб**: на всю ширину, заголовок слева, кнопки справа.
- При уходе с dirty формы открывается `TmobisAlert` (сохранить / отбросить / отмена).

## Пример

```tsx
<FormHeader
  title="Редактирование"
  isDirty={isDirty}
  isSubmitting={isSubmitting}
  submit={submit}
  back={back}
  discardAndBack={discardAndBack}
  saveError={saveError}
/>
```

## Пропсы

| Проп | Тип | Описание |
|------|-----|----------|
| `title` | `string` | Заголовок (по умолчанию «Редактирование») |
| `isDirty` | `boolean` | Есть несохранённые изменения |
| `isSubmitting` | `boolean` | Идёт сохранение |
| `submit` | `(closeAfterSave?: boolean) => Promise<boolean>` | Сохранение |
| `back` | `() => void` | Назад без подтверждения (если форма чистая) |
| `discardAndBack` | `() => void` | Сброс и выход |
| `saveError` | `string?` | Текст ошибки сохранения |
| `backLabel` / `saveLabel` | `string?` | Подписи кнопок |
| `children` | `ReactNode?` | Доп. кнопки перед «Назад» / «Сохранить» |

Используется внутри `EntityEdit`. Стили — через кастомную тему (`border-border`, `bg-background`, `text-muted-foreground`).
