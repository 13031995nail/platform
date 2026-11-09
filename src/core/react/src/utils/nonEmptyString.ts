export {};

declare global {
  type NonEmptyString = string & { __brand: 'nonEmpty' };
}

export const isNonEmptyString = (value: string): value is NonEmptyString => value.length > 0;

export const toNonEmptyString = (value: string): NonEmptyString => {
  if (!isNonEmptyString(value)) {
    throw new Error('String must be non-empty.');
  }
  return value as NonEmptyString;
};
