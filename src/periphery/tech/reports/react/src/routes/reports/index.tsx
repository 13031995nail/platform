import { Report, ReportClassName } from "../../models/report";
import EntityListForm from "@tmobis/core/components/entity-list/form";
import useEntityList from "@tmobis/core/components/entity-list/use-entity-list";
import { useFocusEffect } from '@react-navigation/native';
import { useCallback } from 'react';
import { useHeader } from "@tmobis/core/components/header/context";
import { ReportsListFormTypes } from "../../list-form-types";

export default function ReportList() {
	const { setHeaderData } = useHeader();
	const listData = useEntityList<Report>({
		entityType: ReportClassName
	});

	useFocusEffect(
		useCallback(() => {
			setHeaderData({ title: 'Отчеты' });
		}, [setHeaderData])
	);

	return <EntityListForm
		editRoute='reports'
		listFormType={ReportsListFormTypes.Report}
		columns={[{
			column: 'name',
			title: 'Имя'
		},
		{
			column: 'description',
			title: 'Описание'
		},
		{
			column: 'virtualName',
			title: 'Виртуальное имя'
		}]}
		listData={listData}
	/>
}
