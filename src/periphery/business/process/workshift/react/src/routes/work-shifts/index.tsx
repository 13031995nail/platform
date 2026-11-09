import { useFocusEffect } from '@react-navigation/native';
import { useCallback } from 'react';
import { useRouter } from 'expo-router';
import EntityListForm from '@tmobis/core/components/entity-list/form';
import EntityListDateRangeFilter from '@tmobis/core/components/entity-list/date-range-filter';
import EntityListLookupFilter from '@tmobis/core/components/entity-list/lookup-filter';
import useEntityList from '@tmobis/core/components/entity-list/use-entity-list';
import { useHeader } from '@tmobis/core/components/header/context';
import { Employee, EmployeeClassName } from '@tmobis/core-business/models/employee';
import {
  WorkShift,
  WorkShiftClassName,
} from '../../models/work-shift';
import {
  WorkShiftStatus,
  WorkShiftStatusClassName,
} from '../../models/work-shift-status';
import {
  EmployeeViewNames,
  WorkShiftStatusViewNames,
  WorkShiftViewNames,
} from '../../views';
import WorkShiftTaskPreview from '../../components/work-shift-task-preview';
import { WorkShiftListFormTypes } from '../../list-form-types';

export default function WorkShiftListForm() {
  const router = useRouter();
  const { setHeaderData } = useHeader();
  const listData = useEntityList<WorkShift>({
    entityType: WorkShiftClassName,
    viewName: WorkShiftViewNames.List,
  });

  useFocusEffect(
    useCallback(() => {
      setHeaderData({ title: 'Смены' });
    }, [setHeaderData])
  );

  return (
    <EntityListForm
      editRoute="work-shifts"
      listFormType={WorkShiftListFormTypes.WorkShift}
      listData={listData}
      hasFilter
      hasSearch={false}
      getRowConfig={() => ({
        mobileCardTitleColumn: 'date',
      })}
      onAdd={() => router.navigate('/work-shifts/new')}
      toolbarFilters={
        <>
          <EntityListLookupFilter<WorkShiftStatus>
            field="workShiftStatusId"
            label="Статус"
            placeholder="Выберите статус..."
            displayField="name"
            entityType={WorkShiftStatusClassName}
            viewName={WorkShiftStatusViewNames.List}
            variant="toolbar"
          />
          <EntityListLookupFilter<Employee>
            field="employeeId"
            label="Сотрудник"
            placeholder="Выберите сотрудника..."
            displayField="fio"
            entityType={EmployeeClassName}
            viewName={EmployeeViewNames.Edit}
            variant="toolbar"
          />
        </>
      }
      columns={[
        {
          column: 'date',
          title: 'Дата',
        },
        {
          column: 'employee.fio',
          title: 'Сотрудник',
        },
        {
          column: 'workShiftStatus.name',
          title: 'Статус',
        },
        {
          title: 'Пункты',
          renderComponent: (data: WorkShift) => <WorkShiftTaskPreview data={data} />,
        },
      ]}
    >
      <EntityListDateRangeFilter
        field="date"
        labelFrom="Дата от"
        labelTo="Дата до"
      />
    </EntityListForm>
  );
}
