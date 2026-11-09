import { useCallback, useEffect, useState } from "react";
import { useCreateEntity, useGetEntity, useUpdateEntity } from "../../api/hooks/useEntityApi";
import { useLocalSearchParams, useRouter } from 'expo-router';
import { toNonEmptyString } from "../../utils/nonEmptyString";
import { DefaultValues, FieldErrors, FieldValues, useForm, UseFormReturn } from 'react-hook-form';
import { ModelMeta } from "../../models/model-meta";
import { IEntity } from "../../models/entity";
import { formatError } from "../../api/errors";
import {
  FormStateController,
  useFormState,
  useFormStateLoading,
} from "../form-state";

export interface EntityEditTab {
  name: string;
  title: string;
}

export interface UseEntityEditProps<TEntity extends IEntity> {
  typeMeta: ModelMeta<TEntity>,
  viewName?: string,
  route: string,
  isNew: boolean,
  defaults?: Partial<TEntity>,
}

export interface UseEntityEditData<TEntity extends IEntity & FieldValues> {
  form: UseFormReturn<TEntity>;
  submit: (closeAfterSave?: boolean) => Promise<boolean>;
  back: () => void;
  discardAndBack: () => void;
  errors: FieldErrors<TEntity>;
  isDirty: boolean;
  formState: FormStateController;
  isLoading: boolean;
  isSubmitting: boolean;
  route: string;
  getValues: () => TEntity;
  saveError?: string;
  refreshData: () => Promise<void>;
}

export default function useEntityEdit<TEntity extends IEntity & FieldValues>({
  typeMeta: {
    className,
    defaults: modelDefaults
  },
  viewName,
  route,
  isNew,
  defaults: formDefaults,
}: UseEntityEditProps<TEntity>): UseEntityEditData<TEntity> {
  const router = useRouter();
  const formState = useFormState();
  const [saveError, setSaveError] = useState<string>();

  const form = useForm<TEntity>({
    defaultValues: {
      ...(modelDefaults as object),
      ...(isNew && formDefaults ? formDefaults : {}),
    } as DefaultValues<TEntity>,
    mode: 'onChange',
    reValidateMode: 'onChange',
  });
  const {
    reset,
    handleSubmit,
    formState: {
      errors,
      isDirty,
      isSubmitting
    },
    getValues
  } = form;

  const handleNotFound = () => {
    alert('НЕ НАЙДЕНА СУЩНОСТЬ!');
  };

  const { id } = useLocalSearchParams<{ id: string }>();
  const entityId = !isNew && id && id.length > 0
    ? toNonEmptyString(id)
    : undefined;
  const { model, isLoadingModel, refetch } = useGetEntity<TEntity>({
    id: entityId,
    entityType: className,
    viewName: viewName,
    onNotFound: handleNotFound
  });

  useFormStateLoading(formState, !isNew && isLoadingModel);

  useEffect(() => {
    if (!isNew && !isLoadingModel) {
      if (model) {
        reset(model);
      } else {
        router.navigate(`/${route}/new`);
      }
    }
  }, [id, isLoadingModel, isNew, model, reset, route, router]);

  // Подстановка/обновление дефолтов новой формы (в т.ч. после загрузки связанных сущностей).
  useEffect(() => {
    if (!isNew || formDefaults === undefined) {
      return;
    }

    reset(
      {
        ...(modelDefaults as object),
        ...getValues(),
        ...formDefaults,
      } as TEntity,
      { keepDirtyValues: true }
    );
  }, [formDefaults, getValues, isNew, modelDefaults, reset]);

  const { updateEntity } = useUpdateEntity<TEntity>({
    entityType: className,
    id: entityId,
  });
  const { createEntity } = useCreateEntity<TEntity>({
    entityType: className,
  });

  const onSubmit = useCallback(
    async (model: TEntity, closeAfterSave = false) => {
      try {
        formState.beginSaving();
        setSaveError(undefined);

        // TODO: как-нибудь получше?
        const data = model as Record<string, unknown>;
        Object.keys(data).forEach(key => {
          if (data[key] === null || data[key] === '') {
            data[key] = undefined;
          }
        });

        if (isNew) {
          const response = await createEntity(model);
          if (closeAfterSave) {
            router.back();
          } else {
            router.replace(`/${route}/${response.id}`);
          }
        } else {
          await updateEntity(model!);
          if (closeAfterSave) {
            router.back();
          }
        }
        formState.endSaving();
        return true;
      } catch (err) {
        console.error("Ошибка при обновлении: ", err);
        const message = err instanceof Error
          ? formatError(err)
          : 'Не удалось сохранить изменения';

        setSaveError(typeof message === 'string' ? message : JSON.stringify(message));
        formState.endSaving();
        return false;
      }
    },
    [createEntity, formState, isNew, route, router, updateEntity]
  );

  const submit = useCallback(
    async (closeAfterSave = false) => {
      let saved = false;

      await handleSubmit(async (model) => {
        saved = await onSubmit(model, closeAfterSave);
      })();

      return saved;
    },
    [handleSubmit, onSubmit]
  );

  const back = useCallback(() => {
    router.back();
  }, [router]);

  const discardAndBack = useCallback(() => {
    reset({
      ...(modelDefaults as object),
      ...(isNew && formDefaults ? formDefaults : {}),
    } as TEntity);
    router.back();
  }, [formDefaults, isNew, modelDefaults, reset, router]);

  const refreshData = useCallback(async () => {
    await refetch();
  }, [refetch]);

  return {
    formState,
    isLoading: !formState.isReady,
    form,
    errors,
    isDirty,
    isSubmitting,
    route,
    submit,
    back,
    getValues,
    discardAndBack,
    saveError,
    refreshData
  };
}