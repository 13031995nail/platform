import { Town, TownClassName } from '@tmobis/core-business/models/town';
import UserMeta, { User } from '@tmobis/core-business/models/user';
import { useFocusEffect } from '@react-navigation/native';
import { TmobisSwitch } from '@tmobis/core/components/checkbox';
import EntityEdit, { EditFormProps } from '@tmobis/core/components/entity-edit';
import useEntityEdit from '@tmobis/core/components/entity-edit/use-entity-edit';
import { getFormStateLabel } from '@tmobis/core/components/form-state';
import { useHeader } from '@tmobis/core/components/header/context';
import { TmobisInput } from '@tmobis/core/components/input';
import { useCallback } from 'react';
import { defaultSlots, FormSlot } from '@tmobis/core/components/entity-edit/field-extension';
import { TmobisDropdownLookup } from '@tmobis/core/components/lookup';
import { useDropdownLookup } from '@tmobis/core/components/lookup/hook';
import { isEmail, requiredWhen } from '@tmobis/core/components/entity-edit/validation';

export default function UserForm({ isNew }: EditFormProps) {
  const data = useEntityEdit<User>({
    typeMeta: UserMeta,
    viewName: 'UserE',
    route: 'users',
    isNew
  });
  const townLookup = useDropdownLookup<Town, User>({
    entityType: TownClassName,
    viewName: 'TownE',
    associationName: 'town',
    form: data,
  });

  const { setHeaderData } = useHeader();

  useFocusEffect(
    useCallback(() => {
      if (isNew) {
        setHeaderData({ title: 'Новый пользователь' });
      } else if (!data.formState.isReady) {
        setHeaderData({
          title: 'Пользователь',
          subtitle: getFormStateLabel(data.formState.state),
        });
      } else {
        setHeaderData({
          title: `Пользователь ${data.form.getValues().name}`,
          subtitle: `${data.form.getValues().login}`
        });
      }
    }, [data.formState.isReady, data.formState.state, isNew, data.form.getValues, setHeaderData])
  );

  return (
    <EntityEdit {...data}>
      <FormSlot slot={defaultSlots.leftTop}>
        <TmobisInput
          fieldName="login"
          label="Логин"
          isRequired={true}
          validationRules={{
            minLength: {
              value: 3,
              message: 'Логин должен быть не короче 3 символов'
            },
            validate: {
              noSpaces: (value: unknown) =>
                !String(value ?? '').includes(' ') ||
                'Логин не должен содержать пробелы'
            }
          }}
        />
        <TmobisInput
          fieldName="name"
          label="Название"
          isRequired={true}
        />
        <TmobisInput
          fieldName="email"
          label="Почта"
          validationRules={{
            validate: {
              requiredWhenActual: requiredWhen<User>(
                (model) => Boolean(model.actuality),
                'Почта обязательна для актуального пользователя'
              ),
              email: isEmail()
            }
          }}
        />
        <TmobisDropdownLookup
          displayField="name"
          label="Город"
          placeholder="Выберите значение..."
          {...townLookup}
        />
        <TmobisSwitch
          fieldName="actuality"
          label="Актуальность"
        />
      </FormSlot>
    </EntityEdit>
  );
}
