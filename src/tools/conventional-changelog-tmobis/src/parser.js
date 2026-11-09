export function createParserOpts(config) {
  return {
    headerPattern: /^(\w*)(?:\((.*)\))?!?: (.*)$/,
    breakingHeaderPattern: /^(\w*)(?:\((.*)\))?!: (.*)$/,
    headerCorrespondence: [
      'type',
      'scope',
      'subject'
    ],
    noteKeywords: ['BREAKING CHANGE', 'BREAKING-CHANGE'],
    revertPattern: /^(?:Откат|откат:)\s"?([\s\S]{0,5000}?)"?\s*This reverts commit (\w*)\./i,
    revertCorrespondence: ['header', 'hash'],
    issuePrefixes: config?.issuePrefixes || ['#']
  }
}
