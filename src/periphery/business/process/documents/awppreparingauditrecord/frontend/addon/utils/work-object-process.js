export const preparationStates = Object.freeze({
  notStarted: 'Не начато',
  inProgress: 'В работе',
  underReview: 'На проверке',
  remarks: 'Есть замечания',
  ready: 'Готов',
  completed: 'Завершено'
});

export function selectRequirements(requirements, section) {
  return (requirements || []).filter(item => {
    switch (section) {
      case 'documents': return Boolean(item.documentName || item.documentFile);
      case 'missing': return item.isCounted && !item.isFilled;
      case 'remarks': return Boolean(item.remark);
      default: return true;
    }
  });
}
