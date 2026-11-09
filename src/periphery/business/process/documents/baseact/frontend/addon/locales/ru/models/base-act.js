export default {
  projections: {
    BaseActE: {
      regDate: {
        __caption__: 'Дата оформления'
      },
      status: {
        __caption__: 'Статус'
      },
      actNumber: {
        __caption__: 'Номер акта'
      },
      changeStatusDate: {
        __caption__: ''
      },
      organizationName: {
        __caption__: ''
      },
      organizationPhone: {
        __caption__: ''
      },
      organizationAddress: {
        __caption__: ''
      },
      actNumberIndex: {
        __caption__: ''
      },
      organization: {
        __caption__: 'Контрагент',
          organizationType: {
            __caption__: 'Тип контрагента',
              name: {
                __caption__: 'Наименование'
              }
          },
          name: {
            __caption__: 'Наименование'
          }
      },
      awp: {
        __caption__: 'Осенне-зимний период',
          name: {
            __caption__: 'Наименование'
          }
      },
      inspector: {
        __caption__: 'Сотрудник',
          firstName: {
            __caption__: ''
          },
          lastName: {
            __caption__: ''
          },
          patronymic: {
            __caption__: ''
          },
      },
      actType: {
        __caption__: 'Тип акта',
          name: {
            __caption__: 'Наименование'
          }
      }
    }
  },
  validations: {
    awp: {
      __caption__: 'Осенне-зимний период'
    },
    inspector: {
      __caption__: 'Сотрудник'
    },
    actType: {
      __caption__: 'Тип акта',
        name: {
          __caption__: 'Наименование'
        }
    }
  }
};