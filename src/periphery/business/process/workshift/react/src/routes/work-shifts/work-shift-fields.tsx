import { TmobisSimpleInput } from '@tmobis/core/components/input';
import { TmobisDatetime } from '@tmobis/core/components/datetime';
import { TmobisLookup } from '@tmobis/core/components/lookup';
import {
  FormControl,
  FormControlLabel,
  FormControlLabelText,
} from '@tmobis/core/components/ui/form-control';
import type { LookupData } from '@tmobis/core/components/lookup/hook';
import type { Employee } from '@tmobis/core-business/models/employee';

function ReadonlyField({
  label,
  value,
}: {
  label: string;
  value?: string | null;
}) {
  return (
    <FormControl isDisabled>
      <FormControlLabel>
        <FormControlLabelText>{label}</FormControlLabelText>
      </FormControlLabel>
      <TmobisSimpleInput value={value ?? ''} isDisabled />
    </FormControl>
  );
}

type WorkShiftFieldsProps = {
  workShiftStatusName: string;
  employeeLookup: LookupData<Employee>;
};

/**
 * Основные поля карточки смены (статус, дата, сотрудник).
 */
export function WorkShiftFields({
  workShiftStatusName,
  employeeLookup,
}: WorkShiftFieldsProps) {
  return (
    <>
      <ReadonlyField label="Статус" value={workShiftStatusName} />
      <TmobisDatetime fieldName="date" label="Дата" mode="date" isRequired />
      <TmobisLookup
        displayField="fio"
        label="Сотрудник"
        placeholder="Выберите сотрудника..."
        columns={[
          {
            column: 'fio',
            title: 'ФИО',
          },
          {
            column: 'email',
            title: 'Email',
          },
        ]}
        {...employeeLookup}
      />
    </>
  );
}
