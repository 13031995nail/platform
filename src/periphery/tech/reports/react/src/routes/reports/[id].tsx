import ReportMeta, { Report } from "../../models/report";
import { useFocusEffect } from '@react-navigation/native';
import EntityEdit, { EditFormProps } from "@tmobis/core/components/entity-edit";
import useEntityEdit from "@tmobis/core/components/entity-edit/use-entity-edit";
import { useHeader } from "@tmobis/core/components/header/context";
import { TmobisInput } from "@tmobis/core/components/input";
import { useCallback } from 'react';
import {defaultSlots, FormSlot} from "@tmobis/core/components/entity-edit/field-extension";

export default function ReportForm({ isNew }: EditFormProps) {
	const data = useEntityEdit<Report>({
		typeMeta: ReportMeta,
		route: "reports",
		isNew
	});

	const { setHeaderData } = useHeader();

	useFocusEffect(
		useCallback(() => {
			if (isNew) {
				setHeaderData({ title: 'Новый отчет' });
			} else if (!data.formState.isReady) {
				setHeaderData({
					title: 'Отчет',
					subtitle: `${data.form.getValues().name}`,
				});
			} else {
				setHeaderData({
					title: 'Отчет',
					subtitle: `${data.form.getValues().name}`
				});
			}
		}, [data.formState.isReady, data.formState.state, isNew, data.form.getValues, setHeaderData])
	);

	return (
		<EntityEdit {...data}>
			<TmobisInput
				fieldName="name"
				label="Имя"
				isRequired={true}
			/>
			<TmobisInput
				fieldName="description"
				label="Описание"
			/>
			<TmobisInput
				fieldName="virtualName"
				label="Виртуальное имя"
			/>
			{/*<TmobisInput*/}
			{/*	fieldName="userScript"*/}
			{/*	label="Пользовательский скрипт"*/}
			{/*/>*/}
		</EntityEdit>
	);
}
