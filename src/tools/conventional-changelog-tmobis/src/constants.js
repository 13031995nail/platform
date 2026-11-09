export const DEFAULT_COMMIT_TYPES = Object.freeze([
  {
    type: 'feat',
    section: 'Изменения'
  },
  {
    type: 'feature',
    section: 'Изменения'
  },
  {
    type: 'fix',
    section: 'Исправления ошибок'
  },
  {
    type: 'perf',
    section: 'Производительность'
  },
  {
    type: 'revert',
    section: 'Откаты'
  },
  {
    type: 'docs',
    section: 'Документация'
  },
  {
    type: 'style',
    section: 'Стиль кода'
  },
  {
    type: 'chore',
    section: 'Прочее'
  },
  {
    type: 'refactor',
    section: 'Рефакторинг'
  },
  {
    type: 'test',
    section: 'Тесты'
  },
  {
    type: 'build',
    section: 'Сборка',
    hidden: true
  },
  {
    type: 'ci',
    section: 'CI/CD',
    hidden: true
  }
].map(Object.freeze))
