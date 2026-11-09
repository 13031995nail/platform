import { User, UserClassName } from "@tmobis/core-business/models/user";
import EntityListForm from "@tmobis/core/components/entity-list/form";
import useEntityList from "@tmobis/core/components/entity-list/use-entity-list";
import EntityListFilter from "@tmobis/core/components/entity-list/entity-list-filter";
import { useFocusEffect } from '@react-navigation/native';
import { useCallback } from 'react';
import { useHeader } from "@tmobis/core/components/header/context";
import { TmobisInput } from "@tmobis/core/components/input";
import { CoreBusinessListFormTypes } from "../../list-form-types";

export default function UserList() {
	const { setHeaderData } = useHeader();
	const listData = useEntityList<User>({
		entityType: UserClassName,
		viewName: "UserE",
		filterFields: ['login', 'name']
	});

	useFocusEffect(
		useCallback(() => {
			setHeaderData({ title: 'Пользователи' });
		}, [setHeaderData])
	);

	return <EntityListForm
		editRoute='users'
		listFormType={CoreBusinessListFormTypes.User}
		hasFilter
		getRowConfig={(user) => ({
			mobileCardTitleColumn: 'name',
			canBeDeleted: user.actuality !== false,
		})}
		columns={[{
			column: 'login',
			title: 'Логин'
		},
		{
			column: 'name',
			title: 'Имя'
		},
		{
			column: 'email',
			title: 'Емейл'
		},
		{
			column: 'actuality',
			title: 'Актуальность'
		}]}
		listData={listData}
	>
		<EntityListFilter>
			{(filter, setFilter) => (
				<>
					<TmobisInput
						fieldName="login"
						label="Логин"
						value={filter.login}
						onChangeText={(newValue: string) => setFilter('login', newValue)}
					/>
					<TmobisInput
						fieldName="name"
						label="Имя"
						value={filter.name}
						onChangeText={(newValue: string) => setFilter('name', newValue)}
					/>
				</>
			)}
		</EntityListFilter>
	</EntityListForm>
}
