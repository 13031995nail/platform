export default {
  projections: {
    AdmissionCommitteeE: {
      approvedDate: {
        __caption__: 'Дата формирования'
      },
      status: {
        __caption__: 'Статус'
      },
      organization: {
        __caption__: 'Муниципальное образование'
      },
      awp: {
        __caption__: 'Осенне-зимний период'
      },
      deputyLeader: {
        __caption__: 'Заместитель председателя',
        employee: {
          __caption__: '',
          fio: {
            __caption__: 'Заместитель председателя'
          }
        }
      }
    },
    AdmissionCommitteeL: {
      organization: {
        __caption__: '',
          name: {
            __caption__: 'Муниципальное образование'
          }
      },
      approvedDate: {
        __caption__: 'Дата формирования'
      },
      leader: {
        __caption__: '',
        employee: {
          __caption__: '',
            fio: {
              __caption__: 'Председатель'
          }
        }
      }
    }
  },
  validations: {
    organization: {
      __caption__: 'Муниципальное образование'
    },
    awp: {
      __caption__: 'Осенне-зимний период',
    }
  }
};