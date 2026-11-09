import { Employee, EmployeeClassName } from "@tmobis/core-business/models/employee";
import EntityListForm from "@tmobis/core/components/entity-list/form";
import useEntityList from "@tmobis/core/components/entity-list/use-entity-list";
import EntityListFilter from "@tmobis/core/components/entity-list/entity-list-filter";
import { useFocusEffect } from '@react-navigation/native';
import { useCallback } from 'react';
import { useHeader } from "@tmobis/core/components/header/context";
import { TmobisInput } from "@tmobis/core/components/input";
import { CoreBusinessListFormTypes } from "../../list-form-types";

export default function EmployeeList() {
	const { setHeaderData } = useHeader();
	const listData = useEntityList<Employee>({
		entityType: EmployeeClassName,
	});

	useFocusEffect(
		useCallback(() => {
			setHeaderData({ title: 'Сотрудники' });
		}, [setHeaderData])
	);

	return <EntityListForm
		editRoute='employees'
		listFormType={CoreBusinessListFormTypes.Employee}
		getRowConfig={() => ({
			mobileCardTitleColumn: 'fio',
		})}
		columns={[{
			column: 'fio',
			title: 'ФИО'
		}]}
		listData={listData}
		hasFilter={false}
		hasSearch={false}
	>
	</EntityListForm>
}
