export default {
  projections: {
    AwpReadinessPassportL: {
      actNumber: {
        __caption__: 'Номер акта'
      },
      regDate: {
        __caption__: 'Дата оформления'
      },
      organization: {
        name: {
          __caption__: 'Контрагент'
        }
      },
      partnerReadiness: {
        __caption__: 'Уровень готовности'
      }
    }
  },
  validations: {
    readinessAct: {
      __caption__: 'Акт обеспечения готовности к отопительному периоду'
    },
    awp: {
      __caption__: 'Осенне-зимний период'
    },
    organization: {
      __caption__: 'Лицо, подлежащее оценке'
    }
  }
};
