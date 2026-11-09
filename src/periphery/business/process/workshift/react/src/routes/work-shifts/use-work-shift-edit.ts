import { useFocusEffect } from '@react-navigation/native';
import { useCallback, useMemo } from 'react';
import { useLocalSearchParams, useRouter } from 'expo-router';
import useEntityEdit from '@tmobis/core/components/entity-edit/use-entity-edit';
import { getFormStateLabel } from '@tmobis/core/components/form-state';
import { useHeader } from '@tmobis/core/components/header/context';
import { useLookup } from '@tmobis/core/components/lookup/hook';
import useEntityList from '@tmobis/core/components/entity-list/use-entity-list';
import { useGetEntity } from '@tmobis/core/api/hooks/useEntityApi';
import { Employee, EmployeeClassName } from '@tmobis/core-business/models/employee';
import WorkShiftMeta, { WorkShift } from '../../models/work-shift';
import {
  WorkShiftTask,
  WorkShiftTaskClassName,
} from '../../models/work-shift-task';
import {
  WorkShiftStatus,
  WorkShiftStatusClassName,
} from '../../models/work-shift-status';
import {
  EmployeeViewNames,
  WorkShiftStatusViewNames,
  WorkShiftTaskViewNames,
  WorkShiftViewNames,
} from '../../views';
import { WorkShiftStatusKeys } from '../../constants';

/**
 * Общая логика формы редактирования смены для web/native раскладок.
 */
export function useWorkShiftEdit(isNew: boolean) {
  const router = useRouter();
  const {
    id,
    date,
    employeeId,
  } = useLocalSearchParams<{
    id: string;
    date?: string;
    employeeId?: string;
  }>();

  const { model: employee } = useGetEntity<Employee>({
    entityType: EmployeeClassName,
    viewName: EmployeeViewNames.Edit,
    id: isNew ? employeeId : undefined,
    enabled: isNew && !!employeeId,
  });

  const { model: workShiftStatus } = useGetEntity<WorkShiftStatus>({
    entityType: WorkShiftStatusClassName,
    viewName: WorkShiftStatusViewNames.List,
    id: isNew ? WorkShiftStatusKeys.NotApproved : undefined,
    enabled: isNew,
  });

  const defaults = useMemo((): Partial<WorkShift> | undefined => {
    if (!isNew) {
      return undefined;
    }

    const result: Partial<WorkShift> = {
      workShiftStatusId: WorkShiftStatusKeys.NotApproved,
    };

    if (workShiftStatus) {
      result.workShiftStatus = workShiftStatus;
    }

    if (date) {
      result.date = date;
    }

    if (employeeId && employee) {
      result.employeeId = employeeId;
      result.employee = employee;
    }

    return result;
  }, [date, employee, employeeId, isNew, workShiftStatus]);

  const data = useEntityEdit<WorkShift>({
    typeMeta: WorkShiftMeta,
    viewName: WorkShiftViewNames.Edit,
    route: 'work-shifts',
    isNew,
    defaults,
  });

  const employeeLookup = useLookup<Employee, WorkShift>({
    entityType: EmployeeClassName,
    viewName: EmployeeViewNames.Edit,
    associationName: 'employee',
    form: data,
  });

  const tasksListData = useEntityList<WorkShiftTask>({
    entityType: WorkShiftTaskClassName,
    viewName: WorkShiftTaskViewNames.List,
    defaultFilter: id ? { workShiftId: id } : undefined,
    useUrlParams: false,
  });

  const { setHeaderData } = useHeader();
  const workShiftStatusName = data.form.watch('workShiftStatus')?.name ?? '';

  useFocusEffect(
    useCallback(() => {
      if (isNew) {
        setHeaderData({ title: 'Новая смена' });
      } else if (!data.formState.isReady) {
        setHeaderData({
          title: 'Смена',
          subtitle: getFormStateLabel(data.formState.state),
        });
      } else {
        const values = data.form.getValues();
        setHeaderData({
          title: `Смена ${values.date ?? ''}`,
          subtitle: values.employee?.fio ?? undefined,
        });
      }
    }, [
      data.form,
      data.formState.isReady,
      data.formState.state,
      isNew,
      setHeaderData,
    ])
  );

  const onTaskRowClick = (task?: WorkShiftTask) => {
    if (task?.id) {
      router.navigate(`/work-shifts/tasks/${task.id}`);
    }
  };

  const onCreateTask = () => {
    router.navigate({
      pathname: '/work-shifts/tasks/new',
      params: { workShiftId: id },
    });
  };

  const workShiftDate = data.form.watch('date') ?? undefined;

  return {
    id,
    isNew,
    data,
    employeeLookup,
    tasksListData,
    workShiftStatusName,
    workShiftDate,
    onTaskRowClick,
    onCreateTask,
  };
}
