export default {
  projections: {
    OrganizationE: {
      actuality: {
        __caption__: 'Актуальность'
      },
      name: {
        __caption__: 'Наименование'
      },
      address: {
        __caption__: 'Адрес'
      },
      legalAddress: {
        __caption__: 'Юридический адрес'
      },
      phone: {
        __caption__: 'Номер телефона'
      },
      email: {
        __caption__: 'E-mail'
      },
      acronym: {
        __caption__: 'Акроним'
      },
      organizationType: {
        __caption__: 'Тип контрагента',
          name: {
            __caption__: 'Наименование'
          }
      },
      hierarchy: {
        __caption__: 'Муниципальное образование',
          name: {
            __caption__: 'Наименование'
          }
      }
    },
    OrganizationL: {
      name: {
        __caption__: 'Наименование'
      },
      legalAddress: {
        __caption__: 'Юридический адрес'
      },
      phone: {
        __caption__: 'Номер телефона'
      },
      organizationType: {
        __caption__: 'Тип контрагента',
          name: {
            __caption__: 'Тип контрагента'
          }
      }
    }
  },
  validations: {
    name: {
      __caption__: 'Наименование'
    },
    organizationType: {
      __caption__: 'Тип контрагента',
        name: {
          __caption__: 'Наименование'
        }
    },
    acronym: {
      __caption__: 'Акроним'
    },
    hierarchy: {
      __caption__: 'Муниципальное образование',
        name: {
          __caption__: 'Наименование'
        }
    },
  }
};