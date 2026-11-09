import { useFocusEffect } from '@react-navigation/native';
import EntityEdit, { EditFormProps } from "@tmobis/core/components/entity-edit";
import useEntityEdit from "@tmobis/core/components/entity-edit/use-entity-edit";
import { getFormStateLabel } from "@tmobis/core/components/form-state";
import { useHeader } from "@tmobis/core/components/header/context";
import { TmobisInput } from "@tmobis/core/components/input";
import { useCallback } from 'react';
import EmployeeMeta, {Employee} from "@tmobis/core-business/models/employee";

export default function EmployeeForm({ isNew }: EditFormProps) {
	const data = useEntityEdit<Employee>({
		typeMeta: EmployeeMeta,
		route: "employees",
		isNew,
	});

	const { setHeaderData } = useHeader();
	useFocusEffect(
		useCallback(() => {
			if (isNew) {
				setHeaderData({ title: 'Новый сотрудник' });
			} else if (!data.formState.isReady) {
				setHeaderData({
					title: 'Сотрудник',
					subtitle: getFormStateLabel(data.formState.state),
				});
			} else {
				setHeaderData({
					title: `Сотрудник ${data.form.getValues().fio}`
				});
			}
		}, [data.formState.isReady, data.formState.state, isNew, data.form.getValues, setHeaderData])
	);

	return (
		<EntityEdit {...data}>
			<TmobisInput
				fieldName="fio"
				label="ФИО"
				isReadonly={true}
			/>
		</EntityEdit>
	);
}
