import { useFocusEffect } from '@react-navigation/native';
import { useCallback, useEffect, useMemo } from 'react';
import { useLocalSearchParams } from 'expo-router';
import EntityEdit, { EditFormProps } from '@tmobis/core/components/entity-edit';
import useEntityEdit from '@tmobis/core/components/entity-edit/use-entity-edit';
import { useEntityEditFieldState } from '@tmobis/core/components/entity-edit/context';
import { getFormStateLabel } from '@tmobis/core/components/form-state';
import { useHeader } from '@tmobis/core/components/header/context';
import { TmobisInput } from '@tmobis/core/components/input';
import { TmobisButton } from '@tmobis/core/components/button';
import {
  TmobisDatetime,
  TmobisSimpleDatetime,
} from '@tmobis/core/components/datetime';
import {
  normalizeDate,
} from '@tmobis/core/components/datetime/use-datetime';
import {
  FormControl,
  FormControlError,
  FormControlErrorText,
  FormControlLabel,
  FormControlLabelText,
} from '@tmobis/core/components/ui/form-control';
import { HStack } from '@tmobis/core/components/ui/hstack';
import { Textarea, TextareaInput } from '@tmobis/core/components/ui/textarea';
import { VStack } from '@tmobis/core/components/ui/vstack';
import { useGetEntity } from '@tmobis/core/api/hooks/useEntityApi';
import {
  TmobisFormControlLabel,
  TmobisFormControlMessage,
} from '@tmobis/core/components/form-control';
import WorkShiftTaskMeta, { WorkShiftTask } from '../../../models/work-shift-task';
import {
  WorkShift,
  WorkShiftClassName,
} from '../../../models/work-shift';
import {
  WorkShiftStatus,
  WorkShiftStatusClassName,
} from '../../../models/work-shift-status';
import { WorkShiftStatusKeys } from '../../../constants';
import {
  WorkShiftStatusViewNames,
  WorkShiftTaskViewNames,
  WorkShiftViewNames,
} from '../../../views';
import { useApiClient } from '@tmobis/core/api/api-client-context';
import { createMessageApi } from '@tmobis/core/api/services/message-api';
import { StartWorkShiftTask } from '../../../messages/start-work-shift-task';
import { FinishWorkShiftTask } from '../../../messages/finish-work-shift-task';
import { getCurrentLocation } from '@tmobis/core/utils/location';
import {
  combineShiftDateAndTime,
  getWorkShiftDate,
} from '../../../utils/planned-start-time';

function NoteField() {
  const { field, errorText, isInvalid } = useEntityEditFieldState({
    fieldName: 'note',
  });

  return (
    <FormControl isInvalid={isInvalid}>
      <FormControlLabel>
        <FormControlLabelText>Примечание</FormControlLabelText>
      </FormControlLabel>
      <Textarea>
        <TextareaInput
          value={field?.value ?? ''}
          onChangeText={field?.onChange}
          onBlur={field?.onBlur}
          multiline
          numberOfLines={6}
          placeholder="Введите примечание"
        />
      </Textarea>
      {isInvalid && errorText ? (
        <FormControlError>
          <FormControlErrorText>{errorText}</FormControlErrorText>
        </FormControlError>
      ) : null}
    </FormControl>
  );
}

function PlannedStartTimeField({
  workShiftDate,
}: {
  workShiftDate?: string | null;
}) {
  const { field, errorText, isInvalid } = useEntityEditFieldState({
    fieldName: 'plannedStartTime',
  });

  const handleChange = useCallback(
    (date: Date | undefined) => {
      if (!date) {
        field?.onChange(undefined);
        return;
      }

      const nextValue = combineShiftDateAndTime(workShiftDate, date);
      field?.onChange(nextValue ?? field.value);
    },
    [field, workShiftDate]
  );

  return (
    <FormControl isInvalid={isInvalid}>
      <TmobisFormControlLabel label="Плановое время начала" />
      <TmobisSimpleDatetime
        value={normalizeDate(field?.value)}
        onChange={handleChange}
        mode="time"
        is24Hour
        placeholder="Выберите время"
      />
      <TmobisFormControlMessage
        isInvalid={isInvalid}
        errorText={errorText}
      />
    </FormControl>
  );
}

export default function WorkShiftTaskEditForm({ isNew }: EditFormProps) {
  const client = useApiClient();
  const messageApi = useMemo(() => createMessageApi(client), [client]);
  const { workShiftId } = useLocalSearchParams<{
    workShiftId?: string;
  }>();

  const { model: workShift } = useGetEntity<WorkShift>({
    entityType: WorkShiftClassName,
    viewName: WorkShiftViewNames.Edit,
    id: isNew ? workShiftId : undefined,
    enabled: isNew && !!workShiftId,
  });

  const { model: workShiftStatus } = useGetEntity<WorkShiftStatus>({
    entityType: WorkShiftStatusClassName,
    viewName: WorkShiftStatusViewNames.List,
    id: isNew ? WorkShiftStatusKeys.NotApproved : undefined,
    enabled: isNew,
  });

  const defaults = useMemo((): Partial<WorkShiftTask> | undefined => {
    if (!isNew) {
      return undefined;
    }

    const result: Partial<WorkShiftTask> = {
      workShiftStatusId: WorkShiftStatusKeys.NotApproved,
    };

    if (workShiftStatus) {
      result.workShiftStatus = workShiftStatus;
    }

    if (workShiftId && workShift) {
      result.workShiftId = workShiftId;
      result.workShift = workShift;
    }

    return result;
  }, [isNew, workShift, workShiftId, workShiftStatus]);

  const data = useEntityEdit<WorkShiftTask>({
    typeMeta: WorkShiftTaskMeta,
    viewName: WorkShiftTaskViewNames.Edit,
    route: 'work-shifts/tasks',
    isNew,
    defaults,
  });

  const { setHeaderData } = useHeader();
  const workShiftDate = getWorkShiftDate(
    data.form.watch('workShift'),
    workShift?.date,
  );

  const actualStartTime = data.form.getValues('actualStartTime');
  const actualEndTime = data.form.getValues('actualEndTime');

  // Привязка даты планового начала к дате смены при её появлении/смене.
  useEffect(() => {
    if (!workShiftDate) {
      return;
    }

    const plannedStartTime = data.form.getValues('plannedStartTime');
    if (!plannedStartTime) {
      return;
    }

    const nextValue = combineShiftDateAndTime(workShiftDate, plannedStartTime);
    if (nextValue && nextValue !== plannedStartTime) {
      data.form.setValue('plannedStartTime', nextValue, {
        shouldDirty: false,
      });
    }
  }, [data.form, workShiftDate, data.formState.isReady]);

  useFocusEffect(
    useCallback(() => {
      if (isNew) {
        setHeaderData({ title: 'Новый пункт смены' });
      } else if (!data.formState.isReady) {
        setHeaderData({
          title: 'Пункт смены',
          subtitle: getFormStateLabel(data.formState.state),
        });
      } else {
        setHeaderData({
          title: 'Пункт смены',
          subtitle: workShiftDate || undefined,
        });
      }
    }, [
      data.formState.isReady,
      data.formState.state,
      isNew,
      setHeaderData,
      workShiftDate,
    ])
  );

  const submit = useCallback(
    async (closeAfterSave = false) => {
      const values = data.form.getValues();
      const shiftDate = getWorkShiftDate(values.workShift, workShiftDate);
      const plannedStartTime = values.plannedStartTime;

      if (shiftDate && plannedStartTime) {
        const normalized = combineShiftDateAndTime(shiftDate, plannedStartTime);
        if (normalized) {
          data.form.setValue('plannedStartTime', normalized, {
            shouldDirty: true,
          });
        }
      }

      return data.submit(closeAfterSave);
    },
    [data, workShiftDate]
  );

  const startStopWork = async () => {
    data.formState.beginLoading();
    const id = data.form.watch('id');
    const currentActualStartTime = data.form.getValues('actualStartTime') ?? '';
    const time = new Date().toISOString();
    const coord = await getCurrentLocation();
    const dataTask = {
      latitude: coord?.latitude,
      longitude: coord?.longitude,
      time: time,
      workShiftTaskId: id,
    };
    const workShiftTask =
      currentActualStartTime === ''
        ? new StartWorkShiftTask(dataTask)
        : new FinishWorkShiftTask(dataTask);

    await messageApi.sendMessage(workShiftTask);
    await data.refreshData();
    data.formState.endLoading();
  };

  return (
    <EntityEdit {...data} submit={submit}>
      <HStack space="md" className="items-start">
        <VStack space="sm" className="flex-1">
          <TmobisDatetime
            fieldName="workShift.date"
            label="Дата смены"
            isDisabled
            is24Hour
            mode="date"
          />
          <TmobisInput
            fieldName="workShiftStatus.name"
            label="Статус"
            isDisabled
          />
          <PlannedStartTimeField workShiftDate={workShiftDate} />
          {!isNew ? (
            <TmobisButton
              isDisabled={actualEndTime ? true : false}
              label={actualStartTime ? 'Закончить работу' : 'Начать работу'}
              onPress={() => {
                startStopWork();
              }}
            />
          ) : null}
        </VStack>
        <VStack space="sm" className="flex-1">
          <NoteField />
        </VStack>
      </HStack>
    </EntityEdit>
  );
}
